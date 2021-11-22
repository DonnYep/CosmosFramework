using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.Event;
public class SliderEventDispatcher : MonoBehaviour
{
    [SerializeField]
    string eventKey = "defaultEventKey";
    LogicEventArgs<Slider> uch;
    IEventManager eventManager;
    Slider slider;
    Button btn_Deregister;
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        uch = ReferencePool.Acquire<LogicEventArgs<Slider>>().SetData(slider);
        eventManager = CosmosEntry.EventManager;
        slider.onValueChanged.AddListener(DispatchEvent);
        btn_Deregister = transform.Find("Btn_Deregister").GetComponent<Button>();
        btn_Deregister.onClick.AddListener(DeregisterEvent);
    }
    void DispatchEvent(float value)
    {
        var data = uch.GetData();
        data.maxValue = slider.maxValue;
        data.value = value;
        eventManager.DispatchEvent(eventKey, null, uch);
    }
    void DeregisterEvent()
    {
        eventManager.DeregisterEvent(eventKey);
    }
    void OnDestroy()
    {
        ReferencePool.Release(uch);
    }
}
