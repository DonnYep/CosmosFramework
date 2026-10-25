using UnityEngine;
using UnityEngine.UI;
using Cosmos;
using System;
public class SliderEventSubscriber : MonoBehaviour
{
    string startText;
    Slider slider;
    Text text;
    IDisposable subscription;

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
        //订阅返回句柄，重复点击不会重复注册
        if (subscription == null)
            subscription = CosmosEntry.EventManager.Subscribe<SliderEvent>(SliderEventHandler);
    }
    void RemoveEvent()
    {
        subscription?.Dispose();
        subscription = null;
    }
    private void OnDestroy()
    {
        RemoveEvent();
    }
    void SliderEventHandler(SliderEvent arg)
    {
        slider.maxValue = arg.MaxValue;
        slider.value = arg.Value;
    }
}
