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
        public void SetMouseLock(bool state)
        {
            if (state)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                IsMouseLocked = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                IsMouseLocked = false;
            }
        }
        public bool IsMouseLocked { get; private set; }
    }
}