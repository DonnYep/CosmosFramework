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
    /// 输入管理器，继承自单例后，则全局管理输入，优化性能
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InputManager :Module<InputManager>
    {
        List<int> monoIDList = new List<int>();
        InputEventArgs inputHandler = new InputEventArgs();
        protected override void InitModule()
        {
            RegisterModule(CFModule.Input);
        }
        public InputManager()
        {
            MonoManager.Instance.AddListener(InputUpdate, UpdateType.Update, (id) => { if (!monoIDList.Contains(id)) monoIDList.Add(id); });
        }
        void InputUpdate()
        {
            //当前包含鼠标在屏幕的左右坐标，wasd输入、鼠标左中右按下，space空格 leftShift
            Vector2 axes = new Vector2(UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis("Vertical"));
            inputHandler.HorizVertAxis = axes;
            inputHandler.MouseButtonLeft = ButtonPressState(KeyCode.Mouse0);
            inputHandler.MouseButtonRight = ButtonPressState(KeyCode.Mouse1);
            inputHandler.MouseButtonMiddle = ButtonPressState(KeyCode.Mouse2);
            inputHandler.Jump = ButtonPressState(KeyCode.Space);
            float mouseX = UnityEngine.Input.GetAxis("Mouse X");
            float mouseY = UnityEngine.Input.GetAxis("Mouse Y");
            inputHandler.MouseAxis = new Vector2(mouseX, mouseY);
            inputHandler.MouseButtonWheel = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            inputHandler.LeftShift = ButtonPressState(KeyCode.LeftShift);
            if(EventManager.Instance.IsEventRegistered(ApplicationConst._InputEventKey))
                EventManager.Instance.DispatchEvent(ApplicationConst._InputEventKey, this, inputHandler);
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
    }
}
