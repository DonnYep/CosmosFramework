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
            Facade.Instance.AddEventListener(InputEventParam.INPUT_INPUT, InputHandler);
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
            InputKeyDebugInfo();
        }
        private void OnDestroy()
        {
            Facade.Instance.RemoveEventListener(InputEventParam.INPUT_INPUT, InputHandler);
        }
        void InputKeyDebugInfo()
        {
            if (inputHandler.MouseButtonLeft == InputButtonState.Down)
                Utility.DebugLog("mouseLeftDown", this);
            if (inputHandler.MouseButtonRight == InputButtonState.Down)
                Utility.DebugLog("mouseRightDown", this);
            if (inputHandler.Jump == InputButtonState.Down)
                Utility.DebugLog("jumpDown", this);
            if (inputHandler.MouseButtonMiddle == InputButtonState.Down)
                Utility.DebugLog("mouseMiddle", this);
            if (inputHandler.LeftShift )
                Utility.DebugLog("leftShiftDown", this);
        }
    }
