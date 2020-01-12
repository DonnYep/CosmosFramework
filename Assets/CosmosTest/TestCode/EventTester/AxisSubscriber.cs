using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Event;
using Cosmos.Input;
using Cosmos.UI;
using UnityEngine.UI;
namespace Cosmos.Test
{
    public enum InputKey
    {
        Vertical,
        Horizontal
    }
    public class AxisSubscriber : MonoBehaviour
    {
        [SerializeField]
        InputKey key;
        int sliderOffset;
        int SliderOffset { get { return Utility.Int(slider.maxValue / 2); } }
        InputEventArgs inputHandler;
        Slider slider;
        Text text;
        private void Start()
        {
            //EventManager.Instance.AddListener(ApplicationConst._InputEventKey, InputHandler);
            Facade.Instance.AddEventListener(ApplicationConst._InputEventKey, InputHandler);
            slider = GetComponentInChildren<Slider>();
            text = GetComponentsInChildren<Text>()[1];
        }
        void InputHandler(object sender,GameEventArgs arg)
        {
            inputHandler = arg as InputEventArgs;
            switch (key)
            {
                case InputKey.Vertical:
                        slider.value = inputHandler.HorizVertAxis.y * slider.maxValue + SliderOffset;
                    break;
                case InputKey.Horizontal:
                        slider.value = inputHandler.HorizVertAxis.x * slider.maxValue + SliderOffset;
                    break;
            }
            float textValue = slider.value-SliderOffset;
            text.text = Utility.Int(textValue).ToString();
            if (inputHandler.MouseButtonLeft==InputButtonState.Down)
                Utility.DebugLog("mouseLeftDown",this);
            if (inputHandler.MouseButtonRight==InputButtonState.Down)
                Utility.DebugLog("mouseRightDown",this);
            if (inputHandler.Jump==InputButtonState.Down)
                Utility.DebugLog("jumpDown", this);
            if (inputHandler.MouseButtonMiddle==InputButtonState.Down)
                Utility.DebugLog("mouseMiddle", this);
            if (inputHandler.LeftShift==InputButtonState.Down)
                Utility.DebugLog("leftShiftDown", this);
            if (inputHandler.LeftShift == InputButtonState.Up)
                Utility.DebugLog("leftShiftUp", this);
            if (inputHandler.LeftShift == InputButtonState.Hold)
                Utility.DebugLog("leftShiftHold", this);
        }
        private void OnDestroy()
        {
            Facade.Instance.RemoveEventListener(ApplicationConst._InputEventKey, InputHandler);
        }
    }
}