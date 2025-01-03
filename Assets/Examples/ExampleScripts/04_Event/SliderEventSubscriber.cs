using UnityEngine;
using UnityEngine.UI;
using Cosmos;
public class SliderEventSubscriber : MonoBehaviour
{
    string startText;
    Slider slider;
    Text text;

    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<Text>();
        startText = text.text;
        var btnDeregister = gameObject.GetComponentInChildren<Button>("Btn_Deregister");
        var btnRegister = gameObject.GetComponentInChildren<Button>("Btn_Register");
        btnDeregister.onClick.AddListener(RemoveEvent);
        btnRegister.onClick.AddListener(AddEvent);
    }
    void AddEvent()
    {
        CosmosEntry.EventManager.AddEvent<SliderEvent>(SliderEventHandler);
    }
    void RemoveEvent()
    {
        CosmosEntry.EventManager.RemoveEvent<SliderEvent>(SliderEventHandler);
    }
    void SliderEventHandler(SliderEvent arg)
    {
        slider.maxValue = arg.MaxValue;
        slider.value = arg.Value;
    }
}
