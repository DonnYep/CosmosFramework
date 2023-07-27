using UnityEngine;
using Cosmos.Input;
namespace Cosmos
{
    /// <summary>
    /// 标准输入设备，这里是PC
    /// </summary>
    public class StandardInputHelper : IInputHelper
    {
        IInputManager inputManager = CosmosEntry.InputManager;
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
        public void OnInitialization()
        {
            inputManager.RegisterVirtualButton(InputButtonType._MouseLeft);
            inputManager.RegisterVirtualButton(InputButtonType._MouseRight);
            inputManager.RegisterVirtualButton(InputButtonType._MouseMiddle);
            inputManager.RegisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
            inputManager.RegisterVirtualButton(InputButtonType._LeftShift);
            inputManager.RegisterVirtualButton(InputButtonType._Escape);
            inputManager.RegisterVirtualButton(InputButtonType._Space);
            inputManager.RegisterVirtualAxis(InputAxisType._MouseX);
            inputManager.RegisterVirtualAxis(InputAxisType._MouseY);
            inputManager.RegisterVirtualAxis(InputAxisType._MouseScrollWheel);
            inputManager.RegisterVirtualAxis(InputAxisType._Horizontal);
            inputManager.RegisterVirtualAxis(InputAxisType._Vertical);
            inputManager.RegisterVirtualAxis(InputAxisType._UpperLower);
        }
        public void OnRefresh()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
                inputManager.SetButtonDown(InputButtonType._LeftShift);
            else if (UnityEngine.Input.GetKeyUp(KeyCode.LeftShift))
                inputManager.SetButtonUp(InputButtonType._LeftShift);

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                inputManager.SetButtonDown(InputButtonType._Escape);
            else if (UnityEngine.Input.GetKeyUp(KeyCode.Escape))
                inputManager.SetButtonUp(InputButtonType._Escape);

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
                inputManager.SetButtonDown(InputButtonType._Space);
            else if (UnityEngine.Input.GetKeyUp(KeyCode.Space))
                inputManager.SetButtonUp(InputButtonType._Space);

            if (UnityEngine.Input.GetMouseButtonDown(0))
                inputManager.SetButtonDown(InputButtonType._MouseLeft);
            else if (UnityEngine.Input.GetMouseButtonUp(0))
                inputManager.SetButtonUp(InputButtonType._MouseLeft);

            if (UnityEngine.Input.GetMouseButtonDown(1))
                inputManager.SetButtonDown(InputButtonType._MouseRight);
            else if (UnityEngine.Input.GetMouseButtonUp(1))
                inputManager.SetButtonUp(InputButtonType._MouseRight);

            if (UnityEngine.Input.GetMouseButtonDown(2))
                inputManager.SetButtonDown(InputButtonType._MouseMiddle);
            else if (UnityEngine.Input.GetMouseButtonUp(2))
                inputManager.SetButtonUp(InputButtonType._MouseMiddle);

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (mouseLeftClickTimer <= 0)
                {
                    mouseLeftClickTimer = mouseLeftDoubleClickInterval;
                }
                else
                {
                    mouseLeftClickTimer = 0;
                    inputManager.SetButtonDown(InputButtonType._MouseLeftDoubleClick);
                    inputManager.SetButtonUp(InputButtonType._MouseLeftDoubleClick);
                }
            }
            if (mouseLeftClickTimer > 0)
            {
                mouseLeftClickTimer -= Time.deltaTime;
            }
            inputManager.SetAxis(InputAxisType._MouseX, UnityEngine.Input.GetAxis("Mouse X"));
            inputManager.SetAxis(InputAxisType._MouseY, UnityEngine.Input.GetAxis("Mouse Y"));
            inputManager.SetAxis(InputAxisType._MouseScrollWheel, UnityEngine.Input.GetAxis("Mouse ScrollWheel"));
            inputManager.SetAxis(InputAxisType._Horizontal, UnityEngine.Input.GetAxis("Horizontal"));
            inputManager.SetAxis(InputAxisType._Vertical, UnityEngine.Input.GetAxis("Vertical"));
            if (UnityEngine.Input.GetKey(KeyCode.UpArrow)) upperLowerValue -= Time.deltaTime;
            else if (UnityEngine.Input.GetKey(KeyCode.DownArrow)) upperLowerValue += Time.deltaTime;
            else upperLowerValue = 0;
            inputManager.SetVirtualMousePosition(UnityEngine.Input.mousePosition);
        }
        public void OnTermination()
        {
            inputManager.DeregisterVirtualButton(InputButtonType._MouseLeft);
            inputManager.DeregisterVirtualButton(InputButtonType._MouseRight);
            inputManager.DeregisterVirtualButton(InputButtonType._MouseMiddle);
            inputManager.DeregisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
            inputManager.DeregisterVirtualButton(InputButtonType._LeftShift);
            inputManager.DeregisterVirtualButton(InputButtonType._Escape);
            inputManager.DeregisterVirtualButton(InputButtonType._Space);
            inputManager.DeregisterVirtualAxis(InputAxisType._MouseX);
            inputManager.DeregisterVirtualAxis(InputAxisType._MouseY);
            inputManager.DeregisterVirtualAxis(InputAxisType._MouseScrollWheel);
            inputManager.DeregisterVirtualAxis(InputAxisType._Horizontal);
            inputManager.DeregisterVirtualAxis(InputAxisType._Vertical);
            inputManager.DeregisterVirtualAxis(InputAxisType._UpperLower);
        }
    }
}