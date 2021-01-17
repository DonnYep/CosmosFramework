using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Input;
using UnityEngine.UI;
using Cosmos;
using System;

public enum InputKey
{
    Vertical,
    Horizontal
}
public class AxisSubscriber : ControllerBase<AxisSubscriber>
{
    [SerializeField]
    InputKey key;
    int sliderOffset;
    int SliderOffset { get { return Utility.Converter.Int(slider.maxValue / 2); } }
    Slider slider;
    Text text;
    protected override void RefreshHandler()
    {
        switch (key)
        {
            case InputKey.Vertical:
                slider.value = Facade.GetAxis(InputAxisType._Vertical) * slider.maxValue + SliderOffset;
                break;
            case InputKey.Horizontal:
                slider.value = Facade.GetAxis(InputAxisType._Horizontal)* slider.maxValue + SliderOffset;
                break;
        }
        float textValue = slider.value - SliderOffset;
        text.text = Utility.Converter.Int(textValue).ToString();
    }
    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentsInChildren<Text>()[1];
        Facade.SetInputDevice(new StandardInputDevice());
    }
}
