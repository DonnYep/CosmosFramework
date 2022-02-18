using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Cosmos.UI
{
    /// <summary>
    /// 严格定义上，Panel是lable的容器。lable是作为组件存在，例如Text、Image等
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UIForm : MonoBehaviour, IUIForm
    {
        struct UILableInfo
        {
            public UILableInfo(Type uIType, UIBehaviour uIBehaviour)
            {
                UIType = uIType;
                UIBehaviour = uIBehaviour;
            }
            public Type UIType { get; private set; }
            public UIBehaviour UIBehaviour { get; private set; }
        }
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
                var peerComps = transform.GetComponentsInPeer<UIForm>(transform);
                Utility.Unity.SortCompsByAscending(peerComps, (comp) => comp.Priority);
            }
        }
        protected int priority = 100;
        public object Handle { get { return gameObject; } }
        public string UIFormName
        {
            get
            {
                if (string.IsNullOrEmpty(uiFormName))
                    uiFormName = GetComponent<UIForm>().GetType().Name;
                return uiFormName;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    uiFormName = value;
            }
        }
        public string UIGroupName { get; private set; }
        protected IUIManager UIManager { get { return CosmosEntry.UIManager; } }
        /// <summary>
        /// Name===[Lnk=== UILableInfo]；
        /// </summary>
        Dictionary<string, LinkedList<UILableInfo>> uiLableDict
            = new Dictionary<string, LinkedList<UILableInfo>>();
        string uiFormName;
        protected virtual void Awake() { }
        protected virtual void OnDestroy() { }
        protected bool HasLable<T>(string lableName)
        {
            if (uiLableDict.ContainsKey(lableName))
                return true;
            var go = transform.FindChildren(lableName);
            var comp = go.GetComponent<T>();
            if (comp != null)
                return true;
            return false;
        }
        /// <summary>
        /// 获取标签控件；
        /// 预先查询已经缓存的控件数据，若不存在，则动态获取缓存并返回；
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="lableName">控件名</param>
        /// <returns>控件组件</returns>
        protected T GetUILable<T>(string lableName)
            where T : UIBehaviour
        {
            T comp = null;
            if (uiLableDict.TryGetValue(lableName, out var lnk))
            {
                Type type = typeof(T);
                foreach (var info in lnk)
                {
                    if (info.UIType == type)
                    {
                        comp = info.UIBehaviour as T;
                        return comp;
                    }
                }
                if (comp == null)
                {
                    comp = gameObject.GetComponentInChildren<T>(lableName);
                    if (comp == null)
                        return null;
                    lnk.AddLast(new UILableInfo(typeof(T), comp));
                }
            }
            else
            {
                comp = gameObject.GetComponentInChildren<T>(lableName);
                if (comp == null)
                    return null;
                lnk = new LinkedList<UILableInfo>();
                lnk.AddLast(new UILableInfo(typeof(T), comp));
                uiLableDict.Add(lableName, lnk);
            }
            return comp;
        }
    }
}