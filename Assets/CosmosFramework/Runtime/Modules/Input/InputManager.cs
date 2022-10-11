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
        VirtualInput virtualInput = new VirtualInput();
        IInputHelper inputHelper;
        ///<inheritdoc/>
        public int VirtualAxisCount
        {
            get { return virtualInput.VirtualAxisCount; }
        }
        ///<inheritdoc/>
        public int VirtualButtonCount
        {
            get { return virtualInput.VirtualButtonCount; }
        }
        ///<inheritdoc/>
        public Vector3 MousePosition
        {
            get { return virtualInput.MousePosition; }
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
            return virtualInput.IsExistVirtualAxis(name);
        }
        ///<inheritdoc/>
        public bool IsExistVirtualButton(string name)
        {
            return virtualInput.IsExistVirtualButton(name);
        }
        ///<inheritdoc/>
        public void RegisterVirtualButton(string name)
        {
            virtualInput.RegisterVirtualButton(name);
        }
        ///<inheritdoc/>
        public void DeregisterVirtualButton(string name)
        {
            virtualInput.DeregisterVirtualButton(name);
        }
        ///<inheritdoc/>
        public void RegisterVirtualAxis(string name)
        {
            virtualInput.RegisterVirtualAxis(name);
        }
        ///<inheritdoc/>
        public void DeregisterVirtualAxis(string name)
        {
            virtualInput.DeregisterVirtualAxis(name);
        }

        ///<inheritdoc/>
        public float GetAxis(string name)
        {
            return virtualInput.GetAxis(name, false);
        }
        ///<inheritdoc/>
        public float GetAxisRaw(string name)
        {
            return virtualInput.GetAxis(name, true);
        }
        ///<inheritdoc/>
        public bool GetButtonDown(string name)
        {
            return virtualInput.GetButtonDown(name);
        }
        ///<inheritdoc/>
        public bool GetButton(string name)
        {
            return virtualInput.GetButton(name);
        }
        ///<inheritdoc/>
        public bool GetButtonUp(string name)
        {
            return virtualInput.GetButtonUp(name);
        }
        ///<inheritdoc/>
        public void SetButtonDown(string name)
        {
            virtualInput.SetButtonDown(name);
        }
        ///<inheritdoc/>
        public void SetButtonUp(string name)
        {
            virtualInput.SetButtonUp(name);
        }
        ///<inheritdoc/>
        public void SetVirtualMousePosition(Vector3 value)
        {
            virtualInput.SetVirtualMousePosition(value);
        }
        ///<inheritdoc/>
        public void SetVirtualMousePosition(float x, float y, float z)
        {
            virtualInput.SetVirtualMousePosition(x, y, z);
        }
        ///<inheritdoc/>
        public void SetAxisPositive(string name)
        {
            virtualInput.SetAxisPositive(name);
        }
        ///<inheritdoc/>
        public void SetAxisNegative(string name)
        {
            virtualInput.SetAxisNegative(name);
        }
        ///<inheritdoc/>
        public void SetAxisZero(string name)
        {
            virtualInput.SetAxisZero(name);
        }
        ///<inheritdoc/>
        public void SetAxis(string name, float value)
        {
            virtualInput.SetAxis(name, value);
        }
        protected override void OnTermination()
        {
            inputHelper?.OnTermination();
        }
        [TickRefresh]
        void OnRefresh()
        {
            if (IsPause)
                return;
            if (IsEnableInputDevice)
                inputHelper?.OnRefresh();
        }
    }
}
