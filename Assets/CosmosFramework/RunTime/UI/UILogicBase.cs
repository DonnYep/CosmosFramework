using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Cosmos.UI{
    public abstract  class UILogicBase:MonoBehaviour{
        /// <summary>
        /// UI的映射表，名字作为主键，具有一个list容器
        /// </summary>
        Dictionary<string, List<UIBehaviour>> uiMap = new Dictionary<string, List<UIBehaviour>>();
        protected virtual void Awake()
        {
            GetUIPanel<Button>();
            GetUIPanel<Text>();
            GetUIPanel<Slider>();
            GetUIPanel<ScrollRect>();
            GetUIPanel<Image>();
            GetUIPanel<InputField>();
            OnInitialization();
        }
        protected abstract void OnInitialization();
        protected T GetUIPanel<T>(string name)
            where T:UIBehaviour
        {
            if (HasPanel(name))
            {
                short listCount = (short)uiMap[name].Count;
                for (short i = 0; i <listCount ; i++)
                {
                    var result = uiMap[name][i] as T;
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
        void GetUIPanel<T>()
            where T : UIBehaviour
        {
            T[] uiPanels = GetComponentsInChildren<T>();
            string panelName;
            short panelCount = (short)uiPanels.Length;
            for (short i = 0; i < panelCount; i++)
            {
                panelName = uiPanels[i].gameObject.name;
                if (uiMap.ContainsKey(panelName))
                {
                    uiMap[panelName].Add(uiPanels[i]);
                }
                else
                {
                    uiMap.Add(panelName, new List<UIBehaviour>() { uiPanels[i] });
                }
            }
        }
        public bool HasPanel(string name)
        {
            return uiMap.ContainsKey(name);
        }
        protected virtual void OnDestroy()
        {
            OnTermination();
            uiMap.Clear();
        }
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void OnTermination() { }
        /// <summary>
        /// 空虚函数
        /// </summary>
        public virtual void ShowPanel() { }
        /// <summary>
        /// 空虚函数
        /// </summary>
        public virtual void HidePanel() { }
        protected virtual void ShowPanelHandler(object sender,GameEventArgs args) { SetPanelActive(true); }
        protected void DispatchUIEvent(string eventKey,object sender,GameEventArgs arg)
        {
            Facade.Instance.DispatchEvent(eventKey, sender, arg);
        }
        protected void DeregisterUIEvent(string eventKey)
        {
            Facade.Instance.DeregisterEvent(eventKey);
        }
        protected void AddUIEventListener(string eventKey,CFAction<object,GameEventArgs> handler)
        {
            Facade.Instance.AddEventListener(eventKey, handler);
        }
        protected void RemoveUIEventListener(string eventKey, CFAction<object, GameEventArgs> handler)
        {
            Facade.Instance.RemoveEventListener(eventKey, handler);
        }
        protected virtual void SetPanelActive(bool state) { gameObject.SetActive(state); }
    }
}