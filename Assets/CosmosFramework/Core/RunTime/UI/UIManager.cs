using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using Cosmos.Resource;
namespace Cosmos.UI
{
    [Module]
    internal sealed class UIManager : Module,IUIManager
    {
        #region Properties
        public GameObject UIRoot{ get; private set; }
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
            resourceManager = GameManager.GetModule<ResourceManager>();
        }
        /// <summary>
        /// 设置UI根节点
        /// </summary>
        /// <param name="uiRoot">传入的UIRoot</param>
        /// <param name="destroyOldOne">销毁旧的uiRoot对象</param>
        public void SetUIRoot(GameObject uiRoot,bool destroyOldOne=false)
        {
            if (destroyOldOne)
                GameObject.Destroy(UIRoot);
            var mountGo = GameManager.GetModuleMount<IUIManager>();
            uiRoot.transform.SetParent(mountGo.transform);
            UIRoot = uiRoot;
        }
        /// <summary>
        /// 通过特性UIAssetAttribute加载Panel
        /// </summary>
        /// <typeparam name="T">带有UIAssetAttribute特性的panel类</typeparam>
        /// <param name="callback">加载成功的回调。若失败，则不执行</param>
        public void OpenUIAsync<T>(Action<T> callback = null)
    where T : UILogicBase
        {
            Type type = typeof(T);
            var attribute = type.GetCustomAttribute<UIAssetAttribute>();
            if (attribute == null)
            {
                Utility.Debug.LogError($"Type:{type} has no UIAssetAttribute");
                return;
            }
            resourceManager.LoadPrefabAsync<T>(panel =>
            {
                panel.transform.SetParent(UIRoot.transform);
                (panel.transform as RectTransform).ResetLocalTransform();
                var comp = Utility.Unity.Add<T>(panel, true);
                callback?.Invoke(comp);
                uiPanelDict.Add(comp.UIName, comp);
            });
        }
        public void OpenUIAsync(Type type, Action<UILogicBase> callback = null)
        {
            PrefabAssetAttribute attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute == null)
            {
                Utility.Debug.LogError($"Type:{type} has no UIAssetAttribute");
                return;
            }
            resourceManager.LoadPrefabAsync(type, panel =>
             {
                 panel.transform.SetParent(UIRoot.transform);
                 (panel.transform as RectTransform).ResetLocalTransform();
                 var comp = Utility.Unity.Add(type, panel, true) as UILogicBase;
                 callback?.Invoke(comp);
                 uiPanelDict.Add(comp.UIName, comp);
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
        public void RemoveUI(string uiName,out UILogicBase panel)
        {
            var hasPanel = uiPanelDict.TryRemove(uiName, out panel);
            if(!hasPanel)
                Utility.Debug.LogError($"UIManager-->>Panel :{ uiName} is not exist !");
        }
        public void RemoveUI(string uiName )
        {
            var hasPanel = uiPanelDict.TryRemove(uiName,out _ );
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
        public bool HasUI(string panelName)
        {
            return uiPanelDict.ContainsKey(panelName);
        }
        #endregion
    }
}
