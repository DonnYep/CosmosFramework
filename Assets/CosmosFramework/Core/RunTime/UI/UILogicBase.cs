using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Cosmos.UI
{
    public abstract class UILogicBase : MonoBehaviour
    {
        /// <summary>
        /// UI的映射表，名字作为主键，具有一个list容器
        /// </summary>
        Dictionary<string, List<UIBehaviour>> uiDict = new Dictionary<string, List<UIBehaviour>>();
        /// <summary>
        /// 是否自动注册获取当前节点下的UIBehaviour对象
        /// </summary>
        protected virtual bool AutoGetChildPanels { get; set; } = true;
        public virtual string UILogicName { get { return gameObject.name; } }
        /// <summary>
        /// 空虚函数
        /// </summary>
        public virtual void ShowPanel() { }
        /// <summary>
        /// 空虚函数
        /// </summary>
        public virtual void HidePanel() { }
        public bool HasPanel(string name)
        {
            return uiDict.ContainsKey(name);
        }
        protected virtual void Awake()
        {
            if (AutoGetChildPanels)
            {
                GetChildPanels<Button>();
                GetChildPanels<Text>();
                GetChildPanels<Slider>();
                GetChildPanels<ScrollRect>();
                GetChildPanels<Image>();
                GetChildPanels<InputField>();
            }
            OnInitialization();
        }
        protected abstract void OnInitialization();
        protected T GetUIPanel<T>(string name)
            where T : UIBehaviour
        {
            if (HasPanel(name))
            {
                short listCount = (short)uiDict[name].Count;
                for (short i = 0; i < listCount; i++)
                {
                    var result = uiDict[name][i] as T;
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取默认节点下的UIBehaviour；
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        protected void GetChildPanels<T>()
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
        /// 获取指定节点下的UI组件
        /// </summary>
        /// <typeparam name="T">UGUI目标类型</typeparam>
        /// <param name="root">目标节点</param>
        protected void GetChildPanels<T>(Transform root)
            where T : UIBehaviour
        {
            T[] uiPanels = root.GetComponentsInChildren<T>();
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
        /// 非空虚函数，此虚方法内部已实现SetPanelActive(true)
        /// </summary>
        protected virtual void ShowPanelHandler(object sender, GameEventArgs args) { SetPanelActive(true); }
        protected virtual void SetPanelActive(bool state) { gameObject.SetActive(state); }
    }
}