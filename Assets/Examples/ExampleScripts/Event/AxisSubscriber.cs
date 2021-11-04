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
public class AxisSubscriber : MonoBehaviour
{
    [SerializeField]
    InputKey key;
    int SliderOffset { get { return (int)(slider.maxValue / 2); } }
    Slider slider;
    Text text;
    IInputManager inputManager;
    IController controller;

    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentsInChildren<Text>()[1];
        inputManager = CosmosEntry.InputManager;
        inputManager.SetInputHelper(new StandardInputHelper());
        controller = CosmosEntry.ControllerManager.CreateController("AxisSubscriber", this);
    }
    [TickRefresh]
    void TickRefresh()
    {
        switch (key)
        {
            case InputKey.Vertical:
                slider.value = inputManager.GetAxis(InputAxisType._Vertical) * slider.maxValue + SliderOffset;
                break;
            case InputKey.Horizontal:
                slider.value = inputManager.GetAxis(InputAxisType._Horizontal) * slider.maxValue + SliderOffset;
                break;
        }
        float textValue = slider.value - SliderOffset;
        text.text = ((int)textValue).ToString();
    }
    private void OnEnable()
    {
        if (controller != null)
            controller.Pause = false;
    }
    private void OnDisable()
    {
        if (controller != null)
            controller.Pause = true;
    }
}
