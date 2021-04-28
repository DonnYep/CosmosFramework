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
            GameManager.GetModule<IInputManager>().RegisterVirtualButton(InputButtonType._MouseLeft);
           GameManager.GetModule<IInputManager>().RegisterVirtualButton(InputButtonType._MouseRight);
           GameManager.GetModule<IInputManager>().RegisterVirtualButton(InputButtonType._MouseMiddle);
           GameManager.GetModule<IInputManager>().RegisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
           GameManager.GetModule<IInputManager>().RegisterVirtualButton(InputButtonType._LeftShift);
           GameManager.GetModule<IInputManager>().RegisterVirtualButton(InputButtonType._Escape);
           GameManager.GetModule<IInputManager>().RegisterVirtualAxis(InputAxisType._MouseX);
           GameManager.GetModule<IInputManager>().RegisterVirtualAxis(InputAxisType._MouseY);
           GameManager.GetModule<IInputManager>().RegisterVirtualAxis(InputAxisType._MouseScrollWheel);
           GameManager.GetModule<IInputManager>().RegisterVirtualAxis(InputAxisType._Horizontal);
           GameManager.GetModule<IInputManager>().RegisterVirtualAxis(InputAxisType._Vertical);
           GameManager.GetModule<IInputManager>().RegisterVirtualAxis(InputAxisType._UpperLower);
        }
        public override void OnRun()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))GameManager.GetModule<IInputManager>().SetButtonDown(InputButtonType._LeftShift);
            else if(UnityEngine.Input.GetKeyUp(KeyCode.LeftShift))GameManager.GetModule<IInputManager>().SetButtonUp(InputButtonType._LeftShift);
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))GameManager.GetModule<IInputManager>().SetButtonDown(InputButtonType._Escape);
            else if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))GameManager.GetModule<IInputManager>().SetButtonUp(InputButtonType._Escape);
            if (UnityEngine.Input.GetMouseButtonDown(0))GameManager.GetModule<IInputManager>().SetButtonDown(InputButtonType._MouseLeft);
            else if (UnityEngine.Input.GetMouseButtonUp(0))GameManager.GetModule<IInputManager>().SetButtonUp(InputButtonType._MouseLeft);
            if (UnityEngine.Input.GetMouseButtonDown(1))GameManager.GetModule<IInputManager>().SetButtonDown(InputButtonType._MouseRight);
            else if (UnityEngine.Input.GetMouseButtonUp(1))GameManager.GetModule<IInputManager>().SetButtonUp(InputButtonType._MouseRight);
            if (UnityEngine.Input.GetMouseButtonDown(2))GameManager.GetModule<IInputManager>().SetButtonDown(InputButtonType._MouseMiddle);
            else if (UnityEngine.Input.GetMouseButtonUp(2))GameManager.GetModule<IInputManager>().SetButtonUp(InputButtonType._MouseMiddle);
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (mouseLeftClickTimer <= 0)
                {
                    mouseLeftClickTimer = mouseLeftDoubleClickInterval;
                }
                else
                {
                    mouseLeftClickTimer = 0;
                   GameManager.GetModule<IInputManager>().SetButtonDown(InputButtonType._MouseLeftDoubleClick);
                   GameManager.GetModule<IInputManager>().SetButtonUp(InputButtonType._MouseLeftDoubleClick);
                }
            }
            if (mouseLeftClickTimer > 0)
            {
                mouseLeftClickTimer -= Time.deltaTime;
            }
           GameManager.GetModule<IInputManager>().SetAxis(InputAxisType._MouseX, UnityEngine.Input.GetAxis("Mouse X"));
           GameManager.GetModule<IInputManager>().SetAxis(InputAxisType._MouseY, UnityEngine.Input.GetAxis("Mouse Y"));
           GameManager.GetModule<IInputManager>().SetAxis(InputAxisType._MouseScrollWheel, UnityEngine.Input.GetAxis("Mouse ScrollWheel"));
           GameManager.GetModule<IInputManager>().SetAxis(InputAxisType._Horizontal, UnityEngine.Input.GetAxis("Horizontal"));
           GameManager.GetModule<IInputManager>().SetAxis(InputAxisType._Vertical, UnityEngine.Input.GetAxis("Vertical"));
            if (UnityEngine.Input.GetKey(KeyCode.UpArrow)) upperLowerValue -= Time.deltaTime;
            else if (UnityEngine.Input.GetKey(KeyCode.DownArrow)) upperLowerValue += Time.deltaTime;
            else upperLowerValue = 0;
           GameManager.GetModule<IInputManager>().SetVirtualMousePosition(UnityEngine.Input.mousePosition);
        }
        public override void OnShutdown()
        {
           GameManager.GetModule<IInputManager>().DeregisterVirtualButton(InputButtonType._MouseLeft);
           GameManager.GetModule<IInputManager>().DeregisterVirtualButton(InputButtonType._MouseRight);
           GameManager.GetModule<IInputManager>().DeregisterVirtualButton(InputButtonType._MouseMiddle);
           GameManager.GetModule<IInputManager>().DeregisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
           GameManager.GetModule<IInputManager>().DeregisterVirtualButton(InputButtonType._LeftShift);
           GameManager.GetModule<IInputManager>().DeregisterVirtualButton(InputButtonType._Escape);
           GameManager.GetModule<IInputManager>().DeregisterVirtualAxis(InputAxisType._MouseX);
           GameManager.GetModule<IInputManager>().DeregisterVirtualAxis(InputAxisType._MouseY);
           GameManager.GetModule<IInputManager>().DeregisterVirtualAxis(InputAxisType._MouseScrollWheel);
           GameManager.GetModule<IInputManager>().DeregisterVirtualAxis(InputAxisType._Horizontal);
           GameManager.GetModule<IInputManager>().DeregisterVirtualAxis(InputAxisType._Vertical);
           GameManager.GetModule<IInputManager>().DeregisterVirtualAxis(InputAxisType._UpperLower);
        }
    }
}