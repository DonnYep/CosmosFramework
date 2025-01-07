using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using Cosmos.Event;
public class SliderEventDispatcher : MonoBehaviour
{
    SliderEvent sliderEvent;
    IEventManager eventManager;
    Slider slider;
    Button btn_Deregister;
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        sliderEvent = ReferencePool.Acquire<SliderEvent>();
        eventManager = CosmosEntry.EventManager;
        slider.onValueChanged.AddListener(DispatchEvent);
        btn_Deregister = transform.Find("Btn_Deregister").GetComponent<Button>();
        btn_Deregister.onClick.AddListener(DeregisterEvent);
    }
    void DispatchEvent(float value)
    {
        sliderEvent.MaxValue = slider.maxValue;
        sliderEvent.Value = value;
        eventManager.Dispatch(sliderEvent);
    }
    void DeregisterEvent()
    {
        eventManager.RemoveAllEvents<SliderEvent>();
    }
    void OnDestroy()
    {
        ReferencePool.Release(sliderEvent);
    }
}
