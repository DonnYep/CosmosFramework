﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
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
            var assetInfo = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
            return OpenUIForm(assetInfo, uiType);
        }
        /// <summary>
        ///  通过AssetInfo加载UI对象（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public T OpenUIForm<T>(AssetInfo assetInfo)
            where T : UIFormBase
        {
            return OpenUIForm(assetInfo, typeof(T)) as T;
        }
        /// <summary>
        /// 通过AssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="uiType">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public UIFormBase OpenUIForm(AssetInfo assetInfo, Type uiType)
        {
            if (!uiFromBaseType.IsAssignableFrom(uiType))
            {
                throw new NotImplementedException($"Type:{uiType} is not inherit form UIFormBase");
            }
            var go = resourceManager.LoadPrefab(assetInfo, true);
            go?.transform.SetParent(UIRoot.transform);
            (go?.transform as RectTransform).ResetRectTransform();
            var comp = Utility.Unity.Add(uiType, go?.gameObject) as UIFormBase;
            if (!EqualUIFormPtrAdress(comp))
                RegisterUIForm(comp);
            return comp;
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        public Coroutine OpenUIFormAsync<T>(Action<T> loadDoneCallback = null)
    where T : UIFormBase
        {
            Type type = typeof(T);
            var attribute = type.GetCustomAttribute<UIAssetAttribute>();
            if (attribute == null)
            {
                throw new ArgumentNullException($"Type:{type} has no UIAssetAttribute");
            }
            var assetInfo = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
            return resourceManager.LoadPrefabAsync(assetInfo, panel =>
             {
                 panel.transform.SetParent(UIRoot.transform);
                 (panel.transform as RectTransform).ResetRectTransform();
                 var comp = Utility.Unity.Add<T>(panel);
                 loadDoneCallback?.Invoke(comp);
                 if (!EqualUIFormPtrAdress(comp))
                     RegisterUIForm(comp);
             }, null, true);
        }
        /// <summary>
        /// 通过AssetInfo加载UI对象（异步）；
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="uiType">目标组件的type类型</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns></returns>
        public Coroutine OpenUIFormAsync(AssetInfo assetInfo, Type uiType, Action<UIFormBase> loadDoneCallback = null)
        {
            if (!uiFromBaseType.IsAssignableFrom(uiType))
            {
                throw new ArgumentException($"Type:{uiType} is not inherit from UIFormBase");
            }
            return resourceManager.LoadPrefabAsync(assetInfo, go =>
             {
                 go.transform.SetParent(UIRoot.transform);
                 (go.transform as RectTransform).ResetRectTransform();
                 var comp = Utility.Unity.Add(uiType, go) as UIFormBase;
                 loadDoneCallback?.Invoke(comp);
                 if (!EqualUIFormPtrAdress(comp))
                     RegisterUIForm(comp);
             }, null, true);
        }
        /// <summary>
        /// 通过AssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        public Coroutine OpenUIFormAsync<T>(AssetInfo assetInfo, Action<T> loadDoneCallback = null)
            where T : UIFormBase
        {
            var type = typeof(T);
            if (!uiFromBaseType.IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type:{type} is not inherit from UIFormBase");
            }
            return resourceManager.LoadPrefabAsync(assetInfo, go =>
            {
                go.transform.SetParent(UIRoot.transform);
                (go.transform as RectTransform).ResetRectTransform();
                var comp = Utility.Unity.Add<T>(go);
                loadDoneCallback?.Invoke(comp);
                if (!EqualUIFormPtrAdress(comp))
                    RegisterUIForm(comp);
            }, null, true);
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <param name="type">带有UIAssetAttribute特性的panel类</param>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns></returns>
        public Coroutine OpenUIFormAsync(Type type, Action<UIFormBase> loadDoneCallback = null)
        {
            PrefabAssetAttribute attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (!uiFromBaseType.IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type:{type} is not inherit from UIFormBase");
            }
            if (attribute == null)
            {
                throw new ArgumentNullException($"Type:{type} has no UIAssetAttribute");
            }
            return resourceManager.LoadPrefabAsync(type, go =>
             {
                 go.transform.SetParent(UIRoot.transform);
                 (go.transform as RectTransform).ResetRectTransform();
                 var comp = Utility.Unity.Add(type, go, true) as UIFormBase;
                 loadDoneCallback?.Invoke(comp);
                 if (!EqualUIFormPtrAdress(comp))
                     RegisterUIForm(comp);
             }, null, true);
        }
        /// <summary>
        /// 隐藏UI，调用UI中的HidePanel方法；
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        public void HideUIForm(string uiName)
        {
            if (string.IsNullOrEmpty(uiName))
            {
                throw new ArgumentNullException("name is  invalid.");
            }
            if (uiDict.TryGetValue(uiName, out var uiForm))
                uiFormHelper?.HideUIForm(uiForm);
            else
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        public void HideUIForm(UIFormBase uiForm)
        {
            if (uiForm == null)
            {
                throw new ArgumentNullException("ui form is  invalid.");
            }
            uiFormHelper?.HideUIForm(uiForm);
        }
        public void ShowUIForm(UIFormBase uiForm)
        {
            if (uiForm == null)
            {
                throw new ArgumentNullException("ui form is  invalid.");
            }
            if (!HasUIForm(uiForm.UIFormName))
            {
                uiDict.TryAdd(uiForm.UIFormName, uiForm);
            }
            uiFormHelper?.ShowUIForm(uiForm);
        }
        public void ShowUIForm(string uiName)
        {
            if (string.IsNullOrEmpty(uiName))
            {
                throw new ArgumentNullException("name is  invalid.");
            }
            if (uiDict.TryGetValue(uiName, out var uiForm))
                uiFormHelper?.ShowUIForm(uiForm);
            else
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        /// <summary>
        /// 移除UI，但是不销毁
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        /// <param name="panel">移除后返回的panel</param>
        public void RemoveUIForm(string uiName, out UIFormBase uiForm)
        {
            var hasPanel = uiDict.TryRemove(uiName, out uiForm);
            if (!hasPanel)
            {
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
                return;
            }
            uiFormHelper?.RemoveUIForm(uiForm);
        }
        /// <summary>
        /// 移除UI，但是不销毁
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        public void RemoveUIForm(string uiName)
        {
            RemoveUIForm(uiName, out _);
        }
        /// <summary>
        /// 销毁UI
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        public void DestroyUlForm(string uiName)
        {
            if (uiDict.TryRemove(uiName, out var uiForm))
                uiFormHelper?.DestroyUIForm(uiForm);
            else
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        public void DestroyUlForm(UIFormBase uiForm)
        {
            if (uiForm == null)
                return;
            uiDict.TryRemove(uiForm.UIFormName, out _);
            uiFormHelper?.DestroyUIForm(uiForm);
        }
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        /// <returns>存在的结果</returns>
        public bool HasUIForm(string uiName)
        {
            return uiDict.ContainsKey(uiName);
        }
        public T PeekUIForm<T>(string uiName)
            where T : UIFormBase
        {
            return PeekUIForm(typeof(T), uiName) as T;
        }
        public UIFormBase PeekUIForm(Type uiType, string uiName)
        {
            if (!uiFromBaseType.IsAssignableFrom(uiType))
            {
                throw new ArgumentException($"Type:{uiType} is not inherit from UIFormBase");
            }
            uiDict.TryGetValue(uiName, out var uiForm);
            return uiForm;
        }
        /// <summary>
        /// 注册UI；
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiForm">UI对象</param>
        public void RegisterUIForm(UIFormBase uiForm)
        {
            if (uiForm == null)
                return;
            if (!HasUIForm(uiForm.UIFormName))
            {
                uiDict.Add(uiForm.UIFormName, uiForm);
            }
            else
            {
                Utility.Debug.LogError($"Form : {uiForm.name} has been registered , replaced by latest one !");
                RemoveUIForm(uiForm.UIFormName);
                uiDict.Add(uiForm.UIFormName, uiForm);
            }
            SortUIForm(uiForm);
        }
        /// <summary>
        /// 注销UI;
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiForm">UI对象</param>
        public void DeregisterUIForm(UIFormBase uiForm)
        {
            if (uiForm == null)
                return;
            if (HasUIForm(uiForm.UIFormName))
            {
                RemoveUIForm(uiForm.UIFormName);
            }
        }
        void SortUIForm(UIFormBase uiForm)
        {
            return;
            peerFormCache.Clear();
            var peers = Utility.Unity.Peers(uiForm.transform);
            var length = peers.Length;
            for (int i = 0; i < length; i++)
            {
                if (uiDict.TryGetValue(peers[i].name, out var uiFormComp))
                {
                    peerFormCache.Add(uiFormComp);
                }
            }
            if (peerFormCache.Count > 0)
                Utility.Unity.SortCompsByAscending(peerFormCache.ToArray(), (form) => form.Priority);
        }
        /// <summary>
        ///检测组件在内存中的地址是否相同 
        /// </summary>
        bool EqualUIFormPtrAdress(UIFormBase uiForm)
        {
            if (uiDict.TryGetValue(uiForm.name, out var compareForm))
            {
                if (uiForm == compareForm)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
