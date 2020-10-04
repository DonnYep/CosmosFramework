using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cosmos;
public class SliderEventSubscriber : MonoBehaviour
{
    [SerializeField]
    string eventKey = "defaultEventKey";
    public string EventKey { get { return eventKey; } }
    LogicEventArgs<Slider> uch;
    public void RegisterEvent()
    {
        Facade.AddEventListener(eventKey, Handler);
    }
    Slider slider;
    Text text;
    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<Text>();
        startText = text.text;
    }
    string startText;
    void Handler(object sender, GameEventArgs arg)
    {
        uch = arg as LogicEventArgs<Slider>;
        slider.maxValue = uch.Data.maxValue;
        slider.value = uch.Data.value;
        var dispatcher = Utility.Converter.ConvertToObject<GameObject>(sender);
    }

}
