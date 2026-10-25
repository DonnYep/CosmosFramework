using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Input
{
    /// <summary>
    /// 虚拟输入设备：按键/轴线的真实状态容器。
    /// <para>查询未注册的名称不再“静默自动注册”（那是拼写错误的温床），
    /// 而是报一次错并按中性值返回，方便尽早发现写错的按键名。</para>
    /// </summary>
    internal sealed class InputVirtualDevice
    {
        readonly Dictionary<string, InputVirtualAxis> virtualAxisDict = new Dictionary<string, InputVirtualAxis>();
        readonly Dictionary<string, InputVirtualButton> virtualButtonDict = new Dictionary<string, InputVirtualButton>();
        /// <summary>
        /// 已经报过错的名称，避免每帧刷日志
        /// </summary>
        readonly HashSet<string> reportedMissingNames = new HashSet<string>();
        Vector3 virtualMousePosition;
        public Vector3 MousePosition 
        { 
            get { return virtualMousePosition; } 
        }
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
            if (!virtualButtonDict.TryGetValue(name, out var button))
            {
                ReportMissingButton(name);
                return false;
            }
            return button.GetButton;
        }
        public bool GetButtonDown(string name)
        {
            if (!virtualButtonDict.TryGetValue(name, out var button))
            {
                ReportMissingButton(name);
                return false;
            }
            return button.GetButtonDown;
        }
        public bool GetButtonUp(string name)
        {
            if (!virtualButtonDict.TryGetValue(name, out var button))
            {
                ReportMissingButton(name);
                return false;
            }
            return button.GetButtonUp;
        }
        public float GetAxis(string name, bool raw)
        {
            if (!virtualAxisDict.TryGetValue(name, out var axis))
            {
                ReportMissingAxis(name);
                return 0f;
            }
            return raw ? axis.GetValueYaw : axis.GetValue;
        }
        public void SetButtonPressed(string name, bool pressed)
        {
            if (!virtualButtonDict.TryGetValue(name, out var button))
            {
                //写入端仍然自动注册：帮助体可能先写后注册，这里不能因为顺序问题丢状态
                button = new InputVirtualButton(name);
                virtualButtonDict.Add(name, button);
            }
            button.SetPressed(pressed);
        }
        public void SetButtonDown(string name)
        {
            if (!virtualButtonDict.TryGetValue(name, out var button))
            {
                button = new InputVirtualButton(name);
                virtualButtonDict.Add(name, button);
            }
            button.Pressed();
        }
        public void SetButtonUp(string name)
        {
            if (!virtualButtonDict.TryGetValue(name, out var button))
            {
                button = new InputVirtualButton(name);
                virtualButtonDict.Add(name, button);
            }
            button.Released();
        }
        public void SetAxisPositive(string name)
        {
            SetAxis(name, 1f);
        }
        public void SetAxisNegative(string name)
        {
            SetAxis(name, -1f);
        }
        public void SetAxisZero(string name)
        {
            SetAxis(name, 0f);
        }
        public void SetAxis(string name, float value)
        {
            if (!virtualAxisDict.TryGetValue(name, out var axis))
            {
                axis = new InputVirtualAxis(name);
                virtualAxisDict.Add(name, axis);
            }
            axis.Update(value);
        }
        public void SetVirtualMousePosition(float x, float y, float z)
        {
            virtualMousePosition.Set(x, y, z);
        }
        public void SetVirtualMousePosition(Vector3 value)
        {
            virtualMousePosition = value;
        }
        /// <summary>
        /// 清空所有按键与轴线的值（保留注册）
        /// </summary>
        public void ResetAll()
        {
            foreach (var button in virtualButtonDict.Values)
                button.Reset();
            foreach (var axis in virtualAxisDict.Values)
                axis.Reset();
            virtualMousePosition = Vector3.zero;
        }
        void ReportMissingButton(string name)
        {
            if (reportedMissingNames.Add("B:" + name))
                Utility.Debug.LogError($"virtual button is not registered : {name}（先用 RegisterVirtualButton 注册，或检查名称拼写）");
        }
        void ReportMissingAxis(string name)
        {
            if (reportedMissingNames.Add("A:" + name))
                Utility.Debug.LogError($"virtual axis is not registered : {name}（先用 RegisterVirtualAxis 注册，或检查名称拼写）");
        }
    }
}
