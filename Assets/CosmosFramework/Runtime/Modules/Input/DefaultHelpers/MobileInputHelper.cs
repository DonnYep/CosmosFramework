using UnityEngine;
using Cosmos.Input;
namespace Cosmos
{
    /// <summary>
    /// 移动端输入帮助体（旧输入管理器 + Touch）。
    /// <para>映射约定（与 StandardInputHelper 的虚拟名保持一致，业务代码不用分平台）：</para>
    /// <para>· 触摸点数 &gt; 0 → <see cref="InputButtonType._MouseLeft"/> 按住；</para>
    /// <para>· 触摸点数 &gt; 1 → <see cref="InputButtonType._MouseRight"/> 按住；</para>
    /// <para>· <see cref="InputAxisType._MouseX"/>/<see cref="_MouseY"/> → 单指拖动增量（已按屏幕尺寸归一化）；</para>
    /// <para>· 虚拟鼠标位置 → 第一根手指的位置。</para>
    /// <para>局限：这里没有虚拟摇杆。摇杆属于 UI 层（UGUI 摇杆把值写进 SetAxis 即可），
    /// 或者换用新输入系统/第三方插件。</para>
    /// </summary>
    public class MobileInputHelper : IInputHelper
    {
        IInputManager inputManager;
        Vector2 lastTouchPosition;
        bool hasLastTouchPosition;
        public void OnInitialization(IInputManager inputManager)
        {
            LegacyInputSafe.ReportUnavailable(nameof(MobileInputHelper));
            this.inputManager = inputManager;
            if (inputManager == null)
                return;
            inputManager.RegisterVirtualButton(InputButtonType._MouseLeft);
            inputManager.RegisterVirtualButton(InputButtonType._MouseRight);
            inputManager.RegisterVirtualButton(InputButtonType._MouseMiddle);
            inputManager.RegisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
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
            var touchCount = LegacyInputSafe.TouchCount;
            var hasTouch = touchCount > 0;
            //触摸作为"左键按住"，双指作为"右键按住"：电平推送，边沿由模块推导
            inputManager.SetButtonPressed(InputButtonType._MouseLeft, hasTouch);
            inputManager.SetButtonPressed(InputButtonType._MouseRight, touchCount > 1);
            inputManager.SetButtonPressed(InputButtonType._MouseMiddle, false);
            inputManager.SetButtonPressed(InputButtonType._MouseLeftDoubleClick, false);

            if (hasTouch)
            {
                var touch = LegacyInputSafe.GetTouch(0);
                var position = touch.position;
                //拖动增量归一化：不同分辨率下手感一致
                if (hasLastTouchPosition && touch.phase != TouchPhase.Began)
                {
                    var screenWidth = Mathf.Max(1, Screen.width);
                    var screenHeight = Mathf.Max(1, Screen.height);
                    inputManager.SetAxis(InputAxisType._MouseX, (position.x - lastTouchPosition.x) / screenWidth);
                    inputManager.SetAxis(InputAxisType._MouseY, (position.y - lastTouchPosition.y) / screenHeight);
                }
                else
                {
                    inputManager.SetAxis(InputAxisType._MouseX, 0f);
                    inputManager.SetAxis(InputAxisType._MouseY, 0f);
                }
                lastTouchPosition = position;
                hasLastTouchPosition = true;
                inputManager.SetVirtualMousePosition(position.x, position.y, 0f);
            }
            else
            {
                hasLastTouchPosition = false;
                inputManager.SetAxis(InputAxisType._MouseX, 0f);
                inputManager.SetAxis(InputAxisType._MouseY, 0f);
                inputManager.SetVirtualMousePosition(Vector3.zero);
            }
            //移动端没有滚轮/键盘方向：统一置零，避免沿用上一次的值
            inputManager.SetAxis(InputAxisType._MouseScrollWheel, 0f);
            inputManager.SetAxis(InputAxisType._Horizontal, 0f);
            inputManager.SetAxis(InputAxisType._Vertical, 0f);
            inputManager.SetAxis(InputAxisType._UpperLower, 0f);
        }
        public void OnTermination()
        {
            if (inputManager == null)
                return;
            inputManager.DeregisterVirtualButton(InputButtonType._MouseLeft);
            inputManager.DeregisterVirtualButton(InputButtonType._MouseRight);
            inputManager.DeregisterVirtualButton(InputButtonType._MouseMiddle);
            inputManager.DeregisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
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
