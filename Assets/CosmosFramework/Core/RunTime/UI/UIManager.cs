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
        Dictionary<string, UILogicBase> uiPanelDict;
        IResourceManager resourceManager;
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            uiPanelDict = new Dictionary<string, UILogicBase>();
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
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（同步）；
        /// </summary>
        /// <typeparam name="T">目标组件的type类型</typeparam>
        /// <returns>生成的UI对象Comp</returns>
        public T OpenUI<T>()
            where T : UILogicBase
        {
            return OpenUI(typeof(T)) as T;
        }
        /// <summary>
        ///  通过特性UIAssetAttribute加载Panel（同步）
        /// </summary>
        /// <param name="type">目标组件的type类型</param>
        /// <returns>生成的UI对象Comp</returns>
        public UILogicBase OpenUI(Type type)
        {
            if (typeof(UILogicBase).IsAssignableFrom(type))
            {
                throw new NotImplementedException($"Type:{type} is not inherit form UILogicBase");
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
            where T :UILogicBase
        {
            return OpenUI(assetInfo, typeof(T)) as T;
        }
        /// <summary>
        /// 通过AssetInfo加载UI对象（同步）；
        /// </summary>
        /// <param name="assetInfo">目标组件的type类型</param>
        /// <param name="type">传入的assetInfo对象</param>
        /// <returns>生成的UI对象Comp</returns>
        public UILogicBase OpenUI(AssetInfo assetInfo, Type type)
        {
            if (typeof(UILogicBase).IsAssignableFrom(type))
            {
                throw new NotImplementedException($"Type:{type} is not inherit form UILogicBase");
            }
            var panel = resourceManager.LoadPrefab(assetInfo, true);
            panel?.transform.SetParent(UIRoot.transform);
            (panel?.transform as RectTransform).ResetLocalTransform();
            var comp = Utility.Unity.Add(type, panel?.gameObject, true) as UILogicBase;
            return comp;
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        public Coroutine OpenUIAsync<T>(Action<T> callback = null)
    where T : UILogicBase
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
                 var comp = Utility.Unity.Add<T>(panel, true);
                 callback?.Invoke(comp);
                 uiPanelDict.Add(comp.UIAssetName, comp);
             });
        }
        /// <summary>
        /// 通过AssetInfo加载UI对象（异步）；
        /// </summary>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="type">目标组件的type类型</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        /// <returns></returns>
        public Coroutine OpenUIAsync(AssetInfo assetInfo, Type type, Action<UILogicBase> loadDoneCallback = null)
        {
            return resourceManager.LoadPrefabAsync(assetInfo, panel =>
             {
                 panel.transform.SetParent(UIRoot.transform);
                 (panel.transform as RectTransform).ResetLocalTransform();
                 var comp = Utility.Unity.Add(type, panel, true) as UILogicBase;
                 loadDoneCallback?.Invoke(comp);
                 uiPanelDict.Add(comp.UIAssetName, comp);
             });
        }
        /// <summary>
        /// 通过AssetInfo加载UI对象
        /// </summary>
        /// <typeparam name="T">目标UI组件</typeparam>
        /// <param name="assetInfo">传入的assetInfo对象</param>
        /// <param name="loadDoneCallback">加载完成后的回调</param>
        public Coroutine OpenUIAsync<T>(AssetInfo assetInfo, Action<T> loadDoneCallback = null)
            where T : UILogicBase
        {
            return resourceManager.LoadPrefabAsync(assetInfo, panel =>
            {
                panel.transform.SetParent(UIRoot.transform);
                (panel.transform as RectTransform).ResetLocalTransform();
                var comp = Utility.Unity.Add<T>(panel, true);
                loadDoneCallback?.Invoke(comp);
                uiPanelDict.Add(comp.UIAssetName, comp);
            });
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel（异步）；
        /// </summary>
        /// <param name="type">带有UIAssetAttribute特性的panel类</param>
        /// <param name="loadDoneCallback">加载成功的回调。若失败，则不执行</param>
        /// <returns></returns>
        public Coroutine OpenUIAsync(Type type, Action<UILogicBase> loadDoneCallback = null)
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
                 var comp = Utility.Unity.Add(type, panel, true) as UILogicBase;
                 loadDoneCallback?.Invoke(comp);
                 uiPanelDict.Add(comp.UIAssetName, comp);
             });
        }
        /// <summary>
        /// 隐藏UI，调用UI中的HidePanel方法；
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        public void HideUI(string uiName)
        {
            if (uiPanelDict.TryRemove(uiName, out var panel))
                panel?.HidePanel();
            else
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        /// <summary>
        /// 移除UI，但是不销毁
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        /// <param name="panel">移除后返回的panel</param>
        public void RemoveUI(string uiName, out UILogicBase panel)
        {
            var hasPanel = uiPanelDict.TryRemove(uiName, out panel);
            if (!hasPanel)
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        /// <summary>
        /// 移除UI，但是不销毁
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        public void RemoveUI(string uiName)
        {
            var hasPanel = uiPanelDict.TryRemove(uiName, out _);
            if (!hasPanel)
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        /// <summary>
        /// 销毁UI
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        public void DestroyUl(string uiName)
        {
            if (uiPanelDict.TryRemove(uiName, out var panel))
                GameObject.Destroy(panel);
            else
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        /// <summary>
        /// 是否存在UI;
        /// <see cref=" UILogicBase",>
        /// UILogicBase.UIName
        /// </summary>
        /// <param name="uiName">UILogicBase.UIName</param>
        /// <returns>存在的结果</returns>
        public bool HasUI(string uiName)
        {
            return uiPanelDict.ContainsKey(uiName);
        }
        #endregion
    }
}
