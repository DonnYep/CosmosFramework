using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Mono;
using Cosmos.Event;
namespace Cosmos.Input { 
    public enum InputButtonState:int
    {
        None=-1,
        Down=0,
        Hold=1,
        Up=2
    }
    /// <summary>
    /// 输入管理器，主要为不同平台设备类型做适配，与之对应的有ControllerManager。
    /// </summary>
    public sealed class InputManager :Module<InputManager>
    {
        short updateID;
        InputEventArgs inputHandler = new InputEventArgs();
        VirtualInput inputModule;
        InputDevice inputDevice;
        protected override void InitModule()
        {
            RegisterModule(CFModules.INPUT);
        }
        public InputManager()
        {
            Facade.Instance.AddMonoListener(InputUpdate, UpdateType.Update, (id) => updateID = id);
        }
         ~InputManager()
        {
            Facade.Instance.RemoveMonoListener(InputUpdate, UpdateType.Update,updateID);
        }
        void InputUpdate()
        {
            //当前包含鼠标在屏幕的左右坐标，wasd输入、鼠标左中右按下，space空格 leftShift
            inputHandler.HorizVertAxis = new Vector2(UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis("Vertical"));
            inputHandler.MouseButtonLeft = ButtonPressState(KeyCode.Mouse0);
            inputHandler.MouseButtonRight = ButtonPressState(KeyCode.Mouse1);
            inputHandler.MouseButtonMiddle = ButtonPressState(KeyCode.Mouse2);
            inputHandler.Jump = ButtonPressState(KeyCode.Space);
            float mouseX = UnityEngine.Input.GetAxis("Mouse X");
            float mouseY = UnityEngine.Input.GetAxis("Mouse Y");
            inputHandler.MouseAxis = new Vector2(mouseX, mouseY);
            inputHandler.MouseButtonWheel = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            inputHandler.LeftShift = UnityEngine.Input.GetKey(KeyCode.LeftShift);
            inputHandler.Escape = UnityEngine.Input.GetKeyDown(KeyCode.Escape);
            if(EventManager.Instance.IsEventRegistered(InputEventParam.INPUTEVENT_INPUT))
                EventManager.Instance.DispatchEvent(InputEventParam.INPUTEVENT_INPUT, this, inputHandler);
        }
        /// <summary>
        /// 这段输入，当前PC有效，其他平台需要额外适配
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        InputButtonState ButtonPressState( KeyCode key)
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
