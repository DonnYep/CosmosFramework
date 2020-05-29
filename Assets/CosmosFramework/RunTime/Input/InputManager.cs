using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Mono;
using Cosmos.Event;
using System;

namespace Cosmos.Input
{
    public enum InputButtonState : short
    {
        None = -1,
        Down = 0,
        Hold = 1,
        Up = 2
    }
    /// <summary>
    /// 输入管理器，主要为不同平台设备类型做适配，与之对应的有ControllerManager。
    /// </summary>
    internal sealed class InputManager : Module<InputManager>
    {
        //TODO 参考HT多平台适配
        #region Legacy
        short updateID;
        LogicEventArgs<InputVariable> inputHandler;
        public override void OnInitialization()
        {
            base.OnInitialization();
            inputHandler = Facade.SpawnReference<LogicEventArgs<InputVariable>>();
            inputHandler.SetData(inputVariable);
            _inputDevice.OnStart();
        }
        public override void OnTermination()
        {
            base.OnTermination();
            Facade.DespawnReference(inputHandler);
            _inputDevice.OnShutdown();
        }
        public override void OnRefresh()
        {
            if (IsEnableInputDevice)
                _inputDevice.OnRun();
            Run();
        }
        /// <summary>
        /// 临时输入模块
        /// </summary>
        void Run()
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
            if (Facade.HasEvent(InputEventCodeParams.INPUT_INPUTMODULE))
                Facade.DispatchEvent(InputEventCodeParams.INPUT_INPUTMODULE, this, inputHandler);
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
        #endregion
        public bool IsEnableInputDevice { get; set; } = true;
        InputVariable inputVariable = new InputVariable();
        VirtualInput inputModule;
       static InputDevice _inputDevice;

        public static  void SetInputDevice(InputDevice inputDevice)
        {
            _inputDevice = inputDevice;
        }

        /// <summary>
        /// 虚拟轴线是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualAxis(string name)
        {
            return inputModule.IsExistVirtualAxis(name);
        }
        /// <summary>
        /// 虚拟按键是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualButton(string name)
        {
            return inputModule.IsExistVirtualButton(name);
        }
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public void RegisterVirtualButton(string name)
        {
            inputModule.RegisterVirtualButton(name);
        }
        /// <summary>
        /// 注销虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public void DeregisterVirtualButton(string name)
        {
            inputModule.DeregisterVirtualButton(name);
        }
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        public void RegisterVirtualAxis(string name)
        {
            inputModule.RegisterVirtualAxis(name);
        }
        /// <summary>
        /// 注销虚拟轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        public void DeregisterVirtualAxis(string name)
        {
            inputModule.DeregisterVirtualAxis(name);
        }

        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition { get { return inputModule.MousePosition; } }
        /// <summary>
        /// 获得轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns></returns>
        public float GetAxis(string name)
        {
            return inputModule.GetAxis(name,false);
        }
        /// <summary>
        /// 未插值的输入 -1，0 ，1
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns></returns>
        public float GetAxisRaw(string name)
        {
            return inputModule.GetAxis(name, true);
        }
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string name)
        {
            return inputModule.GetButtonDown(name);
        }
        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public bool GetButton(string name)
        {
            return  inputModule.GetButton(name);
        }
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonUp(string name)
        {
            return inputModule.GetButtonUp(name);
        }

        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonDown(string name)
        {
            inputModule.SetButtonDown(name);
        }
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonUp(string name)
        {
            inputModule.SetButtonUp(name);
        }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        public void SetVirtualMousePosition(Vector3 value)
        {
            inputModule.SetVirtualMousePosition(value);
        }
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        public void SetVirtualMousePosition(float x,float y,float z)
        {
            inputModule.SetVirtualMousePosition(x, y, z);
        }
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisPositive(string name)
        {
            inputModule.SetAxisPositive(name);
        }
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisNegative(string name)
        {
            inputModule.SetAxisNegative(name);
        }
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisZero(string name)
        {
            inputModule.SetAxisZero(name);
        }
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        public void SetAxis(string name,float value)
        {
            inputModule.SetAxis(name, value);
        }
    }
}
