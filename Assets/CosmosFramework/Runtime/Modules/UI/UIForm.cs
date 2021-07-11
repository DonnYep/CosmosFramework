using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Cosmos.UI
{
    [DisallowMultipleComponent]
    public abstract class UIForm: MonoBehaviour
    {
        /// <summary>
        /// 设置 UIForm层级顺序 ；
        /// 默认优先级为100，取值区间为[0,10000]；
        /// </summary>
        public int Priority
        {
            get { return priority; }
            set
            {
                if (value > 10000)
                    priority = 10000;
                else if (value < 0)
                    priority = 0;
                var peerComps = Utility.Unity.PeersComponet<UIForm>(transform);
                Utility.Unity.SortCompsByDescending(peerComps, (comp) => comp.Priority);
            }
        }
        protected int priority = 100;
        protected IUIManager UIManager { get { return GameManager.GetModule<IUIManager>(); } }
        /// <summary>
        /// UI的映射表，名字作为主键，具有一个list容器
        /// </summary>
        Dictionary<string, List<UIBehaviour>> uiPanelDict;
        /// <summary>
        /// 是否自动注册获取当前节点下的UIBehaviour对象
        /// </summary>
        protected bool autoGetUIComponents = true;
        public virtual string UIFormName
        {
            get
            {
                if (string.IsNullOrEmpty(uiFormName))
                    uiFormName = GetComponent<UIForm>().GetType().Name;
                return uiFormName;
            }
        }
        /// <summary>
        /// 所属组的名称；
        /// </summary>
        public string GroupName { get; protected set; }

        string uiFormName;

        public bool HasUIPanel(string name)
        {
            return uiPanelDict.ContainsKey(name);
        }
        /// <summary>
        /// 被开启；
        /// </summary>
        protected virtual void OnShow() { }
        /// <summary>
        /// 被关闭；
        /// </summary>
        protected virtual void OnHide() { }
        protected abstract void OnInitialization();
        protected T GetUIPanel<T>(string name)
            where T : UIBehaviour
        {
            if (HasUIPanel(name))
            {
                short listCount = (short)uiPanelDict[name].Count;
                for (short i = 0; i < listCount; i++)
                {
                    var result = uiPanelDict[name][i] as T;
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
        protected void GetUIComponents<T>()
            where T : UIBehaviour
        {
            T[] uiPanels = GetComponentsInChildren<T>();
            string panelName;
            short panelCount = (short)uiPanels.Length;
            for (short i = 0; i < panelCount; i++)
            {
                panelName = uiPanels[i].gameObject.name;
                if (uiPanelDict.ContainsKey(panelName))
                {
                    uiPanelDict[panelName].Add(uiPanels[i]);
                }
                else
                {
                    uiPanelDict.Add(panelName, new List<UIBehaviour>() { uiPanels[i] });
                }
            }
        }
        /// <summary>
        /// 获取指定节点下的UI组件
        /// </summary>
        /// <typeparam name="T">UGUI目标类型</typeparam>
        /// <param name="root">目标节点</param>
        protected void GetUIComponents<T>(Transform root)
            where T : UIBehaviour
        {
            T[] uiPanels = root.GetComponentsInChildren<T>();
            string panelName;
            short panelCount = (short)uiPanels.Length;
            for (short i = 0; i < panelCount; i++)
            {
                panelName = uiPanels[i].gameObject.name;
                if (uiPanelDict.ContainsKey(panelName))
                {
                    uiPanelDict[panelName].Add(uiPanels[i]);
                }
                else
                {
                    uiPanelDict.Add(panelName, new List<UIBehaviour>() { uiPanels[i] });
                }
            }
        }
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void OnTermination() { }
        void Awake()
        {
            uiPanelDict = new Dictionary<string, List<UIBehaviour>>();
            if (autoGetUIComponents)
            {
                GetUIComponents<Button>();
                GetUIComponents<Text>();
                GetUIComponents<Slider>();
                GetUIComponents<ScrollRect>();
                GetUIComponents<Image>();
                GetUIComponents<InputField>();
            }
            OnInitialization();
        }
        void OnDestroy()
        {
            OnTermination();
            uiPanelDict?.Clear();
        }
    }
}