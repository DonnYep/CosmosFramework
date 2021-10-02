using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cosmos;
using Cosmos.Event;
public class SliderEventSubscriber : MonoBehaviour
{
    [SerializeField]
    string eventKey = "defaultEventKey";
    public string EventKey { get { return eventKey; } }
    LogicEventArgs<Slider> uch;
    public void RegisterEvent()
    {
        CosmosEntry.EventManager.AddListener(eventKey, Handler);
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
        var data = uch.GetData();
        slider.maxValue = data.maxValue;
        slider.value = data.value;
        var dispatcher = Utility.Converter.ConvertToObject<GameObject>(sender);
    }

}
