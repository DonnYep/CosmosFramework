﻿using UnityEngine;
namespace Cosmos.Input
{
    public interface IInputManager : IModuleManager
    {
        /// <summary>
        /// 虚拟轴线数量；
        /// </summary>
        int VirtualAxisCount { get; }
        /// <summary>
        /// 虚拟按钮数量；
        /// </summary>
        int VirtualButtonCount { get; }
        /// <summary>
        /// 鼠标位置；
        /// </summary>
        Vector3 MousePosition { get; }
        /// <summary>
        /// 是否启用虚拟输入；
        /// </summary>
        bool IsEnableInputDevice { get; set; }
        /// <summary>
        /// 设置自定义的输入帮助体；
        /// </summary>
        /// <param name="helper">自定义的输入帮助体</param>
        void SetInputHelper(IInputHelper helper);
        /// <summary>
        /// 虚拟轴线是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        bool IsExistVirtualAxis(string name);
        /// <summary>
        /// 虚拟按键是否存在
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否存在</returns>
        bool IsExistVirtualButton(string name);
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        void RegisterVirtualButton(string name);
        /// <summary>
        /// 注销虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        void DeregisterVirtualButton(string name);
        /// <summary>
        /// 注册虚拟按键
        /// </summary>
        /// <param name="name">按键名称</param>
        void RegisterVirtualAxis(string name);
        /// <summary>
        /// 注销虚拟轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        void DeregisterVirtualAxis(string name);


        /// <summary>
        /// 获得轴线
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>数值</returns>
        float GetAxis(string name);
        /// <summary>
        /// 未插值的输入 -1，0 ，1
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>数值</returns>
        float GetAxisRaw(string name);
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        bool GetButtonDown(string name);
        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        bool GetButton(string name);
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按键名称</param>
        /// <returns>是否按下</returns>
        bool GetButtonUp(string name);

        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        void SetButtonDown(string name);
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        void SetButtonUp(string name);
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        void SetVirtualMousePosition(Vector3 value);
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        void SetVirtualMousePosition(float x, float y, float z);
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisPositive(string name);
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisNegative(string name);
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisZero(string name);
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        void SetAxis(string name, float value);
    }
}
