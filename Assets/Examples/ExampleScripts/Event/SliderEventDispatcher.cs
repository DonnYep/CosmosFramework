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
    UIImplementArgs<Slider> uch;
    [SerializeField]
    string message;
    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        uch = new UIImplementArgs<Slider>(slider);
    }
    Slider slider;
    public void DispatchEvent()
    {
        uch.Data.maxValue = slider.maxValue;
        uch.Data.value = slider.value;
        Facade.Instance.DispatchEvent(eventKey, this, uch);
    }
    public void DeregisterEvent()
    {
        Facade.Instance.DeregisterEvent(eventKey);
    }
}
