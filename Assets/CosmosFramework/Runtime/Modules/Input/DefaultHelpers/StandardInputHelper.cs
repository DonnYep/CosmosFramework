using UnityEngine;
using Cosmos.Input;
namespace Cosmos
{
    /// <summary>
    /// 标准输入设备（PC）。
    /// <para>按键以“电平”逐帧推送（SetButtonPressed），按下/抬起边沿由输入模块推导，
    /// 因此不再出现"边沿晚一帧"的问题；鼠标左键双击是模块内合成的两次边沿。</para>
    /// <para>仅适用于旧输入管理器；工程只启用新输入系统时请实现自己的 IInputHelper，
    /// 或把 Active Input Handling 设为 Both。</para>
    /// </summary>
    public class StandardInputHelper : IInputHelper
    {
        /// <summary>
        /// 鼠标左键双击时间间隔
        /// </summary>
        const float MouseLeftDoubleClickInterval = 0.3f;
        IInputManager inputManager;
        /// <summary>
        /// 鼠标左键单击计时器
        /// </summary>
        float mouseLeftClickTimer;
        /// <summary>
        /// UpperLower轴线值
        /// </summary>
        float upperLowerValue;
        public void OnInitialization(IInputManager inputManager)
        {
            LegacyInputSafe.ReportUnavailable(nameof(StandardInputHelper));
            this.inputManager = inputManager;
            if (inputManager == null)
                return;
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
            if (inputManager == null || !LegacyInputSafe.Available)
                return;
            inputManager.SetButtonPressed(InputButtonType._LeftShift, LegacyInputSafe.GetKey(KeyCode.LeftShift));
            inputManager.SetButtonPressed(InputButtonType._Escape, LegacyInputSafe.GetKey(KeyCode.Escape));
            inputManager.SetButtonPressed(InputButtonType._Space, LegacyInputSafe.GetKey(KeyCode.Space));
            inputManager.SetButtonPressed(InputButtonType._MouseLeft, LegacyInputSafe.GetMouseButton(0));
            inputManager.SetButtonPressed(InputButtonType._MouseRight, LegacyInputSafe.GetMouseButton(1));
            inputManager.SetButtonPressed(InputButtonType._MouseMiddle, LegacyInputSafe.GetMouseButton(2));

            //鼠标左键双击：合成一次"按下+抬起"，当帧即可被查询到
            if (LegacyInputSafe.GetMouseButtonDown(0))
            {
                if (mouseLeftClickTimer <= 0f)
                {
                    mouseLeftClickTimer = MouseLeftDoubleClickInterval;
                }
                else
                {
                    mouseLeftClickTimer = 0f;
                    inputManager.SetButtonDown(InputButtonType._MouseLeftDoubleClick);
                    inputManager.SetButtonUp(InputButtonType._MouseLeftDoubleClick);
                }
            }
            if (mouseLeftClickTimer > 0f)
                mouseLeftClickTimer -= Time.deltaTime;

            inputManager.SetAxis(InputAxisType._MouseX, LegacyInputSafe.GetAxis("Mouse X"));
            inputManager.SetAxis(InputAxisType._MouseY, LegacyInputSafe.GetAxis("Mouse Y"));
            inputManager.SetAxis(InputAxisType._MouseScrollWheel, LegacyInputSafe.GetAxis("Mouse ScrollWheel"));
            inputManager.SetAxis(InputAxisType._Horizontal, LegacyInputSafe.GetAxis("Horizontal"));
            inputManager.SetAxis(InputAxisType._Vertical, LegacyInputSafe.GetAxis("Vertical"));
            //上下方向键：按住的时长累积（供飞行/升降等场景使用）
            if (LegacyInputSafe.GetKey(KeyCode.UpArrow))
                upperLowerValue -= Time.deltaTime;
            else if (LegacyInputSafe.GetKey(KeyCode.DownArrow))
                upperLowerValue += Time.deltaTime;
            else
                upperLowerValue = 0f;
            inputManager.SetAxis(InputAxisType._UpperLower, upperLowerValue);
            inputManager.SetVirtualMousePosition(LegacyInputSafe.MousePosition);
        }
        public void OnTermination()
        {
            if (inputManager == null)
                return;
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
            inputManager = null;
        }
    }
}
