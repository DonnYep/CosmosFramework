using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cosmos.UI
{
    [Module]
    internal sealed class UIManager : Module<UIManager>
    {
        #region Properties
        internal static string MainUICanvasName { get; set; }
        GameObject mainUICanvas;
        internal GameObject MainUICanvas { get { return mainUICanvas; } set { mainUICanvas = value; } }
        Dictionary<string, UILogicBase> uiPanelDict = new Dictionary<string, UILogicBase>();
        #endregion

        #region Methods
        /// <summary>
        /// Resource文件夹相对路径
        /// 返回实例化的对象
        /// </summary>
        /// <param name="path">如UI\Canvas</param>
        internal GameObject InitMainCanvas(string path)
        {
            if (mainUICanvas != null)
                return mainUICanvas;
            else
            {
                Facade.LoadResAysnc<GameObject>(path, go =>
                {
                    mainUICanvas = go;
                    mainUICanvas.name = "MainUICanvas";
                    mainUICanvas.transform.SetParent(MountPoint.transform);
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
        internal GameObject InitMainCanvas(string path, string name)
        {
            if (mainUICanvas != null)
                return mainUICanvas;
            else
            {
                Facade.LoadResAysnc<GameObject>(path, go =>
                {
                    mainUICanvas = go;
                    mainUICanvas.name = name;
                    mainUICanvas.transform.SetParent(MountPoint.transform);
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
        internal void LoadPanel<T>(string panelName, Action<T> callBack = null)
            where T : UILogicBase
        {
            if (HasPanel(panelName))
                return;
            Facade.LoadResAysnc<GameObject>(panelName, go =>
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
        internal void LoadPanel<T>(Action<T> callBack = null)
    where T : UILogicBase
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            if (attribute == null)
                return;
            if (HasPanel(attribute.PrefabName))
                return;
            Facade.LoadResPrefabAsync<T>(go =>
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
        internal void ShowPanel<T>(string panelName, Action<T> callBack = null)
            where T : UILogicBase
        {
            if (HasPanel(panelName))
            {
                var panel = uiPanelDict[panelName] as T;
                panel.gameObject.SetActive(true);
                callBack?.Invoke(uiPanelDict[panelName] as T);
                return;
            }
            Facade.LoadResAysnc<GameObject>(panelName, go =>
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
        /// <typeparam name="T"></typeparam>
        /// <param name="callBack"></param>
        internal void ShowPanel<T>( Action<T> callBack = null)
    where T : UILogicBase
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            if (attribute == null)
                return;
            if (HasPanel(attribute.PrefabName))
            {
                callBack?.Invoke(uiPanelDict[attribute.PrefabName] as T);
                return;
            }
            Facade.LoadResPrefabAsync<T>(panel =>
            {
                panel.transform.SetParent(MainUICanvas.transform);
                (panel.transform as RectTransform).ResetLocalTransform();
                callBack?.Invoke(panel);
                uiPanelDict.Add(attribute.PrefabName, panel);
            });
        }
        internal void HidePanel(string panelName)
        {
            if (uiPanelDict.ContainsKey(panelName))
                uiPanelDict[panelName].HidePanel();
        }
        internal void RemovePanel(string panelName)
        {
            if (uiPanelDict.ContainsKey(panelName))
            {
                var result = uiPanelDict[panelName].gameObject;
                GameManager.KillObject(result);
                uiPanelDict.Remove(panelName);
            }
            else
                Utility.Debug.LogError("UIManager-->>" + "Panel :" + panelName + "  not register !");
        }
        internal void RemovePanel<T>()
            where T:UILogicBase
        {
            Type type = typeof(T);
            PrefabUnitAttribute attribute = type.GetCustomAttribute<PrefabUnitAttribute>();
            if (attribute == null)
                return;
            if (uiPanelDict.ContainsKey(attribute.PrefabName))
            {
                var result = uiPanelDict[attribute.PrefabName].gameObject;
                GameManager.KillObject(result);
                uiPanelDict.Remove(attribute.PrefabName);
            }
            else
                Utility.Debug.LogError("UIManager-->>" + "Panel :" + attribute.PrefabName + "  not register !");
        }
        internal bool HasPanel(string panelName)
        {
            return uiPanelDict.ContainsKey(panelName);
        }
        #endregion
    }
}
