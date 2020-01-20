using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace Cosmos.Test
{
    public class SliderEventSubscriber : MonoBehaviour
    {
        [SerializeField]
        string eventKey = "defaultEventKey";
        public string EventKey { get { return eventKey; } }
        public void RegisterEvent()
        {
            Facade.Instance.AddEventListener(eventKey, EventCenterTest);
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
            Facade.Instance.RemoveEventListener(eventKey, EventCenterTest);
        }
        public void DeregisterEventManager()
        {
            GameManager.Instance.DeregisterModule(CFModule.Event);
        }
        void EventCenterTest(object sender, GameEventArgs arg)
        {
            UIEventArgs uch = arg as UIEventArgs;
            slider.maxValue = uch.SliderMaxValue;
            slider.value = uch.SliderValue;
            var dispatcher = Utility.ConvertToObject<GameObject>(sender);
            text.text = startText + "\n-" + dispatcher.name;
        }
    }
}