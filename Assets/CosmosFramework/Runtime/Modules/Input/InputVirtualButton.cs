using UnityEngine;

namespace Cosmos.Input
{
    internal sealed class InputVirtualButton
    {
        public string Name { get; private set; }
        int lastPressedFrame = -5;
        int releasedFrame = -5;
        bool pressed = false;
        public InputVirtualButton(string name)
        {
            Name = name;
        }
        public void Pressed()
        {
            if (pressed)
                return;
            pressed = true;
            lastPressedFrame = Time.frameCount;
        }
        public void Released()
        {
            pressed = false;
            releasedFrame = Time.frameCount;
        }
        public bool GetButton
        {
            get { return pressed; }
        }
        public bool GetButtonDown
        {
            get { return lastPressedFrame - Time.frameCount == -1; }
        }
        public bool GetButtonUp
        {
            get { return releasedFrame == Time.frameCount - 1; }
        }
    }
}
