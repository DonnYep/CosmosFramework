using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.UI
{
    public sealed class UIManager : Module<UIManager>
    {
        public static string MainUICanvasName { get; set; }
         GameObject mainUICanvas;
        public  GameObject MainUICanvas { get { if (mainUICanvas == null) Facade.Instance.LoadAysnc<GameObject>(ApplicationConst._MainUICanvansPath, go =>
           {
               mainUICanvas = GameObject.Instantiate(go);
               mainUICanvas.name = "MainUICanvans";
               mainUICanvas.transform.SetParent(ModuleMountObject.transform);
               GameObject.DontDestroyOnLoad(mainUICanvas);
           });
                return mainUICanvas;
            } }
        Dictionary<string, UILogicBase> uiPanelMap = new Dictionary<string, UILogicBase>();
        protected override void InitModule()
        {
            RegisterModule(CFModule.UI);
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
