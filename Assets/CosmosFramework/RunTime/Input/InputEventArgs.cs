using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Input;
namespace Cosmos {
    /// <summary>
    /// 输入控制事件数据类
    /// </summary>
    public class InputEventArgs : GameEventArgs
    {
        public override void Reset() { }
        public Vector2 HorizVertAxis { get; set; }
        public Vector2 MouseAxis { get;set; }
        public float MouseButtonWheel { get; set; }
        public InputButtonState Jump { get; set; }
        public InputButtonState MouseButtonRight { get; set; }
        public InputButtonState MouseButtonMiddle { get; set; }
        public InputButtonState MouseButtonLeft { get; set; }
        public bool LeftShift { get; set; }
        public bool Escape { get; set; }
        public void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        public void UnlockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
        public void HideMouse()
        {
            if (MouseButtonLeft == 0 || MouseButtonMiddle == 0 || MouseButtonRight == 0)
                LockMouse();
            if (Escape)
                UnlockMouse();
        }
    }
}