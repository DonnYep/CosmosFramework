using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Input{
    public sealed class VirtualInput
    {
        Dictionary<string, VirtualAxis> virtualAxes = new Dictionary<string, VirtualAxis>();
        Dictionary<string, VirtualButton> virtualButtons = new Dictionary<string, VirtualButton>();
        Vector3 virtualMousePosition;
        public bool IsExistVirtualAxis(string name)
        {
            return virtualAxes.ContainsKey(name);
        }
        public bool IsExistVirtualButton(string name)
        {
            return virtualButtons.ContainsKey(name);
        }
        public void RegisterVirtualAxis(string name)
        {
            if (virtualAxes.ContainsKey(name))
                Utility.Debug.LogError("virtual Aixs is allready register, axis name: " + name);
            else
                virtualAxes.Add(name, new VirtualAxis(name));
        }
        public void RegisterVirtualButton(string name)
        {
            if (virtualButtons.ContainsKey(name))
                Utility.Debug.LogError("virtual Button is allready register, button name: " + name);
            else
                virtualButtons.Add(name, new VirtualButton(name));
        }
        public void DeregisterVirtualAxis(string name)
        {
            if (virtualAxes.ContainsKey(name))
                virtualAxes.Remove(name);
        }
        public void DeregisterVirtualButton(string name)
        {
            if (virtualButtons.ContainsKey(name))
                virtualButtons.Remove(name);
        }
        public bool GetButton(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            return virtualButtons[name].GetButton;
        }
        public bool GetButtonDown(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            return virtualButtons[name].GetButtonDown;
        }
        public bool GetButtonUp(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            return virtualButtons[name].GetButtonUp;
        }
        public float GetAxis(string name,bool raw)
        {
            if (!IsExistVirtualAxis(name))
                RegisterVirtualAxis(name);
            return raw ? virtualAxes[name].GetValueYaw : virtualAxes[name].GetValue;
        }
        public void SetButtonDown(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            virtualButtons[name].Pressed();
        }
        public void SetButtonUp(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            virtualButtons[name].Released();
        }
        public void SetAxisPositive(string name)
        {
            if (!IsExistVirtualAxis(name)) 
                RegisterVirtualAxis(name);
            virtualAxes[name].Update(1);
        }
        public void SetAxisNegative(string name)
        {
            if (!IsExistVirtualAxis(name))
                RegisterVirtualAxis(name);
            virtualAxes[name].Update(-1);
        }
        public void SetAxisZero(string name)
        {
            if (!IsExistVirtualAxis(name))
                RegisterVirtualAxis(name);
            virtualAxes[name].Update(0);
        }
        public void SetAxis(string name,float value)
        {
            if (!IsExistVirtualAxis(name))
                RegisterVirtualAxis(name);
            virtualAxes[name].Update(value);
        }
        public void SetVirtualMousePosition(float x,float y,float z)
        {
            virtualMousePosition.Set(x, y, z);
        }
        public void SetVirtualMousePosition(Vector3 value)
        {
            virtualMousePosition = value;
        }
        public Vector3 MousePosition { get { return virtualMousePosition; } }
    }
    public sealed class VirtualButton
    {
        public string Name { get; private set; }
        int lastPressedFrame = -5;
        int releasedFrame = -5;
        bool pressed = false;
        public VirtualButton(string name)
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
        public bool GetButton { get { return pressed; } }
        public bool GetButtonDown { get { return (lastPressedFrame - Time.frameCount == -1); } }
        public bool GetButtonUp { get { return releasedFrame == Time.frameCount - 1; } }
    }
    public sealed class VirtualAxis
    {
        public string Name { get; private set; }
        float value;
        public VirtualAxis(string name)
        {
            Name = name;
        }
        public void Update(float value)
        {
            this.value = value;
        }
        public float GetValue { get { return value; } }
        public float GetValueYaw { get { if (value < 0) return -1; else if (value > 0) return 1; else return 0; } }
    }
}