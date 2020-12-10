using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Cosmos.UI
{
    public abstract class UIFormBase : MonoBehaviour
    {
        protected IUIManager uiManager;
        /// <summary>
        /// UI的映射表，名字作为主键，具有一个list容器
        /// </summary>
        Dictionary<string, List<UIBehaviour>> uiDict;
        /// <summary>
        /// 是否自动注册获取当前节点下的UIBehaviour对象
        /// </summary>
        protected bool autoGetChildUIForm = true;
        public virtual string UIFormName { get { return gameObject.name; } }
        /// <summary>
        /// 被开启；
        /// 空虚函数；
        /// </summary>
        public virtual void ShowUIForm() { }
        /// <summary>
        /// 被关闭；
        /// 空虚函数；
        /// </summary>
        public virtual void HideUIForm() { }
        public bool HasUIForm(string name)
        {
            return uiDict.ContainsKey(name);
        }
        protected abstract void OnInitialization();
        protected T GetUIForm<T>(string name)
            where T : UIBehaviour
        {
            if (HasUIForm(name))
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
        protected void GetChildUIForm<T>()
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
        protected void GetChildUIForm<T>(Transform root)
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
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void OnTermination() { }
        protected virtual void SetPanelActive(bool state) { gameObject.SetActive(state); }
        void Awake()
        {
            uiDict = new Dictionary<string, List<UIBehaviour>>();
            uiManager = GameManager.GetModule<IUIManager>();
            if (autoGetChildUIForm)
            {
                GetChildUIForm<Button>();
                GetChildUIForm<Text>();
                GetChildUIForm<Slider>();
                GetChildUIForm<ScrollRect>();
                GetChildUIForm<Image>();
                GetChildUIForm<InputField>();
            }
            OnInitialization();
        }
        void OnDestroy()
        {
            OnTermination();
            uiDict.Clear();
        }
    }
}