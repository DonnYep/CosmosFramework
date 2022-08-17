using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Cosmos.UI
{
    /// <summary>
    /// 严格定义上，Panel是lable的容器。lable是作为组件存在，例如Text、Image等
    /// 注意，UIForm 类是框架默认提供的UI类型，若需要使用自定义的面板类型，则可令其实现IUIForm接口，并配合DefaultUIFormAssetHelper来生成对应的资源；
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UGUIUIForm : MonoBehaviour, IUIForm
    {
        struct UILabelInfo
        {
            public UILabelInfo(Type uIType, UIBehaviour uIBehaviour)
            {
                UIType = uIType;
                UIBehaviour = uIBehaviour;
            }
            public Type UIType { get; private set; }
            public UIBehaviour UIBehaviour { get; private set; }
        }
        bool active;
        /// <inheritdoc/>
        public bool Active
        {
            get { return active; }
            set
            {
                if (active != value)
                {
                    active = value;
                    if (active)
                        OnOpen();
                    else
                        OnClose();
                }
            }
        }
        /// <inheritdoc/>
        public int Priority
        {
            get { return priority; }
            set
            {
                if (value > 10000)
                    priority = 10000;
                else if (value < 0)
                    priority = 0;
                var peerComps = gameObject.GetComponentsInPeer<UGUIUIForm>(transform);
                Utility.Unity.SortCompsByAscending(peerComps, (comp) => comp.Priority);
            }
        }
        protected int priority = 100;
        /// <inheritdoc/>
        public object Handle { get { return gameObject; } }
        string uiFormName;
        public string UIFormName
        {
            get
            {
                if (string.IsNullOrEmpty(uiFormName))
                    uiFormName = GetComponent<UGUIUIForm>().GetType().Name;
                return uiFormName;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    uiFormName = value;
            }
        }
        /// <inheritdoc/>
        public UIAssetInfo UIAssetInfo { get; set; }
        /// <summary>
        /// Name===[Lnk=== UILabelInfo]；
        /// </summary>
        Dictionary<string, LinkedList<UILabelInfo>> uiLabelDict
            = new Dictionary<string, LinkedList<UILabelInfo>>();

        ///<inheritdoc/>
        public virtual void OnInit()
        {
            Utility.Debug.LogInfo($"{UIFormName} OnInit");
        }
        ///<inheritdoc/>
        public virtual void OnOpen()
        {
            gameObject.SetActive(true);
        }
        ///<inheritdoc/>
        public virtual void OnClose()
        {
            gameObject.SetActive(false);
        }
        ///<inheritdoc/>
        public virtual void OnRelease()
        {
            Utility.Debug.LogInfo($"{UIFormName} OnRelease");
        }
        protected bool HasLabel<T>(string lableName)
        {
            if (uiLabelDict.ContainsKey(lableName))
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
        protected T GetUILabel<T>(string lableName)
            where T : UIBehaviour
        {
            T comp = null;
            if (uiLabelDict.TryGetValue(lableName, out var lnk))
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
                    lnk.AddLast(new UILabelInfo(typeof(T), comp));
                }
            }
            else
            {
                comp = gameObject.GetComponentInChildren<T>(lableName);
                if (comp == null)
                    return null;
                lnk = new LinkedList<UILabelInfo>();
                lnk.AddLast(new UILabelInfo(typeof(T), comp));
                uiLabelDict.Add(lableName, lnk);
            }
            return comp;
        }
    }
}