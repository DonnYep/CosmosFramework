using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Input
{
    //================================================
    /*
     * 1、输入模块，适配不同平台的按键输入事件。
     *
     * 2、模块只保留与后端无关的三件事：虚拟按键/轴线、输入闸门、边沿判定；
     *    具体按键映射由 IInputHelper 提供（旧 Input Manager、新 Input System、第三方插件都可接入）。
     *
     * 3、输入闸门（Push/PopInputBlock）供"UI 打开屏蔽游戏输入""过场屏蔽全部输入"使用：
     *    屏蔽期间查询返回中性值，但帮助体仍在刷新原始状态，因此解除屏蔽后按住的键依然是按住。
     */
    //================================================
    [Module(Priority = ModuleConstant.PRIORITY_INPUT)]
    internal sealed class InputManager : Module, IInputManager
    {
        bool isEnableInputDevice = true;
        InputVirtualDevice inputVirtualDevice;
        IInputHelper inputHelper;
        /// <summary>
        /// 输入闸门持有者（按压入顺序，允许嵌套）
        /// </summary>
        readonly List<object> inputBlocks = new List<object>();
        ///<inheritdoc/>
        public int VirtualAxisCount
        {
            get { return inputVirtualDevice.VirtualAxisCount; }
        }
        ///<inheritdoc/>
        public int VirtualButtonCount
        {
            get { return inputVirtualDevice.VirtualButtonCount; }
        }
        ///<inheritdoc/>
        public Vector3 MousePosition
        {
            get { return inputVirtualDevice.MousePosition; }
        }
        ///<inheritdoc/>
        public bool IsEnableInputDevice
        {
            get { return isEnableInputDevice; }
            set
            {
                if (isEnableInputDevice == value)
                    return;
                isEnableInputDevice = value;
                if (!value)
                {
                    //停止输入后必须清状态，否则按住不放的键会一直"卡在按下"
                    inputVirtualDevice.ResetAll();
                }
            }
        }
        ///<inheritdoc/>
        public bool IsInputBlocked { get { return inputBlocks.Count > 0; } }
        ///<inheritdoc/>
        public int InputBlockCount { get { return inputBlocks.Count; } }
        ///<inheritdoc/>
        public void SetInputHelper(IInputHelper helper)
        {
            if (inputHelper != null)
            {
                inputHelper.OnTermination();
                inputHelper = null;
            }
            //换帮助体前清空状态，避免旧帮助体遗留的按键/轴线被新帮助体"继承"
            inputVirtualDevice.ResetAll();
            inputHelper = helper;
            inputHelper?.OnInitialization(this);
        }
        ///<inheritdoc/>
        public void PushInputBlock(object owner)
        {
            if (owner == null)
                return;
            if (inputBlocks.Contains(owner))
                return;
            inputBlocks.Add(owner);
        }
        ///<inheritdoc/>
        public bool PopInputBlock(object owner)
        {
            if (owner == null)
                return false;
            return inputBlocks.Remove(owner);
        }
        ///<inheritdoc/>
        public void ClearInputBlocks()
        {
            inputBlocks.Clear();
        }
        ///<inheritdoc/>
        public void ResetAllVirtualStates()
        {
            inputVirtualDevice.ResetAll();
        }
        ///<inheritdoc/>
        public bool IsExistVirtualAxis(string name)
        {
            return inputVirtualDevice.IsExistVirtualAxis(name);
        }
        ///<inheritdoc/>
        public bool IsExistVirtualButton(string name)
        {
            return inputVirtualDevice.IsExistVirtualButton(name);
        }
        ///<inheritdoc/>
        public void RegisterVirtualButton(string name)
        {
            inputVirtualDevice.RegisterVirtualButton(name);
        }
        ///<inheritdoc/>
        public void DeregisterVirtualButton(string name)
        {
            inputVirtualDevice.DeregisterVirtualButton(name);
        }
        ///<inheritdoc/>
        public void RegisterVirtualAxis(string name)
        {
            inputVirtualDevice.RegisterVirtualAxis(name);
        }
        ///<inheritdoc/>
        public void DeregisterVirtualAxis(string name)
        {
            inputVirtualDevice.DeregisterVirtualAxis(name);
        }

        ///<inheritdoc/>
        public float GetAxis(string name)
        {
            if (IsInputBlocked)
                return 0f;
            return inputVirtualDevice.GetAxis(name, false);
        }
        ///<inheritdoc/>
        public float GetAxisRaw(string name)
        {
            if (IsInputBlocked)
                return 0f;
            return inputVirtualDevice.GetAxis(name, true);
        }
        ///<inheritdoc/>
        public bool GetButtonDown(string name)
        {
            if (IsInputBlocked)
                return false;
            return inputVirtualDevice.GetButtonDown(name);
        }
        ///<inheritdoc/>
        public bool GetButton(string name)
        {
            if (IsInputBlocked)
                return false;
            return inputVirtualDevice.GetButton(name);
        }
        ///<inheritdoc/>
        public bool GetButtonUp(string name)
        {
            if (IsInputBlocked)
                return false;
            return inputVirtualDevice.GetButtonUp(name);
        }
        ///<inheritdoc/>
        public void SetButtonPressed(string name, bool pressed)
        {
            inputVirtualDevice.SetButtonPressed(name, pressed);
        }
        ///<inheritdoc/>
        public void SetButtonDown(string name)
        {
            inputVirtualDevice.SetButtonDown(name);
        }
        ///<inheritdoc/>
        public void SetButtonUp(string name)
        {
            inputVirtualDevice.SetButtonUp(name);
        }
        ///<inheritdoc/>
        public void SetVirtualMousePosition(Vector3 value)
        {
            inputVirtualDevice.SetVirtualMousePosition(value);
        }
        ///<inheritdoc/>
        public void SetVirtualMousePosition(float x, float y, float z)
        {
            inputVirtualDevice.SetVirtualMousePosition(x, y, z);
        }
        ///<inheritdoc/>
        public void SetAxisPositive(string name)
        {
            inputVirtualDevice.SetAxisPositive(name);
        }
        ///<inheritdoc/>
        public void SetAxisNegative(string name)
        {
            inputVirtualDevice.SetAxisNegative(name);
        }
        ///<inheritdoc/>
        public void SetAxisZero(string name)
        {
            inputVirtualDevice.SetAxisZero(name);
        }
        ///<inheritdoc/>
        public void SetAxis(string name, float value)
        {
            inputVirtualDevice.SetAxis(name, value);
        }
        protected override void OnInitialization()
        {
            inputVirtualDevice = new InputVirtualDevice();
        }
        protected override void OnUpdate()
        {
            //注意：闸门屏蔽期间仍然刷新原始状态，否则"按下-抬起"会整段丢失；
            //屏蔽只作用在查询侧。
            if (IsEnableInputDevice)
                inputHelper?.OnRefresh();
        }
        protected override void OnTermination()
        {
            inputHelper?.OnTermination();
            inputHelper = null;
            inputBlocks.Clear();
            inputVirtualDevice?.ResetAll();
        }
    }
}
