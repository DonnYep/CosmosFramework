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
        internal static string MainUICanvasName { get; set; }
        GameObject mainUICanvas;
        public GameObject MainUICanvas { get { return mainUICanvas; } set { mainUICanvas = value; } }
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
        /// Resource文件夹相对路径
        /// 返回实例化的对象
        /// </summary>
        /// <param name="path">如UI\Canvas</param>
        public GameObject InitMainCanvas(string path)
        {
            if (mainUICanvas != null)
                return mainUICanvas;
            else
            {
                resourceManager.LoadResAysnc<GameObject>(path, go =>
                {
                    mainUICanvas = go;
                    mainUICanvas.name = "MainUICanvas";
                    var mountGo = GameManager.GetModuleMount<IUIManager>();
                    mainUICanvas.transform.SetParent(mountGo.transform);
                });
                return mainUICanvas;
            }
        }
        /// <summary>
        /// Resource文件夹相对路径
        /// 返回实例化的对象
        /// </summary>
        /// <param name="path">如UI\Canvas</param>
        /// <param name="name">生成后重命名的名称</param>
        public GameObject InitMainCanvas(string path, string name)
        {
            if (mainUICanvas != null)
                return mainUICanvas;
            else
            {
                resourceManager.LoadResAysnc<GameObject>(path, go =>
                {
                    mainUICanvas = go;
                    mainUICanvas.name = name;
                    var mountGo = GameManager.GetModuleMount<IUIManager>();
                    mainUICanvas.transform.SetParent(mountGo.transform);
                });
                return mainUICanvas;
            }
        }
        /// <summary>
        /// 载入面板，若字典中已存在，则返回且不使用回调。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        /// <typeparam name="T"> UILogicBase</typeparam>
        /// <param name="panelName">相对完整路径</param>
        /// <param name="callBack">仅在载入时回调</param>
        public void LoadPanel<T>(string panelName, Action<T> callBack = null)
            where T : UILogicBase
        {
            if (HasPanel(panelName))
                return;
            resourceManager.LoadResAysnc<GameObject>(panelName, go =>
            {
                GameObject result = go;
                result.transform.SetParent(MainUICanvas.transform);
                (result.transform as RectTransform).ResetLocalTransform();
                T panel = result.GetComponent<T>();
                callBack?.Invoke(panel);
                uiPanelDict.Add(panelName, panel);
            });
        }
        /// <summary>
        /// 通过特性UIResourceAttribute加载Panel
        /// </summary>
        /// <typeparam name="T">UILogicBase派生类</typeparam>
        /// <param name="callBack">加载完毕后的回调</param>
        public void LoadPanel<T>(Action<T> callBack = null)
    where T : UILogicBase
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            if (attribute == null)
                return;
            if (HasPanel(attribute.PrefabName))
                return;
            resourceManager.LoadResPrefabAsync<T>(go =>
            {
                go.transform.SetParent(MainUICanvas.transform);
                (go.transform as RectTransform).ResetLocalTransform();
                go.gameObject.name = attribute.PrefabName;
                callBack?.Invoke(go);
                uiPanelDict.Add(attribute.PrefabName, go);
            }
            );
        }
        /// <summary>
        /// 载入面板，若字典中已存在，则使用回调，并返回。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        /// <typeparam name="T"> UILogicBase</typeparam>
        /// <param name="panelName">相对完整路径</param>
        /// <param name="callBack">仅在载入时回调</param>
        public void ShowPanel<T>(string panelName, Action<T> callBack = null)
            where T : UILogicBase
        {
            if (HasPanel(panelName))
            {
                var panel = uiPanelDict[panelName] as T;
                panel.gameObject.SetActive(true);
                callBack?.Invoke(uiPanelDict[panelName] as T);
                return;
            }
            resourceManager.LoadResAysnc<GameObject>(panelName, go =>
            {
                GameObject result = go;
                result.transform.SetParent(MainUICanvas.transform);
                (result.transform as RectTransform).ResetLocalTransform();
                T panel = result.GetComponent<T>();
                callBack?.Invoke(panel);
                uiPanelDict.Add(panelName, panel);
            });
        }
        /// <summary>
        /// 通过特性PrefabUnitAttribute加载Panel
        /// </summary>
        /// <typeparam name="T">带有PrefabUnitAttribute特性的panel类</typeparam>
        /// <param name="callBack">加载成功的回调。若失败，则不执行</param>
        public void ShowPanel<T>(Action<T> callBack = null)
    where T : UILogicBase
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            if (attribute == null)
                return;
            resourceManager.LoadResPrefabAsync<T>(panel =>
            {
                panel.transform.SetParent(MainUICanvas.transform);
                (panel.transform as RectTransform).ResetLocalTransform();
                callBack?.Invoke(panel);
                uiPanelDict.Add(panel.gameObject.name, panel);
            });
        }
        public void HidePanel(string panelName)
        {
            uiPanelDict.TryGetValue(panelName, out var panel);
            panel?.HidePanel();
        }
        public void RemovePanel(string panelName)
        {
            if (uiPanelDict.TryRemove(panelName, out var panel))
                GameObject.Destroy(panel);
            else
                Utility.Debug.LogError("UIManager-->>" + "Panel :" + panelName + " not exist !");
        }
        public void RemovePanel<T>()
            where T : UILogicBase
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            if (attribute == null)
                return;
            if (uiPanelDict.TryRemove(attribute.PrefabName, out var panel))
                GameObject.Destroy(panel);
            else
                Utility.Debug.LogError("UIManager-->>" + "Panel :" + attribute.PrefabName + "  not exist !");
        }
        public bool HasPanel(string panelName)
        {
            return uiPanelDict.ContainsKey(panelName);
        }
        #endregion
    }
}
