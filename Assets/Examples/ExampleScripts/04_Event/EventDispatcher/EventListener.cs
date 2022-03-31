using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
namespace Cosmos{
    public class EventListener : MonoBehaviour
    {
        /// <summary>
        /// for editor
        /// </summary>
        public StringContent keyContentDataSet;
        public string selectedKeyContent;
        public int previousSelectedIndex;

        public string EventKey { get { return selectedKeyContent; } }
        [Header("通过事件中心分发事件，这里使用unityAction注册事件")]
        public UnityEvent actions;
        private void Awake()
        {
            Register();
            Utility.Debug.LogInfo("RegisterEvent:\n"+ this.gameObject.name, this.gameObject);
        }
        private void OnDestroy()
        {
            Deregister();
            Utility.Debug.LogInfo("DeregisterEvent:\n" + this.gameObject.name);
        }
        void Handler(object sender,GameEventArgs arg)
        {
            actions?.Invoke();
        }
        /// <summary>
        /// 注销事件，当事件中心的此类Key事件为空时，自动注销这个key。
        /// </summary>
        public void Deregister()
        {
           CosmosEntry.EventManager.RemoveListener(EventKey, Handler);
        }
        public void Register()
        {
            CosmosEntry.EventManager.AddListener(EventKey, Handler);
        }
    }
}