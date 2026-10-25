using UnityEngine;

namespace Cosmos.Input
{
    /// <summary>
    /// 虚拟按钮。
    /// <para>状态以“电平”为准：帮助体每帧推送"是否按住"，按下/抬起边沿在这里推导并只在当帧为真，
    /// 因此不存在"边沿被推迟一帧"的老问题。</para>
    /// </summary>
    internal sealed class InputVirtualButton
    {
        public string Name { get; private set; }
        bool pressed;
        int pressedFrame = -1;
        int releasedFrame = -1;
        public InputVirtualButton(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 电平式设置：只有状态真正变化时才记录边沿帧
        /// </summary>
        /// <param name="value">是否按住</param>
        public void SetPressed(bool value)
        {
            if (pressed == value)
                return;
            pressed = value;
            if (value)
                pressedFrame = Time.frameCount;
            else
                releasedFrame = Time.frameCount;
        }
        /// <summary>
        /// 置为按下（等价 SetPressed(true)）
        /// </summary>
        public void Pressed()
        {
            SetPressed(true);
        }
        /// <summary>
        /// 置为抬起（等价 SetPressed(false)）；未按下时不会产生"抬起"边沿
        /// </summary>
        public void Released()
        {
            SetPressed(false);
        }
        /// <summary>
        /// 是否按住
        /// </summary>
        public bool GetButton
        {
            get { return pressed; }
        }
        /// <summary>
        /// 本帧是否刚按下
        /// </summary>
        public bool GetButtonDown
        {
            get { return pressedFrame >= 0 && pressedFrame == Time.frameCount; }
        }
        /// <summary>
        /// 本帧是否刚抬起
        /// </summary>
        public bool GetButtonUp
        {
            get { return releasedFrame >= 0 && releasedFrame == Time.frameCount; }
        }
        /// <summary>
        /// 清空状态（停止输入/切换帮助体时使用）
        /// </summary>
        public void Reset()
        {
            pressed = false;
            pressedFrame = -1;
            releasedFrame = -1;
        }
    }
}
