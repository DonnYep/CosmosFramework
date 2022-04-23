using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Cosmos.UI
{
    //================================================
    /*
     * 1、UI模块；由于UI模块没有强约束类型，因此是可以添加实现了IUIForm
     *  接口的自定义对象。
     */
    //================================================
    [Module]
    internal sealed partial class UIManager : Module, IUIManager
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
            var inct = this.Instance();
            uiRoot.SetParent(inct.transform);
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
            var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.AssetBundleName, attribute.AssetPath) { UIGroupName = attribute.UIGroupName };
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
                PeekUIForm(assetInfo.UIAssetName, out uiForm);
                ActiveUIForm(assetInfo.UIAssetName);
                return uiForm;
            }
            if (!string.IsNullOrEmpty(assetInfo.UIGroupName))
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
                    PeekUIForm(assetInfo.UIAssetName, out uiForm);
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
                var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.AssetBundleName, attribute.AssetPath) { UIGroupName = attribute.UIGroupName };
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
                    callback?.Invoke(uiForm);
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
        /// 通过UIAssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>Task异步任务</returns>
        public async Task<T> OpenUIFormAsync<T>(UIAssetInfo assetInfo)
            where T : class, IUIForm
        {
            T uiForm = null;
            await OpenUIFormAsync<T>(assetInfo, pnl => uiForm = pnl);
            return uiForm;
        }
        /// <summary>
        ///  通过UIAssetInfo加载UI对象
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="uiType">目标组件的type类型</param>
        /// <returns>Task异步任务</returns>
        public async Task<IUIForm> OpenUIFormAsync(UIAssetInfo assetInfo,Type uiType)
        {
            IUIForm uiForm = null;
            await OpenUIFormAsync(assetInfo,uiType, pnl => uiForm = pnl);
            return uiForm;
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
                var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.AssetBundleName, attribute.AssetPath) { UIGroupName = attribute.UIGroupName };
                return OpenUIFormAsync(assetInfo, uiType, callback);
            }
        }
        /// <summary>
        /// 关闭释放UIForm；
        /// 此操作会释放UIForm对象；
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        public void CloseUIForm(string uiFormName)
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
            uiFormAssetHelper.CloseUIForm(uiForm);
        }
        /// <summary>
        /// 关闭释放UIForm；
        /// 此操作会释放UIForm对象；
        /// </summary>
        /// <param name="uiForm">open的UIForm</param>
        public void CloseUIForm(IUIForm uiForm)
        {
            if (uiForm == null)
                throw new ArgumentNullException("UIForm is invalid.");
            var uiFormName = uiForm.UIFormName;
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (uiFormDict.Remove(uiFormName, out var srcUIForm))
            {
                if (srcUIForm != uiForm)
                    throw new ArgumentException($"{uiFormName}'s ptr is not equal !");//指针不一致
                if (!string.IsNullOrEmpty(uiForm.UIGroupName))
                {
                    uiGroupDict.TryGetValue(uiForm.UIGroupName, out var group);
                    group?.RemoveUIForm(uiForm);
                    if (group.UIFormCount <= 0)
                        uiGroupDict.Remove(uiForm.UIGroupName);
                }
                uiFormAssetHelper.CloseUIForm(uiForm);
            }
            else
                throw new ArgumentNullException($"UI  { uiFormName} is not existed or registered !");
        }
        /// <summary>
        /// 失活UIForm；
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
        ///  失活UIForm；
        /// </summary>
        /// <param name="uiForm">UIForm对象</param>
        public void DeactiveUIForm(IUIForm uiForm)
        {
            if (uiForm == null)
                throw new ArgumentNullException("UIForm is invalid.");
            var uiFormName = uiForm.UIFormName;
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (HasUIForm(uiFormName))
            {
                uiFormDict.TryGetValue(uiFormName, out var srcUIForm);
                if (srcUIForm == uiForm)
                    uiFormMotionHelper.DeactiveUIForm(uiForm);
                else
                    throw new ArgumentException($"{uiFormName}'s ptr is not equal !");//指针不一致
            }
        }
        /// <summary>
        /// 激活UIForm；
        /// </summary>
        /// <param name="uiForm">UIForm对象</param>
        public void ActiveUIForm(IUIForm uiForm)
        {
            if (uiForm == null)
                throw new ArgumentNullException("UIForm is invalid.");
            var uiFormName = uiForm.UIFormName;
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            if (HasUIForm(uiFormName))
            {
                uiFormDict.TryGetValue(uiFormName, out var srcUIForm);
                if (srcUIForm == uiForm)
                    uiFormMotionHelper.ActiveUIForm(uiForm);
                else
                    throw new ArgumentException($"{uiFormName}'s ptr is not equal !");//指针不一致
            }
        }
        /// <summary>
        /// 激活UIForm；
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
        /// 释放关闭整个组；
        /// </summary>
        /// <param name="uiGroupName">UI组的名字</param>
        public void CloseUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                var uiFormArray = group.GetAllUIForm();
                var length = uiFormArray.Length;
                for (int i = 0; i < length; i++)
                    uiFormAssetHelper.CloseUIForm(uiFormArray[i]);
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
        /// <see cref=" IUIForm",>
        /// UIForm.UIName
        /// </summary>
        /// <param name="uiFormName">UI资源的名称</param>
        /// <returns>存在的结果</returns>
        public bool HasUIForm(string uiFormName)
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            return uiFormDict.ContainsKey(uiFormName);
        }
        /// <summary>
        /// 获取UIForm；
        /// </summary>
        public bool PeekUIForm<T>(string uiFormName, out T uiForm)
            where T : class, IUIForm
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            var rst = uiFormDict.TryGetValue(uiFormName, out var form);
            uiForm = form as T;
            return rst;
        }
        /// <summary>
        /// 获取UIForm；
        /// </summary>
        public bool PeekUIForm(string uiFormName, out IUIForm uiForm)
        {
            Utility.Text.IsStringValid(uiFormName, "UIFormName is invalid !");
            return uiFormDict.TryGetValue(uiFormName, out uiForm);
        }
        /// <summary>
        /// 通过条件选择组中的UIForm；
        /// </summary>
        /// <param name="uiGroupName">UI组的名字</param>
        /// <param name="handler">条件委托</param>
        /// <returns>符合条件的UIForm</returns>
        public IUIForm[] FindUIForms(string uiGroupName, Predicate<IUIForm> handler)
        {
            Utility.Text.IsStringValid(uiGroupName, "UIGroupName is invalid !");
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
        protected override void OnInitialization()
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
