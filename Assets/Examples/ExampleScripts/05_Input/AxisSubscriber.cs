using UnityEngine;
using Cosmos.Input;
using UnityEngine.UI;
using Cosmos;

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

    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentsInChildren<Text>()[1];
        inputManager = CosmosEntry.InputManager;
    }
    void Update()
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
}
