using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.UI
{
    internal sealed class UIManager : Module<UIManager>
    {
        public static string MainUICanvasName { get; set; }
        GameObject mainUICanvas;
        public GameObject MainUICanvas { get { return mainUICanvas; } set { mainUICanvas = value; } }
        Dictionary<string, UILogicBase> uiPanelDict = new Dictionary<string, UILogicBase>();
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
                Facade.LoadResAysnc<GameObject>(path, go =>
                {
                    mainUICanvas = go;
                    mainUICanvas.name = "MainUICanvas";
                    mainUICanvas.transform.SetParent(ModuleMountObject.transform);
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
        public GameObject InitMainCanvas(string path,string name)
        {
            if (mainUICanvas != null)
                return mainUICanvas;
            else
            {
                Facade.LoadResAysnc<GameObject>(path, go =>
                {
                    mainUICanvas = go;
                    mainUICanvas.name = name;
                    mainUICanvas.transform.SetParent(ModuleMountObject.transform);
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
        public void LoadPanel<T>(string panelName, CFAction<T> callBack = null)
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
        /// 载入面板，若字典中已存在，则使用回调，并返回。若不存在，则异步加载且使用回调。
        /// 基于Resources
        /// </summary>
        /// <typeparam name="T"> UILogicBase</typeparam>
        /// <param name="panelName">相对完整路径</param>
        /// <param name="callBack">仅在载入时回调</param>
        public void ShowPanel<T>(string panelName, CFAction<T> callBack = null)
            where T : UILogicBase
        {
            if (HasPanel(panelName))
            {
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
        public void HidePanel(string panelName)
        {
            if (uiPanelDict.ContainsKey(panelName))
                uiPanelDict[panelName].HidePanel();
        }
        public void RemovePanel(string panelName)
        {
            if (uiPanelDict.ContainsKey(panelName))
            {
                var result = uiPanelDict[panelName].gameObject;
                GameManager.KillObject(result);
                uiPanelDict.Remove(panelName);
            }
            else
                Utility.DebugError("UIManager-->>" + "Panel :" + panelName + "  not register !");
        }
        public bool HasPanel(string panelName)
        {
            return uiPanelDict.ContainsKey(panelName);
        }
    }
}
