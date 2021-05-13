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
        Dictionary<string, UIFormBase> uiDict;
        Type uiFromBaseType = typeof(UIFormBase);
        IUIFormHelper uiFormHelper;
        IResourceManager resourceManager;
        List<UIFormBase> peerFormCache;
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            uiDict = new Dictionary<string, UIFormBase>();
            peerFormCache = new List<UIFormBase>();
            uiFormHelper = new DefaultUIFormHelper();
        }
        public override void OnPreparatory()
        {
            resourceManager = GameManager.GetModule<IResourceManager>();
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
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <returns>生成的UI对象Comp</returns>
        public T OpenUIForm<T>()
            where T : UIFormBase
        {
            return OpenUIForm(typeof(T)) as T;
        }
        /// <summary>
        ///  通过特性UIAssetAttribute加载Panel（同步）
        /// </summary>
        /// <param name="uiType">目标组件的type类型</param>
        /// <returns>生成的UI对象Comp</returns>
        public UIFormBase OpenUIForm(Type uiType)
        {
            if (uiFromBaseType.IsAssignableFrom(uiType))
            {
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIFormBase");
            }
            var attribute = uiType.GetCustomAttribute<UIAssetAttribute>();
            if (attribute == null)
            {
                throw new NotImplementedException($"Type:{uiType} has no UIAssetAttribute");
            }
            var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
            return OpenUIForm(assetInfo, uiType);
        }
        /// <summary>
        ///  通过UIAssetInfo加载UI对象（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public T OpenUIForm<T>(UIAssetInfo assetInfo)
            where T : UIFormBase
        {
            return OpenUIForm(assetInfo, typeof(T)) as T;
        }
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="uiType">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public UIFormBase OpenUIForm(UIAssetInfo assetInfo, Type uiType)
        {
            if (!uiFromBaseType.IsAssignableFrom(uiType))
            {
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIFormBase");
            }
            if (string.IsNullOrEmpty(assetInfo.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (HasUIForm(assetInfo.UIAssetName))
            {
                var uiForm = PeekUIForm(assetInfo.UIAssetName);
                uiFormHelper.ShowUIForm(uiForm);
                return uiForm;
            }
            var go = resourceManager.LoadPrefab(assetInfo, true);
            go?.transform.SetParent(UIRoot.transform);
            (go?.transform as RectTransform).ResetRectTransform();
            var comp = go.GetOrAddComponent(uiType)as UIFormBase;
            uiDict.Add(assetInfo.UIAssetName, comp);
            SortUIForm(comp);
            return comp;
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync<T>(Action<T> loadDoneCallback = null)
    where T : UIFormBase
        {
            Type type = typeof(T);
            var attribute = type.GetCustomAttribute<UIAssetAttribute>();
            if (attribute == null)
            {
                throw new ArgumentNullException($"Type:{type} has no UIAssetAttribute");
            }
            if (string.IsNullOrEmpty(attribute.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (HasUIForm(attribute.UIAssetName))
            {
                return Utility.Unity.StartCoroutine(() =>
                {
                    var uiForm = PeekUIForm(attribute.UIAssetName);
                    uiFormHelper.ShowUIForm(uiForm);
                });
            }
            else
            {
                var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return resourceManager.LoadPrefabAsync(assetInfo, panel =>
                 {
                     panel.transform.SetParent(UIRoot.transform);
                     (panel.transform as RectTransform).ResetRectTransform();
                     var comp = panel.GetOrAddComponent<T>();
                     loadDoneCallback?.Invoke(comp);
                     uiDict.Add(assetInfo.UIAssetName, comp);
                     SortUIForm(comp);
                 }, null, true);
            }
        }
        /// <summary>
        /// 通过UIAssetInfo加载UI对象（异步）；
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="uiType">目标组件的type类型</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync(UIAssetInfo assetInfo, Type uiType, Action<UIFormBase> loadDoneCallback = null)
        {
            if (!uiFromBaseType.IsAssignableFrom(uiType))
            {
                throw new ArgumentException($"Type:{uiType} is not inherit from UIFormBase");
            }
            if (string.IsNullOrEmpty(assetInfo.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (HasUIForm(assetInfo.UIAssetName))
            {
                return Utility.Unity.StartCoroutine(() =>
                {
                    var uiForm = PeekUIForm(assetInfo.UIAssetName);
                    uiFormHelper.ShowUIForm(uiForm);
                });
            }
            else
                return resourceManager.LoadPrefabAsync(assetInfo, go =>
                 {
                     go.transform.SetParent(UIRoot.transform);
                     (go.transform as RectTransform).ResetRectTransform();
                     var comp = go.GetOrAddComponent(uiType)as UIFormBase;
                     loadDoneCallback?.Invoke(comp);
                     uiDict.Add(assetInfo.UIAssetName, comp);
                     SortUIForm(comp);
                 }, null, true);
        }
        /// <summary>
        /// 通过UIAssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync<T>(UIAssetInfo assetInfo, Action<T> loadDoneCallback = null)
            where T : UIFormBase
        {
            var type = typeof(T);
            if (!uiFromBaseType.IsAssignableFrom(type))
                throw new ArgumentException($"Type:{type} is not inherit from UIFormBase");
            if (string.IsNullOrEmpty(assetInfo.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (HasUIForm(assetInfo.UIAssetName))
            {
                return Utility.Unity.StartCoroutine(() =>
                {
                    var uiForm = PeekUIForm(assetInfo.UIAssetName);
                    uiFormHelper.ShowUIForm(uiForm);
                });
            }
            else
                return resourceManager.LoadPrefabAsync(assetInfo, go =>
                {
                    go.transform.SetParent(UIRoot.transform);
                    (go.transform as RectTransform).ResetRectTransform();
                    var comp = go.GetOrAddComponent<T>();
                    loadDoneCallback?.Invoke(comp);
                    uiDict.Add(assetInfo.UIAssetName, comp);
                    SortUIForm(comp);
                }, null, true);
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <param name="uiType">带有UIAssetAttribute特性的panel类</param>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns>协程对象</returns>
        public Coroutine OpenUIFormAsync(Type uiType, Action<UIFormBase> loadDoneCallback = null)
        {
            var attribute = uiType.GetCustomAttribute<UIAssetAttribute>();
            if (!uiFromBaseType.IsAssignableFrom(uiType))
                throw new ArgumentException($"Type:{uiType} is not inherit from UIFormBase");
            if (attribute == null)
                throw new ArgumentNullException($"Type:{uiType} has no UIAssetAttribute");
            if (string.IsNullOrEmpty(attribute.UIAssetName))
                throw new ArgumentException("UIFormName is invalid !");
            if (HasUIForm(attribute.UIAssetName))
            {
                return Utility.Unity.StartCoroutine(() =>
                {
                    var uiForm = PeekUIForm(attribute.UIAssetName);
                    uiFormHelper.ShowUIForm(uiForm);
                });
            }
            else
            {
                var assetInfo = new UIAssetInfo(attribute.UIAssetName, attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return resourceManager.LoadPrefabAsync(assetInfo, go =>
                 {
                     go.transform.SetParent(UIRoot.transform);
                     (go.transform as RectTransform).ResetRectTransform();
                     var comp =go.GetOrAddComponent(uiType)as UIFormBase;
                     loadDoneCallback?.Invoke(comp);
                     uiDict.Add(assetInfo.UIAssetName, comp);
                     SortUIForm(comp);
                 }, null, true);
            }
        }
        public void CloseUIForm(string uiAssetName)
        {
            CheckUIFormValid(uiAssetName);
            uiDict.Remove(uiAssetName, out var uiForm);
            uiFormHelper.CloseUIForm(uiForm);
        }
        /// <summary>
        /// 隐藏UI，调用UI中的HidePanel方法；
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiAssetName">ui资源的名称</param>
        public void HideUIForm(string uiAssetName)
        {
            CheckUIFormValid(uiAssetName);
            uiDict.TryGetValue(uiAssetName, out var uiForm);
            uiFormHelper.HideUIForm(uiForm);
        }
        public void ShowUIForm(string uiAssetName)
        {
            CheckUIFormValid(uiAssetName);
            uiDict.TryGetValue(uiAssetName, out var uiForm);
            uiFormHelper.ShowUIForm(uiForm);
        }
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiAssetName">ui资源的名称</param>
        /// <returns>存在的结果</returns>
        public bool HasUIForm(string uiAssetName)
        {
            return uiDict.ContainsKey(uiAssetName);
        }
        public T PeekUIForm<T>(string uiAssetName)
            where T : UIFormBase
        {
            return PeekUIForm(uiAssetName) as T;
        }
        public UIFormBase PeekUIForm(string uiAssetName)
        {
            CheckUIFormValid(uiAssetName);
            uiDict.TryGetValue(uiAssetName, out var uiForm);
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
            if (!uiDict.ContainsKey(uiAssetName))
                throw new ArgumentNullException($"UI  { uiAssetName} is not existed or registered !");
        }
        void SortUIForm(UIFormBase uiForm)
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
        #endregion
    }
}
