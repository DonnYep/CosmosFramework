using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cosmos.Event;
namespace Cosmos{
    public class EventSubscriber : MonoBehaviour {
        [SerializeField]
        string eventKey = "defaultEventKey";
        public string EventKey { get { return eventKey; } }
        public void RegisterEvent()
        {
            Facade.Instance.AddEventListener(eventKey, Handler);
        }
        private void Start()
        {
            RegisterEvent();
        }
        public void Deregister()
        {
            Facade.Instance.RemoveEventListener(eventKey, Handler);
        }
        public void DeregisterEventManager()
        {
            GameManager.Instance.DeregisterModule(CFModule.Event);
        }
      void Handler(object sender,GameEventArgs arg)
        {

        }
    }
}