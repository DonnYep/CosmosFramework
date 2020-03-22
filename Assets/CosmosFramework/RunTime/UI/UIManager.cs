using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.UI
{
    public sealed class UIManager : Module<UIManager>
    {
        public static string MainUICanvasName { get; set; }
        GameObject mainUICanvas;
        public GameObject MainUICanvas { get { return mainUICanvas; } }
        Dictionary<string, UILogicBase> uiPanelMap = new Dictionary<string, UILogicBase>();
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
                Facade.Instance.LoadResAysnc<GameObject>(path, go =>
                {
                    mainUICanvas = go;
                    mainUICanvas.name = "MainUICanvas";
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
            Facade.Instance.LoadResAysnc<GameObject>(panelName, go =>
            {
                GameObject result = go;
                result.transform.SetParent(MainUICanvas.transform);
                (result.transform as RectTransform).ResetLocalTransform();
                T panel = result.GetComponent<T>();
                callBack?.Invoke(panel);
                uiPanelMap.Add(panelName, panel);
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
                callBack?.Invoke(uiPanelMap[panelName] as T);
                return;
            }
            Facade.Instance.LoadResAysnc<GameObject>(panelName, go =>
            {
                GameObject result = go;
                result.transform.SetParent(MainUICanvas.transform);
                (result.transform as RectTransform).ResetLocalTransform();
                T panel = result.GetComponent<T>();
                callBack?.Invoke(panel);
                uiPanelMap.Add(panelName, panel);
            });
        }
        public void HidePanel(string panelName)
        {
            if (uiPanelMap.ContainsKey(panelName))
                uiPanelMap[panelName].HidePanel();
        }
        public void RemovePanel(string panelName)
        {
            if (uiPanelMap.ContainsKey(panelName))
            {
                var result = uiPanelMap[panelName].gameObject;
                GameManager.KillObject(result);
                uiPanelMap.Remove(panelName);
            }
            else
                Utility.DebugError("UIManager\n" + "Panel :" + panelName + "  not register !");
        }
        public bool HasPanel(string panelName)
        {
            return uiPanelMap.ContainsKey(panelName);
        }
    }
}
