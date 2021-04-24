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
    public string EventKey { get { return eventKey; } }
    public string DispatcherName { get { return gameObject.name; } }
    LogicEventArgs<Slider> uch;
    [SerializeField]
    string message;
    IEventManager eventManager;
    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        uch =ReferencePool.Accquire<LogicEventArgs<Slider>>().SetData(slider);
        eventManager = GameManager.GetModule<IEventManager>();
    }
    Slider slider;
    public void DispatchEvent()
    {
        uch.Data.maxValue = slider.maxValue;
        uch.Data.value = slider.value;
        eventManager.DispatchEvent(eventKey, null, uch);
    }
    public void DeregisterEvent()
    {
        eventManager.DeregisterEvent(eventKey);
    }
    private void OnDestroy()
    {
        ReferencePool.Release(uch);
    }
}
