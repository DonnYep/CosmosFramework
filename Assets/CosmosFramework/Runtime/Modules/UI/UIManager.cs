using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using Cosmos.Resource;
namespace Cosmos.UI
{
    [Module]
    internal sealed class UIManager : Module, IUIManager
    {
        #region Properties
        public GameObject UIRoot { get; private set; }
        Type uiFromBaseType = typeof(UIForm);
        IUIFormHelper uiFormHelper;

        IUIFormAssetHelper uiFormAssetHelper;

        List<UIForm> peerFormCache;
        Dictionary<string, UIFormGroup> uiGroupDict;
        /// <summary>
        /// UIFormName===UIForm
        /// </summary>
        Dictionary<string, UIForm> uiFormDict;
        #endregion
        #region Methods
        public override void OnPreparatory()
        {
            peerFormCache = new List<UIForm>();
            uiGroupDict = new Dictionary<string, UIFormGroup>();
            uiFormDict = new Dictionary<string, UIForm>();
            uiFormHelper = new DefaultUIFormHelper();
            uiFormAssetHelper = new DefaultUIFormAssetHelper();
        }
        /// <summary>
        /// 设置UI根节点
        /// </summary>
        /// <param name="uiRoot">传入的UIRoot</param>
        /// <param name="destroyOldOne">销毁旧的uiRoot对象</param>
        public void SetUIRoot(GameObject uiRoot, bool destroyOldOne = false)
        {
            if (destroyOldOne)
                GameObject.Destroy(UIRoot);
            var mountGo = GameManager.GetModuleMount<IUIManager>();
            uiRoot.transform.SetParent(mountGo.transform);
            UIRoot = uiRoot;
        }
        public void SetHelper(IUIFormHelper helper)
        {
            this.uiFormHelper = helper;
        }
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
            where T : UIForm
        {
            return OpenUIForm(typeof(T)) as T;
        }
        /// <summary>
        ///  通过特性UIAssetAttribute加载Panel（同步）
        /// </summary>
        /// <param name="uiType">目标组件的type类型</param>
        /// <returns>生成的UI对象Comp</returns>
        public UIForm OpenUIForm(Type uiType)
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
            where T : UIForm
        {
            return OpenUIForm(assetInfo, typeof(T)) as T;
        }
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="uiType">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public UIForm OpenUIForm(UIAssetInfo assetInfo, Type uiType)
        {
            UIForm uiForm = null;
            CheckUIAssetInfoValid(assetInfo, uiType);
            if (HasUIForm(assetInfo.UIAssetName))
            {
                uiForm = PeekUIForm(assetInfo.UIAssetName);
                ShowUIForm(assetInfo.UIAssetName);
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
                    ShowUIForm(assetInfo.UIAssetName);
                    return uiForm;
                }
            }
            else
            {
                return SpawnUIForm(assetInfo, uiType);
            }
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync<T>(Action<T> loadDoneCallback = null)
    where T : UIForm
        {

            Type uiType = typeof(T);
            var attribute = GetTypeAttribute(uiType);
            if (string.IsNullOrEmpty(attribute.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (HasUIForm(attribute.UIAssetName))
            {
                ShowUIForm(attribute.UIAssetName);
                return null;
            }
            else
            {
                var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.UIGroupName, attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return OpenUIFormAsync(assetInfo, typeof(T), uiForm => { loadDoneCallback?.Invoke(uiForm as T); });
            }
        }
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（异步）；
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="uiType">目标组件的type类型</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<UIForm> loadDoneCallback = null)
        {
            CheckUIAssetInfoValid(assetInfo, uiType);
            if (HasUIForm(assetInfo.UIAssetName))
            {
                ShowUIForm(assetInfo.UIAssetName);
                return null;
            }
            else
                return uiFormAssetHelper.InstanceUIFormAsync(assetInfo, uiType, uiForm =>
                {
                    uiForm.transform.SetParent(UIRoot.transform);
                    (uiForm.transform as RectTransform).ResetRectTransform();
                    loadDoneCallback?.Invoke(uiForm);
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
                    SortUIForm(uiForm);
                });
        }
        /// <summary>
        /// 通过UIAssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync<T>(UIAssetInfo assetInfo, Action<T> loadDoneCallback = null)
            where T : UIForm
        {
            var type = typeof(T);
            CheckUIAssetInfoValid(assetInfo, type);
            if (HasUIForm(assetInfo.UIAssetName))
            {
                ShowUIForm(assetInfo.UIAssetName);
                return null;
            }
            else
                return OpenUIFormAsync(assetInfo, typeof(T), uiForm => { loadDoneCallback?.Invoke(uiForm as T); });
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <param name="uiType">带有UIAssetAttribute特性的panel类</param>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync(Type uiType, Action<UIForm> loadDoneCallback = null)
        {
            var attribute = GetTypeAttribute(uiType);
            if (string.IsNullOrEmpty(attribute.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (HasUIForm(attribute.UIAssetName))
            {
                ShowUIForm(attribute.UIAssetName);
                return null;
            }
            else
            {
                var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.UIGroupName, attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return OpenUIFormAsync(assetInfo, uiType, loadDoneCallback);
            }
        }
        public void CloseUIForm(string uiAssetName)
        {
            CheckUIFormValid(uiAssetName);
            uiFormDict.Remove(uiAssetName, out var uiForm);
            if (!string.IsNullOrEmpty(uiForm.GroupName))
            {
                uiGroupDict.TryGetValue(uiForm.GroupName, out var group);
                group?.RemoveUIForm(uiForm);
                if (group.UIFormCount <= 0)
                    uiGroupDict.Remove(uiForm.GroupName);
            }
            uiFormAssetHelper.ReleaseUIForm(uiForm);
        }
        /// <summary>
        /// 隐藏UI，调用UI中的HidePanel方法；
        /// <see cref=" UIForm",>
        /// UIForm.UIName
        /// </summary>
        /// <param name="uiAssetName">ui资源的名称</param>
        public void HideUIForm(string uiAssetName)
        {
            CheckUIFormValid(uiAssetName);
            if (HasUIForm(uiAssetName))
            {
                uiFormDict.TryGetValue(uiAssetName, out var uiForm);
                uiFormHelper.HideUIForm(uiForm);
                Utility.Assembly.InvokeMethod(uiForm, "OnHide", null);
            }
        }
        public void ShowUIForm(string uiAssetName)
        {
            CheckUIFormValid(uiAssetName);
            if (HasUIForm(uiAssetName))
            {
                uiFormDict.TryGetValue(uiAssetName, out var uiForm);
                uiFormHelper.ShowUIForm(uiForm);
                Utility.Assembly.InvokeMethod(uiForm, "OnShow", null);
            }
        }
        public void CloseUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                UIForm[] uiFormArray = group.GetAllUIForm();
                var length = uiFormArray.Length;
                for (int i = 0; i < length; i++)
                    uiFormAssetHelper.ReleaseUIForm(uiFormArray[i]);
                uiGroupDict.Remove(uiGroupName);
            }
        }
        public void HideUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                UIForm[] uiFormArray = group.GetAllUIForm();
                var length = uiFormArray.Length;
                for (int i = 0; i < length; i++)
                    HideUIForm(uiFormArray[i]);
            }
        }
        public void ShowUIGroup(string uiGroupName)
        {
            if (uiGroupDict.TryGetValue(uiGroupName, out var group))
            {
                UIForm[] uiFormArray = group.GetAllUIForm();
                var length = uiFormArray.Length;
                for (int i = 0; i < length; i++)
                    ShowUIForm(uiFormArray[i]);
            }
        }
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UIForm",>
        /// UIForm.UIName
        /// </summary>
        /// <param name="uiAssetName">ui资源的名称</param>
        /// <returns>存在的结果</returns>
        public bool HasUIForm(string uiAssetName)
        {
            if (string.IsNullOrEmpty(uiAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            return uiFormDict.ContainsKey(uiAssetName);
        }
        public T PeekUIForm<T>(string uiAssetName)
            where T : UIForm
        {
            return PeekUIForm(uiAssetName) as T;
        }
        public UIForm PeekUIForm(string uiAssetName)
        {
            CheckUIFormValid(uiAssetName);
            uiFormDict.TryGetValue(uiAssetName, out var uiForm);
            return uiForm;
        }
        /// <summary>
        /// 检测UIForm的名字是否有效，且是否存在；
        /// </summary>
        /// <param name="uiAssetName">UI资源的名称</param>
        void CheckUIFormValid(string uiAssetName)
        {
            if (string.IsNullOrEmpty(uiAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (!uiFormDict.ContainsKey(uiAssetName))
                throw new ArgumentNullException($"UI  { uiAssetName} is not existed or registered !");
        }
        void HideUIForm(UIForm uiForm)
        {
            uiFormHelper.HideUIForm(uiForm);
            Utility.Assembly.InvokeMethod(uiForm, "OnHide", null);
        }
        void ShowUIForm(UIForm uiForm)
        {
            uiFormHelper.ShowUIForm(uiForm);
            Utility.Assembly.InvokeMethod(uiForm, "OnShow", null);
        }
        void SortUIForm(UIForm uiForm)
        {
            //return;
            //peerFormCache.Clear();
            //var peers = Utility.Unity.Peers(uiForm.transform);
            //var length = peers.Length;
            //for (int i = 0; i < length; i++)
            //{
            //    if (uiDict.TryGetValue(peers[i].UI out var uiFormComp))
            //    {
            //        peerFormCache.Add(uiFormComp);
            //    }
            //}
            //if (peerFormCache.Count > 0)
            //    Utility.Unity.SortCompsByAscending(peerFormCache.ToArray(), (form) => form.Priority);
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
        UIForm SpawnUIForm(UIAssetInfo assetInfo, Type uiType)
        {
            var uiForm = uiFormAssetHelper.InstanceUIForm(assetInfo, uiType);
            uiFormDict.Add(uiForm.UIFormName, uiForm);
            uiForm.transform.SetParent(UIRoot.transform);
            (uiForm.transform as RectTransform).ResetRectTransform();
            SortUIForm(uiForm);
            return uiForm;
        }
        #endregion

    }
}
