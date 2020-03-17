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
    UIImplementArgs<Slider> uch;
    public void RegisterEvent()
    {
        Facade.Instance.AddEventListener(eventKey, Handler);
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
    public void DeregisterEvent()
    {
        Facade.Instance.RemoveEventListener(eventKey, Handler);
    }
    public void DeregisterEventManager()
    {
        GameManager.Instance.DeregisterModule(CFModules.EVENT);
    }
    void Handler(object sender, GameEventArgs arg)
    {
        uch = arg as UIImplementArgs<Slider>;
        slider.maxValue = uch.Data.maxValue;
        slider.value = uch.Data.value;
        var dispatcher = Utility.ConvertToObject<GameObject>(sender);
    }

}
