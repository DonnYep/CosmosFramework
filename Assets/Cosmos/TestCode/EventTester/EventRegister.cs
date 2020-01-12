using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos.UI;
using System;
using Cosmos.Event;
namespace Cosmos.Test{
    public class EventRegister: MonoBehaviour {
        [SerializeField]
        string key = "Unit";
        public void RegisterEvent()
        {
            //EventManager.Instance.AddListener(key, EventCenterTest);
            Facade.Instance.AddEventListener(key, EventCenterTest);
        }
        Slider slider;
        Text text;
        private void Start()
        {
            RegisterEvent();
            slider = GetComponentInChildren<Slider>();
            text = GetComponentInChildren<Text>();
            startText = text.text;
        }
        string startText;
        public void Deregister()
        {
            //EventManager.Instance.RemoveListener(key, EventCenterTest);
            Facade.Instance.RemoveEventListener(key, EventCenterTest);
        }
        public void DeregisterEventManager()
        {
            GameManager.Instance.DeregisterModule(CFModule.Event);
        }
        void EventCenterTest(object sender,GameEventArgs arg)
        {
            UIEventArgs uch = arg as UIEventArgs;
            slider.maxValue = uch.SliderMaxValue;
            slider.value = uch.SliderValue;
            var dispatcher = Utility.ConvertToObject<IEventDispatcher>(sender);
            text.text = startText + "\n-" +dispatcher.DispatcherName;
        }
    }
}