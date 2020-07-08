using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Input;
using UnityEngine.UI;
using Cosmos;
public enum InputKey
{
    Vertical,
    Horizontal
}
public class AxisSubscriber : ControllerBase
{
    [SerializeField]
    InputKey key;
    int sliderOffset;
    int SliderOffset { get { return Utility.Converter.Int(slider.maxValue / 2); } }
    Slider slider;
    Text text;
    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentsInChildren<Text>()[1];
        Facade.SetInputDevice(new StandardInputDevice());
    }
    protected override void UpdateHandler()
    {
        switch (key)
        {
            case InputKey.Vertical:
                slider.value = Facade.GetAxis(InputAxisType.Vertical) * slider.maxValue + SliderOffset;
                break;
            case InputKey.Horizontal:
                slider.value = Facade.GetAxis(InputAxisType.Horizontal)* slider.maxValue + SliderOffset;
                break;
        }
        float textValue = slider.value - SliderOffset;
        text.text = Utility.Converter.Int(textValue).ToString();
    }
}
