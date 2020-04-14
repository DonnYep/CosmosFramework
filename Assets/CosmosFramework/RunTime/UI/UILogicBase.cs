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
        Dictionary<string, List<UIBehaviour>> uiDict = new Dictionary<string, List<UIBehaviour>>();
        protected virtual void Awake()
        {
            RegisterUIComp<Button>();
            RegisterUIComp<Text>();
            RegisterUIComp<Slider>();
            RegisterUIComp<ScrollRect>();
            RegisterUIComp<Image>();
            RegisterUIComp<InputField>();
            OnInitialization();
        }
        protected abstract void OnInitialization();
        /// <summary>
        /// 获取UI组件
        /// </summary>
        protected T GetUIComp<T>(string name)
            where T:UIBehaviour
        {
            if (HasUIComp(name))
            {
                short listCount = (short)uiDict[name].Count;
                for (short i = 0; i <listCount ; i++)
                {
                    var result = uiDict[name][i] as T;
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
        /// <summary>
        /// 在Awake阶段注册UI组件到当前的字典中
        /// </summary>
        /// <typeparam name="T"></typeparam>
       protected void RegisterUIComp<T>()
            where T : UIBehaviour
        {
            T[] uiPanels = GetComponentsInChildren<T>();
            string panelName;
            short panelCount = (short)uiPanels.Length;
            for (short i = 0; i < panelCount; i++)
            {
                panelName = uiPanels[i].gameObject.name;
                if (uiDict.ContainsKey(panelName))
                {
                    uiDict[panelName].Add(uiPanels[i]);
                }
                else
                {
                    uiDict.Add(panelName, new List<UIBehaviour>() { uiPanels[i] });
                }
            }
        }
        /// <summary>
        /// 注销或移除某个panel下的T类型UI组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="panelName"></param>
        protected void DeregisterUIComp<T>(string panelName)
            where T:UIBehaviour
        {
            if (HasUIComp(panelName))
            {
                HashSet<T> removeSet = new HashSet<T>();
                for (int i = 0; i < uiDict[panelName].Count; i++)
                {
                    if(uiDict[panelName][i] is T)
                    {
                        removeSet.Add(uiDict[panelName][i] as T);
                    }
                }
                foreach (var rs in removeSet)
                {
                    uiDict[panelName].Remove(rs);
                }
                removeSet.Clear();
            }
            else
            {
                Utility.DebugLog("UIComp not registed>>" + panelName, MessageColor.fuchsia);
            }
        }
        /// <summary>
        /// 注销整个对应名称的panel
        /// </summary>
        /// <param name="panelName"></param>
        protected void DeregisterUIComp(string panelName)
        {
            if (HasUIComp(panelName))
                uiDict.Remove(panelName);
        }
        public bool HasUIComp(string name)
        {
            return uiDict.ContainsKey(name);
        }
        protected virtual void OnDestroy()
        {
            OnTermination();
            uiDict.Clear();
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