using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Cosmos.UI
{
    [Module]
    internal sealed class UIManager : Module, IUIManager
    {
        #region Properties
        /// <summary>
        /// UI 根对象；
        /// </summary>
        public Transform UIRoot { get; private set; }
        Type uiFromBaseType = typeof(IUIForm);
        /// <summary>
        /// UI动效帮助体；
        /// </summary>
        IUIFormMotionHelper uiFormMotionHelper;
        /// <summary>
        /// UI资产帮助体；
        /// </summary>
        IUIFormAssetHelper uiFormAssetHelper;

        /// <summary>
        /// UIGroupName===UIGroup；
        /// </summary>
        Dictionary<string, IUIFormGroup> uiGroupDict;
        /// <summary>
        /// UIFormName===UIForm；
        /// </summary>
        Dictionary<string, IUIForm> uiFormDict;
        #endregion
        #region Methods
        /// <summary>
        /// 设置UI根节点
        /// </summary>
        /// <param name="uiRoot">传入的UIRoot</param>
        /// <param name="destroyOldOne">销毁旧的uiRoot对象</param>
        public void SetUIRoot(Transform uiRoot, bool destroyOldOne = false)
        {
            if (destroyOldOne)
                GameObject.Destroy(UIRoot.gameObject);
            var mountGo = GameManager.GetModuleMount<IUIManager>();
            uiRoot.SetParent(mountGo.transform);
            UIRoot = uiRoot;
        }
        /// <summary>
        /// 设置ui动效帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        public void SetMotionHelper(IUIFormMotionHelper helper)
        {
            this.uiFormMotionHelper = helper;
        }
        /// <summary>
        /// 设置ui资产加载帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        public void SetUIFormAssetHelper(IUIFormAssetHelper helper)
        {
            this.uiFormAssetHelper = helper;
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <returns>生成的UI对象Comp</returns>
        public T OpenUIForm<T>()
            where T : class, IUIForm
        {
            return OpenUIForm(typeof(T)) as T;
        }
        /// <summary>
        ///  通过特性UIAssetAttribute加载Panel（同步）
        /// </summary>
        /// <param name="uiType">目标组件的type类型</param>
        /// <returns>生成的UI对象Comp</returns>
        public IUIForm OpenUIForm(Type uiType)
        {
            var attribute = GetTypeAttribute(uiType);
            var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.UIGroupName, attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
            return OpenUIForm(assetInfo, uiType);
        }
        /// <summary>
        ///  通过UIAssetInfo加载UI对象（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public T OpenUIForm<T>(UIAssetInfo assetInfo)
            where T : class, IUIForm
        {
            return OpenUIForm(assetInfo, typeof(T)) as T;
        }
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="uiType">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public IUIForm OpenUIForm(UIAssetInfo assetInfo, Type uiType)
        {
            IUIForm uiForm = null;
            CheckUIAssetInfoValid(assetInfo, uiType);
            if (HasUIForm(assetInfo.UIAssetName))
            {
                uiForm = PeekUIForm(assetInfo.UIAssetName);
                ActiveUIForm(assetInfo.UIAssetName);
                return uiForm;
            }
            if (string.IsNullOrEmpty(assetInfo.UIGroupName))
            {
                if (!uiGroupDict.TryGetValue(assetInfo.UIGroupName, out var group))
                {
                    group = new UIFormGroup(assetInfo.UIGroupName);
                    uiGroupDict.Add(group.UIGroupName, group);
                }
                if (group.HasUIForm(assetInfo.UIAssetName))
                    throw new ArgumentException($"UIGroup:{assetInfo.UIGroupName}---UIFormName: {assetInfo.UIAssetName}has already existed !");
                else
                {
                    uiForm = PeekUIForm(assetInfo.UIAssetName);
                    ActiveUIForm(assetInfo.UIAssetName);
                    return uiForm;
                }
            }
            else
            {
                uiForm = uiFormAssetHelper.InstanceUIForm(assetInfo, uiType);
                uiFormDict.Add(uiForm.UIFormName, uiForm);
                uiFormAssetHelper.AttachTo(uiForm, UIRoot);
                return uiForm;
            }
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync<T>(Action<T> callback = null)
    where T : class, IUIForm
        {

            Type uiType = typeof(T);
            var attribute = GetTypeAttribute(uiType);
            if (string.IsNullOrEmpty(attribute.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (HasUIForm(attribute.UIAssetName))
            {
                ActiveUIForm(attribute.UIAssetName);
                return null;
            }
            else
            {
                var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.UIGroupName, attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return OpenUIFormAsync(assetInfo, typeof(T), uiForm => { callback?.Invoke(uiForm as T); });
            }
        }
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（异步）；
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="uiType">目标组件的type类型</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<IUIForm> callback = null)
        {
            CheckUIAssetInfoValid(assetInfo, uiType);
            if (HasUIForm(assetInfo.UIAssetName))
            {
                ActiveUIForm(assetInfo.UIAssetName);
                return null;
            }
            else
                return uiFormAssetHelper.InstanceUIFormAsync(assetInfo, uiType, uiForm =>
                {
                    uiFormAssetHelper.AttachTo(uiForm, UIRoot);
                    callback?.Invoke(uiForm);
                    uiFormDict.Add(assetInfo.UIAssetName, uiForm);
                    if (!string.IsNullOrEmpty(assetInfo.UIGroupName))
                    {
                        if (!uiGroupDict.TryGetValue(assetInfo.UIGroupName, out var group))
                        {
                            group = new UIFormGroup(assetInfo.UIGroupName);
                            uiGroupDict.Add(assetInfo.UIGroupName, group);
                        }
                        group.AddUIForm(uiForm);
                    }
                });
        }
        /// <summary>
        /// 通过UIAssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync<T>(UIAssetInfo assetInfo, Action<T> callback = null)
            where T : class, IUIForm
        {
            var type = typeof(T);
            CheckUIAssetInfoValid(assetInfo, type);
            if (HasUIForm(assetInfo.UIAssetName))
            {
                ActiveUIForm(assetInfo.UIAssetName);
                return null;
            }
            else
                return OpenUIFormAsync(assetInfo, typeof(T), uiForm => { callback?.Invoke(uiForm as T); });
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <param name="uiType">带有UIAssetAttribute特性的panel类</param>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync(Type uiType, Action<IUIForm> callback = null)
        {
            var attribute = GetTypeAttribute(uiType);
            if (string.IsNullOrEmpty(attribute.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (HasUIForm(attribute.UIAssetName))
            {
                ActiveUIForm(attribute.UIAssetName);
                return null;
            }
            else
            {
                var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.UIGroupName, attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return OpenUIFormAsync(assetInfo, uiType, callback);
            }
        }
        /// <summary>
        /// 释放UIForm；
        /// 此操作会释放UIForm对象；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        public void ReleaseUIForm(string uiFormName)
        {
            CheckUIFormValid(uiFormName);
            uiFormDict.Remove(uiFormName, out var uiForm);
            if (!string.IsNullOrEmpty(uiForm.UIGroupName))
            {
                uiGroupDict.TryGetValue(uiForm.UIGroupName, out var group);
                group?.RemoveUIForm(uiForm);
                if (group.UIFormCount <= 0)
                    uiGroupDict.Remove(uiForm.UIGroupName);
            }
            uiFormAssetHelper.ReleaseUIForm(uiForm);
        }
        /// <summary>
        /// 失活UIForm，并触发UIForm中的OnDeactive回调；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        public void DeactiveUIForm(string uiFormName)
        {
            CheckUIFormValid(uiFormName);
            if (HasUIForm(uiFormName))
            {
                uiFormDict.TryGetValue(uiFormName, out var uiForm);
                uiFormMotionHelper.DeactiveUIForm(uiForm);
            }
        }
        /// <summary>
        /// 激活UIForm,并触发UIForm中的OnActive回调；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        public void ActiveUIForm(string uiFormName)
        {
            CheckUIFormValid(uiFormName);
            if (HasUIForm(uiFormName))
            {
                uiFormDict.TryGetValue(uiFormName, out var uiForm);
                uiFormMotionHelper.ActiveUIForm(uiForm);
            }
        }
        /// <summary>
        /// 释放整个组；
        /// </summary>
        /// <param name="uiGroupName">UI组的名字</param>
        public void ReleaseUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                var uiFormArray = group.GetAllUIForm();
                var length = uiFormArray.Length;
                for (int i = 0; i < length; i++)
                    uiFormAssetHelper.ReleaseUIForm(uiFormArray[i]);
                uiGroupDict.Remove(uiGroupName);
            }
        }
        /// <summary>
        /// 失活整个ui组；
        /// </summary>
        /// <param name="uiGroupName">UI组的名字</param>
        public void DeactiveUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                var uiFormArray = group.GetAllUIForm();
                var length = uiFormArray.Length;
                for (int i = 0; i < length; i++)
                    uiFormMotionHelper.DeactiveUIForm(uiFormArray[i]);
            }
        }
        /// <summary>
        /// 激活整个UI组；
        /// </summary>
        /// <param name="uiGroupName">ui组的名字</param>
        public void ActiveUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                var uiFormArray = group.GetAllUIForm();
                var length = uiFormArray.Length;
                for (int i = 0; i < length; i++)
                    uiFormMotionHelper.ActiveUIForm(uiFormArray[i]);
            }
        }
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UIForm",>
        /// UIForm.UIName
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        /// <returns>存在的结果</returns>
        public bool HasUIForm(string uiFormName)
        {
            if (string.IsNullOrEmpty(uiFormName))
                throw new ArgumentException("UIFormName is invalid !");
            return uiFormDict.ContainsKey(uiFormName);
        }
        /// <summary>
        /// 获取UIForm；
        /// </summary>
        public T PeekUIForm<T>(string uiFormName)
            where T : class, IUIForm
        {
            return PeekUIForm(uiFormName) as T;
        }
        /// <summary>
        /// 获取UIForm；
        /// </summary>
        public IUIForm PeekUIForm(string uiFormName)
        {
            CheckUIFormValid(uiFormName);
            uiFormDict.TryGetValue(uiFormName, out var uiForm);
            return uiForm;
        }
        /// <summary>
        /// 通过条件选择组中的UIForm；
        /// </summary>
        /// <param name="uiGroupName">UI组的名字</param>
        /// <param name="handler">条件委托</param>
        /// <returns>符合条件的UIForm</returns>
        public IUIForm[] FindUIForms(string uiGroupName, Predicate<IUIForm> handler)
        {
            if (string.IsNullOrEmpty(uiGroupName))
                throw new ArgumentNullException("UIGroupName is invalid !");
            if (handler == null)
                throw new ArgumentNullException("Handler is invalid !");
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
                return group.GetUIForms(handler);
            else
                throw new ArgumentException($"UIGroup  {uiGroupName} is not existed !");
        }
        /// <summary>
        /// 通过条件选择UIForm
        /// </summary>
        /// <param name="handler">条件委托</param>
        /// <returns>符合条件的UIForm</returns>
        public IUIForm[] FindtUIForms(Predicate<IUIForm> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("Handler is invalid !");
            var dst = new IUIForm[uiFormDict.Count];
            int idx = 0;
            foreach (var uiForm in uiFormDict.Values)
            {
                if (handler.Invoke(uiForm))
                {
                    dst[idx] = uiForm;
                    idx++;
                }
            }
            Array.Resize(ref dst, idx);
            return dst;
        }
        /// <summary>
        /// 设置UIForm的组别；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        /// <param name="uiGroupName">UI组的名字</param>
        public void GroupUIForm(string uiFormName, string uiGroupName)
        {
            CheckUIFormValid(uiFormName);
            if (string.IsNullOrEmpty(uiGroupName))
                throw new ArgumentNullException("UIGroupName is invalid !");
            if (!uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                group = new UIFormGroup(uiGroupName);
                uiGroupDict.Add(uiGroupName, group);
            }
            var uiForm = uiFormDict[uiFormName];
            if (!string.IsNullOrEmpty(uiForm.UIGroupName))
            {
                if (uiGroupDict.TryGetValue(uiGroupName, out var latestGroup))
                    latestGroup.RemoveUIForm(uiForm);
            }
            group.AddUIForm(uiForm);
        }
        /// <summary>
        /// 解除UIForm的组别；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        public void UngroupUIForm(string uiFormName)
        {
            CheckUIFormValid(uiFormName);
            var uiForm = uiFormDict[uiFormName];
            if (!string.IsNullOrEmpty(uiForm.UIGroupName))
            {
                if (uiGroupDict.TryGetValue(uiForm.UIGroupName, out var group))
                    group.RemoveUIForm(uiForm);
            }
        }
        protected override void OnPreparatory()
        {
            uiGroupDict = new Dictionary<string, IUIFormGroup>();
            uiFormDict = new Dictionary<string, IUIForm>();
            uiFormMotionHelper = new DefaultUIFormHelper();
            uiFormAssetHelper = new DefaultUIFormAssetHelper();
        }
        /// <summary>
        /// 检测UIForm的名字是否有效，且是否存在；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        void CheckUIFormValid(string uiFormName)
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (!uiFormDict.ContainsKey(uiFormName))
                throw new ArgumentNullException($"UI  { uiFormName} is not existed or registered !");
        }
        void CheckUIAssetInfoValid(UIAssetInfo assetInfo, Type uiType)
        {
            if (assetInfo == null)
                throw new ArgumentNullException("UIAssetInfo is invalid !");
            if (string.IsNullOrEmpty(assetInfo.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIForm");
        }
        UIAssetAttribute GetTypeAttribute(Type uiType)
        {
            if (uiType == null)
                throw new ArgumentNullException("UIType is invalid !");
            var attribute = uiType.GetCustomAttribute<UIAssetAttribute>();
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new ArgumentException($"Type:{uiType} is not inherit from UIForm");
            if (attribute == null)
                throw new ArgumentNullException($"Type:{uiType} has no UIAssetAttribute");
            return attribute;
        }
        #endregion
    }
}
