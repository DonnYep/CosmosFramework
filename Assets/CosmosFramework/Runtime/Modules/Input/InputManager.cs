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
        ///<inheritdoc/>
        public bool IsEnableInputDevice { get; set; } = true;
        VirtualInput inputModule = new VirtualInput();
        IInputHelper inputHelper;
        ///<inheritdoc/>
        public void SetInputHelper(IInputHelper helper)
        {
            inputHelper?.OnShutdown();
            inputHelper = helper;
            inputHelper?.OnStart();
        }
        ///<inheritdoc/>
        public bool IsExistVirtualAxis(string name)
        {
            return inputModule.IsExistVirtualAxis(name);
        }
        ///<inheritdoc/>
        public bool IsExistVirtualButton(string name)
        {
            return inputModule.IsExistVirtualButton(name);
        }
        ///<inheritdoc/>
        public void RegisterVirtualButton(string name)
        {
            inputModule.RegisterVirtualButton(name);
        }
        ///<inheritdoc/>
        public void DeregisterVirtualButton(string name)
        {
            inputModule.DeregisterVirtualButton(name);
        }
        ///<inheritdoc/>
        public void RegisterVirtualAxis(string name)
        {
            inputModule.RegisterVirtualAxis(name);
        }
        ///<inheritdoc/>
        public void DeregisterVirtualAxis(string name)
        {
            inputModule.DeregisterVirtualAxis(name);
        }
        ///<inheritdoc/>
        public Vector3 MousePosition { get { return inputModule.MousePosition; } }
        ///<inheritdoc/>
        public float GetAxis(string name)
        {
            return inputModule.GetAxis(name, false);
        }
        ///<inheritdoc/>
        public float GetAxisRaw(string name)
        {
            return inputModule.GetAxis(name, true);
        }
        ///<inheritdoc/>
        public bool GetButtonDown(string name)
        {
            return inputModule.GetButtonDown(name);
        }
        ///<inheritdoc/>
        public bool GetButton(string name)
        {
            return inputModule.GetButton(name);
        }
        ///<inheritdoc/>
        public bool GetButtonUp(string name)
        {
            return inputModule.GetButtonUp(name);
        }
        ///<inheritdoc/>
        public void SetButtonDown(string name)
        {
            inputModule.SetButtonDown(name);
        }
        ///<inheritdoc/>
        public void SetButtonUp(string name)
        {
            inputModule.SetButtonUp(name);
        }
        ///<inheritdoc/>
        public void SetVirtualMousePosition(Vector3 value)
        {
            inputModule.SetVirtualMousePosition(value);
        }
        ///<inheritdoc/>
        public void SetVirtualMousePosition(float x, float y, float z)
        {
            inputModule.SetVirtualMousePosition(x, y, z);
        }
        ///<inheritdoc/>
        public void SetAxisPositive(string name)
        {
            inputModule.SetAxisPositive(name);
        }
        ///<inheritdoc/>
        public void SetAxisNegative(string name)
        {
            inputModule.SetAxisNegative(name);
        }
        ///<inheritdoc/>
        public void SetAxisZero(string name)
        {
            inputModule.SetAxisZero(name);
        }
        ///<inheritdoc/>
        public void SetAxis(string name, float value)
        {
            inputModule.SetAxis(name, value);
        }
        protected override void OnTermination()
        {
            base.OnTermination();
            inputHelper?.OnShutdown();
        }
        [TickRefresh]
        void OnRefresh()
        {
            if (IsPause)
                return;
            if (IsEnableInputDevice)
                inputHelper?.OnRun();
        }
    }
}
