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
        IUIFormHelper uiFormHelper;
        IResourceManager resourceManager;
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            uiDict = new Dictionary<string, UIFormBase>();
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
        public T OpenUI<T>()
            where T : UIFormBase
        {
            return OpenUI(typeof(T)) as T;
        }
        /// <summary>
        ///  通过特性UIAssetAttribute加载Panel（同步）
        /// </summary>
        /// <param name="type">目标组件的type类型</param>
        /// <returns>生成的UI对象Comp</returns>
        public UIFormBase OpenUI(Type type)
        {
            if (typeof(UIFormBase).IsAssignableFrom(type))
            {
                throw new NotImplementedException($"Type:{type} is not inherit form UIFormBase");
            }
            var attribute = type.GetCustomAttribute<UIAssetAttribute>();
            if (attribute == null)
            {
                throw new NotImplementedException($"Type:{type} has no UIAssetAttribute");
            }
            var assetInfo = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
            return OpenUI(assetInfo, type);
        }
        /// <summary>
        ///  通过AssetInfo加载UI对象（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public T OpenUI<T>(AssetInfo assetInfo)
            where T : UIFormBase
        {
            return OpenUI(assetInfo, typeof(T)) as T;
        }
        /// <summary>
        /// 通过AssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="type">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public UIFormBase OpenUI(AssetInfo assetInfo, Type type)
        {
            if (typeof(UIFormBase).IsAssignableFrom(type))
            {
                throw new NotImplementedException($"Type:{type} is not inherit form UIFormBase");
            }
            var panel = resourceManager.LoadPrefab(assetInfo, true);
            panel?.transform.SetParent(UIRoot.transform);
            (panel?.transform as RectTransform).ResetLocalTransform();
            var comp = Utility.Unity.Add(type, panel?.gameObject, true) as UIFormBase;
            return comp;
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        public Coroutine OpenUIAsync<T>(Action<T> callback = null)
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
                 (panel.transform as RectTransform).ResetLocalTransform();
                 var comp = Utility.Unity.Add<T>(panel, false);
                 callback?.Invoke(comp);
                 uiDict.Add(comp.UIFormName, comp);
             },null,true);
        }
        /// <summary>
        /// 通过AssetInfo加载UI对象（异步）；
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="type">目标组件的type类型</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns></returns>
        public Coroutine OpenUIAsync(AssetInfo assetInfo, Type type, Action<UIFormBase> loadDoneCallback = null)
        {
            return resourceManager.LoadPrefabAsync(assetInfo, panel =>
             {
                 panel.transform.SetParent(UIRoot.transform);
                 (panel.transform as RectTransform).ResetLocalTransform();
                 var comp = Utility.Unity.Add(type, panel, true) as UIFormBase;
                 loadDoneCallback?.Invoke(comp);
                 uiDict.Add(comp.UIFormName, comp);
             });
        }
        /// <summary>
        /// 通过AssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        public Coroutine OpenUIAsync<T>(AssetInfo assetInfo, Action<T> loadDoneCallback = null)
            where T : UIFormBase
        {
            return resourceManager.LoadPrefabAsync(assetInfo, panel =>
            {
                panel.transform.SetParent(UIRoot.transform);
                (panel.transform as RectTransform).ResetLocalTransform();
                var comp = Utility.Unity.Add<T>(panel, true);
                loadDoneCallback?.Invoke(comp);
                uiDict.Add(comp.UIFormName, comp);
            });
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <param name="type">带有UIAssetAttribute特性的panel类</param>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns></returns>
        public Coroutine OpenUIAsync(Type type, Action<UIFormBase> loadDoneCallback = null)
        {
            PrefabAssetAttribute attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute == null)
            {
                throw new ArgumentNullException($"Type:{type} has no UIAssetAttribute");
            }
            return resourceManager.LoadPrefabAsync(type, panel =>
             {
                 panel.transform.SetParent(UIRoot.transform);
                 (panel.transform as RectTransform).ResetLocalTransform();
                 var comp = Utility.Unity.Add(type, panel, true) as UIFormBase;
                 loadDoneCallback?.Invoke(comp);
                 uiDict.Add(comp.UIFormName, comp);
             });
        }
        /// <summary>
        /// 隐藏UI，调用UI中的HidePanel方法；
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        public void HideUI(string uiName)
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
        public void HideUI(UIFormBase uiForm)
        {
            if (uiForm == null)
            {
                throw new ArgumentNullException("ui form is  invalid.");
            }
            uiFormHelper?.HideUIForm(uiForm);
        }
        public void ShowUI(UIFormBase uiForm)
        {
            if (uiForm == null)
            {
                throw new ArgumentNullException("ui form is  invalid.");
            }
            if (!HasUI(uiForm.UIFormName))
            {
                uiDict.TryAdd(uiForm.UIFormName, uiForm);
            }
            uiFormHelper?.ShowUIForm(uiForm);
        }
        public void ShowUI(string uiName)
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
        public void RemoveUI(string uiName, out UIFormBase uiForm)
        {
            var hasPanel = uiDict.TryRemove(uiName, out uiForm);
            uiFormHelper.RemoveUIForm(uiForm);
            if (!hasPanel)
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        /// <summary>
        /// 移除UI，但是不销毁
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        public void RemoveUI(string uiName)
        {
            RemoveUI(uiName, out _ );
        }
        /// <summary>
        /// 销毁UI
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        public void DestroyUl(string uiName)
        {
            if (uiDict.TryRemove(uiName, out var uiForm))
                uiFormHelper.DestroyUIForm(uiForm);
            else
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        public void DestroyUl(UIFormBase uiForm)
        {
            uiFormHelper.DestroyUIForm(uiForm);
        }
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UIFormBase",>
        /// UIFormBase.UIName
        /// </summary>
        /// <param name="uiName">UIFormBase.UIName</param>
        /// <returns>存在的结果</returns>
        public bool HasUI(string uiName)
        {
            return uiDict.ContainsKey(uiName);
        }
        #endregion
    }
}
