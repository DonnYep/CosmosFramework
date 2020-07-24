using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Input;
namespace Cosmos{
    /// <summary>
    /// 标准输入设备，这里是PC
    /// </summary>
    public class StandardInputDevice : InputDevice
    {
        /// <summary>
        /// 鼠标左键双击时间间隔
        /// </summary>
        readonly float mouseLeftDoubleClickInterval = 0.3f;
        /// <summary>
        /// 鼠标左键单击计时器
        /// </summary>
        float mouseLeftClickTimer = 0;
        /// <summary>
        /// UpperLower轴线值
        /// </summary>
        float upperLowerValue = 0;
        public override void OnStart()
        {
            Facade.RegisterVirtualButton(InputButtonType.MouseLeft);
            Facade.RegisterVirtualButton(InputButtonType.MouseRight);
            Facade.RegisterVirtualButton(InputButtonType.MouseMiddle);
            Facade.RegisterVirtualButton(InputButtonType.MouseLeftDoubleClick);
            Facade.RegisterVirtualButton(InputButtonType.LeftShift);
            Facade.RegisterVirtualButton(InputButtonType.Escape);
            Facade.RegisterVirtualAxis(InputAxisType.MouseX);
            Facade.RegisterVirtualAxis(InputAxisType.MouseY);
            Facade.RegisterVirtualAxis(InputAxisType.MouseScrollWheel);
            Facade.RegisterVirtualAxis(InputAxisType.Horizontal);
            Facade.RegisterVirtualAxis(InputAxisType.Vertical);
            Facade.RegisterVirtualAxis(InputAxisType.UpperLower);
        }
        public override void OnRun()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift)) Facade.SetButtonDown(InputButtonType.LeftShift);
            else if(UnityEngine.Input.GetKeyUp(KeyCode.LeftShift)) Facade.SetButtonUp(InputButtonType.LeftShift);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape)) Facade.SetButtonDown(InputButtonType.Escape);
            else if (UnityEngine.Input.GetKeyUp(KeyCode.Escape)) Facade.SetButtonUp(InputButtonType.Escape);
            if (UnityEngine.Input.GetMouseButtonDown(0)) Facade.SetButtonDown(InputButtonType.MouseLeft);
            else if (UnityEngine.Input.GetMouseButtonUp(0)) Facade.SetButtonUp(InputButtonType.MouseLeft);
            if (UnityEngine.Input.GetMouseButtonDown(1)) Facade.SetButtonDown(InputButtonType.MouseRight);
            else if (UnityEngine.Input.GetMouseButtonUp(1)) Facade.SetButtonUp(InputButtonType.MouseRight);
            if (UnityEngine.Input.GetMouseButtonDown(2)) Facade.SetButtonDown(InputButtonType.MouseMiddle);
            else if (UnityEngine.Input.GetMouseButtonUp(2)) Facade.SetButtonUp(InputButtonType.MouseMiddle);
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (mouseLeftClickTimer <= 0)
                {
                    mouseLeftClickTimer = mouseLeftDoubleClickInterval;
                }
                else
                {
                    mouseLeftClickTimer = 0;
                    Facade.SetButtonDown(InputButtonType.MouseLeftDoubleClick);
                    Facade.SetButtonUp(InputButtonType.MouseLeftDoubleClick);
                }
            }
            if (mouseLeftClickTimer > 0)
            {
                mouseLeftClickTimer -= Time.deltaTime;
            }
            Facade.SetAxis(InputAxisType.MouseX, UnityEngine.Input.GetAxis("Mouse X"));
            Facade.SetAxis(InputAxisType.MouseY, UnityEngine.Input.GetAxis("Mouse Y"));
            Facade.SetAxis(InputAxisType.MouseScrollWheel, UnityEngine.Input.GetAxis("Mouse ScrollWheel"));
            Facade.SetAxis(InputAxisType.Horizontal, UnityEngine.Input.GetAxis("Horizontal"));
            Facade.SetAxis(InputAxisType.Vertical, UnityEngine.Input.GetAxis("Vertical"));
            if (UnityEngine.Input.GetKey(KeyCode.UpArrow)) upperLowerValue -= Time.deltaTime;
            else if (UnityEngine.Input.GetKey(KeyCode.DownArrow)) upperLowerValue += Time.deltaTime;
            else upperLowerValue = 0;
            Facade.SetVirtualMousePosition(UnityEngine.Input.mousePosition);
        }
        public override void OnShutdown()
        {
            Facade.DeregisterVirtualButton(InputButtonType.MouseLeft);
            Facade.DeregisterVirtualButton(InputButtonType.MouseRight);
            Facade.DeregisterVirtualButton(InputButtonType.MouseMiddle);
            Facade.DeregisterVirtualButton(InputButtonType.MouseLeftDoubleClick);
            Facade.DeregisterVirtualButton(InputButtonType.LeftShift);
            Facade.DeregisterVirtualButton(InputButtonType.Escape);
            Facade.DeregisterVirtualAxis(InputAxisType.MouseX);
            Facade.DeregisterVirtualAxis(InputAxisType.MouseY);
            Facade.DeregisterVirtualAxis(InputAxisType.MouseScrollWheel);
            Facade.DeregisterVirtualAxis(InputAxisType.Horizontal);
            Facade.DeregisterVirtualAxis(InputAxisType.Vertical);
            Facade.DeregisterVirtualAxis(InputAxisType.UpperLower);
        }
    }
}