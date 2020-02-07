using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.UI
{
    public sealed class UIManager : Module<UIManager>
    {
        public static string MainUICanvasName { get; set; }
        static GameObject mainUICanvas;
        public static GameObject MainUICanvas { get { if (mainUICanvas == null) Facade.Instance.LoadAysnc<GameObject>(ApplicationConst._MainUICanvansPath, go =>
           {
               mainUICanvas = GameObject.Instantiate(go);
               mainUICanvas.name = ApplicationConst._MainUICanvansPath;
               GameObject.DontDestroyOnLoad(mainUICanvas);
           });
                return mainUICanvas;
            } }
        Dictionary<string, UILogicBase> uiPanelMap = new Dictionary<string, UILogicBase>();
        protected override void InitModule()
        {
            RegisterModule(CFModule.UI);
        }
        public void ShowPanel<T>(string panelName,CFAction<T> callBack)
            where T:UILogicBase
        {
            if (uiPanelMap.ContainsKey(panelName))
            {
                callBack?.Invoke(uiPanelMap[panelName] as T);
                return;
            }
            Facade.Instance.LoadAysnc<GameObject>("UI/" + panelName, go => 
            {
                var result= GameObject.Instantiate(go);
                result.gameObject.name = panelName;
                result.transform.SetParent(MainUICanvas.transform);
                (result.transform as RectTransform).ResetRectTransform();
                T panel = result.GetComponent<T>();
                uiPanelMap.Add(panelName, panel);
            });
        }
        public void HidePanel<T>(string panelName)
        {
            if (uiPanelMap.ContainsKey(panelName))
            {

            }
        }
    }
}
