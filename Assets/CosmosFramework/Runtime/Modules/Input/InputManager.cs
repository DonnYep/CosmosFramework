using UnityEngine;
namespace Cosmos.Input
{
    //================================================
    /*
     * 1、输入模块，适配不同平台的按键输入事件；
     */
    //================================================
    [Module]
    internal sealed class InputManager : Module, IInputManager
    {
        bool isEnableInputDevice = true;
        InputVirtualDevice inputVirtualDevice;
        IInputHelper inputHelper;
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
            set { isEnableInputDevice = value; }
        }
        ///<inheritdoc/>
        public void SetInputHelper(IInputHelper helper)
        {
            inputHelper?.OnTermination();
            inputHelper = helper;
            inputHelper?.OnInitialization();
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
            return inputVirtualDevice.GetAxis(name, false);
        }
        ///<inheritdoc/>
        public float GetAxisRaw(string name)
        {
            return inputVirtualDevice.GetAxis(name, true);
        }
        ///<inheritdoc/>
        public bool GetButtonDown(string name)
        {
            return inputVirtualDevice.GetButtonDown(name);
        }
        ///<inheritdoc/>
        public bool GetButton(string name)
        {
            return inputVirtualDevice.GetButton(name);
        }
        ///<inheritdoc/>
        public bool GetButtonUp(string name)
        {
            return inputVirtualDevice.GetButtonUp(name);
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
            if (IsEnableInputDevice)
                inputHelper?.OnRefresh();
        }
        protected override void OnTermination()
        {
            inputHelper?.OnTermination();
        }
    }
}
