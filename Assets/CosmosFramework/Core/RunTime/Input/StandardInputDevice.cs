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
            Facade.RegisterVirtualButton(InputButtonType._MouseLeft);
            Facade.RegisterVirtualButton(InputButtonType._MouseRight);
            Facade.RegisterVirtualButton(InputButtonType._MouseMiddle);
            Facade.RegisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
            Facade.RegisterVirtualButton(InputButtonType._LeftShift);
            Facade.RegisterVirtualButton(InputButtonType._Escape);
            Facade.RegisterVirtualAxis(InputAxisType._MouseX);
            Facade.RegisterVirtualAxis(InputAxisType._MouseY);
            Facade.RegisterVirtualAxis(InputAxisType._MouseScrollWheel);
            Facade.RegisterVirtualAxis(InputAxisType._Horizontal);
            Facade.RegisterVirtualAxis(InputAxisType._Vertical);
            Facade.RegisterVirtualAxis(InputAxisType._UpperLower);
        }
        public override void OnRun()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift)) Facade.SetButtonDown(InputButtonType._LeftShift);
            else if(UnityEngine.Input.GetKeyUp(KeyCode.LeftShift)) Facade.SetButtonUp(InputButtonType._LeftShift);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape)) Facade.SetButtonDown(InputButtonType._Escape);
            else if (UnityEngine.Input.GetKeyUp(KeyCode.Escape)) Facade.SetButtonUp(InputButtonType._Escape);
            if (UnityEngine.Input.GetMouseButtonDown(0)) Facade.SetButtonDown(InputButtonType._MouseLeft);
            else if (UnityEngine.Input.GetMouseButtonUp(0)) Facade.SetButtonUp(InputButtonType._MouseLeft);
            if (UnityEngine.Input.GetMouseButtonDown(1)) Facade.SetButtonDown(InputButtonType._MouseRight);
            else if (UnityEngine.Input.GetMouseButtonUp(1)) Facade.SetButtonUp(InputButtonType._MouseRight);
            if (UnityEngine.Input.GetMouseButtonDown(2)) Facade.SetButtonDown(InputButtonType._MouseMiddle);
            else if (UnityEngine.Input.GetMouseButtonUp(2)) Facade.SetButtonUp(InputButtonType._MouseMiddle);
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (mouseLeftClickTimer <= 0)
                {
                    mouseLeftClickTimer = mouseLeftDoubleClickInterval;
                }
                else
                {
                    mouseLeftClickTimer = 0;
                    Facade.SetButtonDown(InputButtonType._MouseLeftDoubleClick);
                    Facade.SetButtonUp(InputButtonType._MouseLeftDoubleClick);
                }
            }
            if (mouseLeftClickTimer > 0)
            {
                mouseLeftClickTimer -= Time.deltaTime;
            }
            Facade.SetAxis(InputAxisType._MouseX, UnityEngine.Input.GetAxis("Mouse X"));
            Facade.SetAxis(InputAxisType._MouseY, UnityEngine.Input.GetAxis("Mouse Y"));
            Facade.SetAxis(InputAxisType._MouseScrollWheel, UnityEngine.Input.GetAxis("Mouse ScrollWheel"));
            Facade.SetAxis(InputAxisType._Horizontal, UnityEngine.Input.GetAxis("Horizontal"));
            Facade.SetAxis(InputAxisType._Vertical, UnityEngine.Input.GetAxis("Vertical"));
            if (UnityEngine.Input.GetKey(KeyCode.UpArrow)) upperLowerValue -= Time.deltaTime;
            else if (UnityEngine.Input.GetKey(KeyCode.DownArrow)) upperLowerValue += Time.deltaTime;
            else upperLowerValue = 0;
            Facade.SetVirtualMousePosition(UnityEngine.Input.mousePosition);
        }
        public override void OnShutdown()
        {
            Facade.DeregisterVirtualButton(InputButtonType._MouseLeft);
            Facade.DeregisterVirtualButton(InputButtonType._MouseRight);
            Facade.DeregisterVirtualButton(InputButtonType._MouseMiddle);
            Facade.DeregisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
            Facade.DeregisterVirtualButton(InputButtonType._LeftShift);
            Facade.DeregisterVirtualButton(InputButtonType._Escape);
            Facade.DeregisterVirtualAxis(InputAxisType._MouseX);
            Facade.DeregisterVirtualAxis(InputAxisType._MouseY);
            Facade.DeregisterVirtualAxis(InputAxisType._MouseScrollWheel);
            Facade.DeregisterVirtualAxis(InputAxisType._Horizontal);
            Facade.DeregisterVirtualAxis(InputAxisType._Vertical);
            Facade.DeregisterVirtualAxis(InputAxisType._UpperLower);
        }
    }
}