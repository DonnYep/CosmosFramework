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
        int SliderOffset { get { return Utility.Converter.Int(slider.maxValue / 2); } }
    LogicEventArgs<InputVariable> inputHandler;
        Slider slider;
        Text text;
        private void Start()
        {
            Facade.Instance.AddEventListener(InputEventCodeParams.INPUT_INPUT, InputHandler);
            slider = GetComponentInChildren<Slider>();
            text = GetComponentsInChildren<Text>()[1];
        }
        void InputHandler(object sender,GameEventArgs arg)
        {
            inputHandler = arg as LogicEventArgs<InputVariable>;
            switch (key)
            {
                case InputKey.Vertical:
                        slider.value = inputHandler.Data.HorizVertAxis.y * slider.maxValue + SliderOffset;
                    break;
                case InputKey.Horizontal:
                        slider.value = inputHandler.Data.HorizVertAxis.x * slider.maxValue + SliderOffset;
                    break;
            }
            float textValue = slider.value-SliderOffset;
            text.text = Utility.Converter.Int(textValue).ToString();
            InputKeyDebugInfo();
        }
        private void OnDestroy()
        {
            Facade.Instance.RemoveEventListener(InputEventCodeParams.INPUT_INPUT, InputHandler);
        }
        void InputKeyDebugInfo()
        {
            if (inputHandler.Data.MouseButtonLeft == InputButtonState.Down)
                Utility.DebugLog("mouseLeftDown", this);
            if (inputHandler.Data.MouseButtonRight == InputButtonState.Down)
                Utility.DebugLog("mouseRightDown", this);
            if (inputHandler.Data.Jump == InputButtonState.Down)
                Utility.DebugLog("jumpDown", this);
            if (inputHandler.Data.MouseButtonMiddle == InputButtonState.Down)
                Utility.DebugLog("mouseMiddle", this);
            if (inputHandler.Data.LeftShift )
                Utility.DebugLog("leftShiftDown", this);
        }
    }
