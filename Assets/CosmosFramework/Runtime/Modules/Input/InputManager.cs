using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cosmos.Event;
using System;

namespace Cosmos.Input
{
    /// <summary>
    /// 输入管理器，主要为不同平台设备类型做适配，与之对应的有ControllerManager。
    /// </summary>
    [Module]
    internal sealed class InputManager : Module, IInputManager
    {
        public override void OnInitialization()
        {
            base.OnInitialization();
            _inputDevice?.OnStart();
        }
        public override void OnTermination()
        {
            base.OnTermination();
            _inputDevice?.OnShutdown();
        }

        public bool IsEnableInputDevice { get; set; } = true;
        VirtualInput inputModule = new VirtualInput();
        static InputDevice _inputDevice;

        public void SetInputDevice(InputDevice inputDevice)
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
            return inputModule.GetAxis(name, false);
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
            return inputModule.GetButton(name);
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
        public void SetVirtualMousePosition(float x, float y, float z)
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
        public void SetAxis(string name, float value)
        {
            inputModule.SetAxis(name, value);
        }
        [TickRefresh]
        void OnRefresh()
        {
            if (IsPause)
                return;
            if (IsEnableInputDevice)
                _inputDevice?.OnRun();
        }
    }
}
