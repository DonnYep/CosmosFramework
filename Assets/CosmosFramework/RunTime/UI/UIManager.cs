using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.UI
{
    public sealed class UIManager : Module<UIManager>
    {
        public static string MainUICanvasName { get; set; }
         GameObject mainUICanvas;
        public  GameObject MainUICanvas { get { return mainUICanvas; } }
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
                Facade.Instance.LoadAysnc<GameObject>(path, go =>
                {
                    mainUICanvas = GameObject.Instantiate(go);
                    mainUICanvas.name = "MainUICanvas";
                    mainUICanvas.transform.SetParent(ModuleMountObject.transform);
                });
                return mainUICanvas;
            }
        }
        public void ShowPanel<T>(string panelName,CFAction<T> callBack=null)
            where T:UILogicBase
        {
            if (uiPanelMap.ContainsKey(panelName))
            {
                callBack?.Invoke(uiPanelMap[panelName] as T);
                return;
            }
            Facade.Instance.LoadAysnc<GameObject>("UI/" + panelName, go => 
            {
                GameObject result= GameObject.Instantiate(go);
                result.gameObject.name = panelName;
                result.transform.SetParent(MainUICanvas.transform);
                (result.transform as RectTransform).ResetLocalTransform();
                T panel = result.GetComponent<T>();
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
                Utility.DebugError("UIManager\n"+"Panel :" + panelName + "  not register !");
        }
        public bool HasPanel(string panelName)
        {
            return uiPanelMap.ContainsKey(panelName);
        }
    }
}
