using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;
public class SliderEventDispatcher : MonoBehaviour
{
    [SerializeField]
    string eventKey = "defaultEventKey";
    public string EventKey { get { return eventKey; } }
    public string DispatcherName { get { return gameObject.name; } }
    LogicEventArgs<Slider> uch;
    [SerializeField]
    string message;
    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        uch = Facade.SpawnReference<LogicEventArgs<Slider>>();
    }
    Slider slider;
    public void DispatchEvent()
    {
        uch.Data.maxValue = slider.maxValue;
        uch.Data.value = slider.value;
        Facade.DispatchEvent(eventKey, null, uch);
    }
    public void DeregisterEvent()
    {
        Facade.DeregisterEvent(eventKey);
    }
    private void OnDestroy()
    {
        Facade.DespawnReference(uch);
    }
}
