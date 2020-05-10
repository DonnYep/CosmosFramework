using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Mono;
using Cosmos.Event;
namespace Cosmos.Input
{
    public enum InputButtonState : int
    {
        None = -1,
        Down = 0,
        Hold = 1,
        Up = 2
    }
    /// <summary>
    /// 输入管理器，主要为不同平台设备类型做适配，与之对应的有ControllerManager。
    /// </summary>
    public sealed class InputManager : Module<InputManager>
    {
        short updateID;
        LogicEventArgs<InputVariable> inputHandler = new LogicEventArgs<InputVariable>();
        InputVariable inputVariable = new InputVariable();
        VirtualInput inputModule;
        InputDevice inputDevice;
        public InputManager()
        {
            inputHandler.SetData(inputVariable);
            Facade.Instance.AddMonoListener(InputUpdate, UpdateType.Update, (id) => updateID = id);
        }
        ~InputManager()
        {
            Facade.Instance.RemoveMonoListener(InputUpdate, UpdateType.Update, updateID);
        }
        void InputUpdate()
        {
            //当前包含鼠标在屏幕的左右坐标，wasd输入、鼠标左中右按下，space空格 leftShift
            inputHandler.Data.HorizVertAxis = new Vector2(UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis("Vertical"));
            inputHandler.Data.MouseButtonLeft = ButtonPressState(KeyCode.Mouse0);
            inputHandler.Data.MouseButtonRight = ButtonPressState(KeyCode.Mouse1);
            inputHandler.Data.MouseButtonMiddle = ButtonPressState(KeyCode.Mouse2);
            inputHandler.Data.Jump = ButtonPressState(KeyCode.Space);
            float mouseX = UnityEngine.Input.GetAxis("Mouse X");
            float mouseY = UnityEngine.Input.GetAxis("Mouse Y");
            inputHandler.Data.MouseAxis = new Vector2(mouseX, mouseY);
            inputHandler.Data.MouseButtonWheel = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            inputHandler.Data.LeftShift = UnityEngine.Input.GetKey(KeyCode.LeftShift);
            inputHandler.Data.Escape = UnityEngine.Input.GetKeyDown(KeyCode.Escape);
            if (Facade.Instance.HasEvent(InputEventCodeParams.INPUT_INPUTMODULE))
                Facade.Instance.DispatchEvent(InputEventCodeParams.INPUT_INPUTMODULE, this, inputHandler);
        }
        /// <summary>
        /// 这段输入，当前PC有效，其他平台需要额外适配
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        InputButtonState ButtonPressState(KeyCode key)
        {
            if (UnityEngine.Input.GetKeyDown(key))
                return InputButtonState.Down;
            else if (UnityEngine.Input.GetKeyUp(key))
                return InputButtonState.Up;
            else if (UnityEngine.Input.GetKey(key))
                return InputButtonState.Hold;
            else
                return InputButtonState.None;
        }
        public void InitInputManager() { }
    }
}
