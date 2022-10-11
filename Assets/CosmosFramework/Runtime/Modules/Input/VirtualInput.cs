using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Input
{
    internal sealed class VirtualInput
    {
        Dictionary<string, InputVirtualAxis> virtualAxisDict = new Dictionary<string, InputVirtualAxis>();
        Dictionary<string, InputVirtualButton> virtualButtonDict = new Dictionary<string, InputVirtualButton>();
        Vector3 virtualMousePosition;
        public int VirtualAxisCount
        {
            get { return virtualAxisDict.Count; }
        }
        public int VirtualButtonCount
        {
            get { return virtualButtonDict.Count; }
        }
        public bool IsExistVirtualAxis(string name)
        {
            return virtualAxisDict.ContainsKey(name);
        }
        public bool IsExistVirtualButton(string name)
        {
            return virtualButtonDict.ContainsKey(name);
        }
        public void RegisterVirtualAxis(string name)
        {
            if (virtualAxisDict.ContainsKey(name))
                Utility.Debug.LogError("virtual Aixs is allready register, axis name: " + name);
            else
                virtualAxisDict.Add(name, new InputVirtualAxis(name));
        }
        public void RegisterVirtualButton(string name)
        {
            if (virtualButtonDict.ContainsKey(name))
                Utility.Debug.LogError("virtual Button is allready register, button name: " + name);
            else
                virtualButtonDict.Add(name, new InputVirtualButton(name));
        }
        public void DeregisterVirtualAxis(string name)
        {
            if (virtualAxisDict.ContainsKey(name))
                virtualAxisDict.Remove(name);
        }
        public void DeregisterVirtualButton(string name)
        {
            if (virtualButtonDict.ContainsKey(name))
                virtualButtonDict.Remove(name);
        }
        public bool GetButton(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            return virtualButtonDict[name].GetButton;
        }
        public bool GetButtonDown(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            return virtualButtonDict[name].GetButtonDown;
        }
        public bool GetButtonUp(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            return virtualButtonDict[name].GetButtonUp;
        }
        public float GetAxis(string name, bool raw)
        {
            if (!IsExistVirtualAxis(name))
                RegisterVirtualAxis(name);
            return raw ? virtualAxisDict[name].GetValueYaw : virtualAxisDict[name].GetValue;
        }
        public void SetButtonDown(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            virtualButtonDict[name].Pressed();
        }
        public void SetButtonUp(string name)
        {
            if (!IsExistVirtualButton(name))
                RegisterVirtualButton(name);
            virtualButtonDict[name].Released();
        }
        public void SetAxisPositive(string name)
        {
            if (!IsExistVirtualAxis(name))
                RegisterVirtualAxis(name);
            virtualAxisDict[name].Update(1);
        }
        public void SetAxisNegative(string name)
        {
            if (!IsExistVirtualAxis(name))
                RegisterVirtualAxis(name);
            virtualAxisDict[name].Update(-1);
        }
        public void SetAxisZero(string name)
        {
            if (!IsExistVirtualAxis(name))
                RegisterVirtualAxis(name);
            virtualAxisDict[name].Update(0);
        }
        public void SetAxis(string name, float value)
        {
            if (!IsExistVirtualAxis(name))
                RegisterVirtualAxis(name);
            virtualAxisDict[name].Update(value);
        }
        public void SetVirtualMousePosition(float x, float y, float z)
        {
            virtualMousePosition.Set(x, y, z);
        }
        public void SetVirtualMousePosition(Vector3 value)
        {
            virtualMousePosition = value;
        }
        public Vector3 MousePosition { get { return virtualMousePosition; } }
    }
}