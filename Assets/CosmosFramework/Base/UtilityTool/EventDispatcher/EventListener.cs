using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
namespace Cosmos{
    public class EventListener : MonoBehaviour
    {
        [SerializeField] string eventKey = "eventKey";
        public string EventKey { get { return eventKey; } }
        [Header("通过事件中心分发事件，这里使用unityAction注册事件")]
        [SerializeField] UnityEvent action;
        public UnityEvent Action { get { return action; } }
        private void Awake()
        {
            Register();
            Utility.DebugLog("RegisterEvent:\n"+ this.gameObject.name, this.gameObject);
        }
        private void OnDestroy()
        {
            Deregister();
            Utility.DebugLog("DeregisterEvent:\n" + this.gameObject.name);
        }
        void Handler(object sender,GameEventArgs arg)
        {
            action?.Invoke();
        }
        public void Deregister()
        {
            Facade.Instance.RemoveEventListener(eventKey, Handler);
        }
        public void Register()
        {
            Facade.Instance.AddEventListener(eventKey, Handler);
        }
    }
}