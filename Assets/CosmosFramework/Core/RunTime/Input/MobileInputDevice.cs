using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Input;
namespace Cosmos
{
    public class MobileInputDevice : InputDevice
    {
        IInputManager inputManager;
        public override void OnStart()
        {
            inputManager.RegisterVirtualButton(InputButtonType._MouseLeft);
            inputManager.RegisterVirtualButton(InputButtonType._MouseRight);
            inputManager.RegisterVirtualButton(InputButtonType._MouseMiddle);
            inputManager.RegisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
            inputManager.RegisterVirtualAxis(InputAxisType._MouseX);
            inputManager.RegisterVirtualAxis(InputAxisType._MouseY);
            inputManager.RegisterVirtualAxis(InputAxisType._MouseScrollWheel);
            inputManager.RegisterVirtualAxis(InputAxisType._Horizontal);
            inputManager.RegisterVirtualAxis(InputAxisType._Vertical);
            inputManager.RegisterVirtualAxis(InputAxisType._UpperLower);
        }
        public override void OnRun()
        {
            if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
                inputManager.SetButtonDown(InputButtonType._MouseLeft);
            else if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended)
                inputManager.SetButtonUp(InputButtonType._MouseLeft);
            if (UnityEngine.Input.touchCount == 2 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began && UnityEngine.Input.GetTouch(1).phase == TouchPhase.Began)
                inputManager.SetButtonDown(InputButtonType._MouseRight);
            else
                inputManager.SetButtonUp(InputButtonType._MouseRight);
            inputManager.SetButtonUp(InputButtonType._MouseMiddle);
            inputManager.SetAxis(InputAxisType._MouseX, UnityEngine.Input.GetAxis("Horizontal"));
            inputManager.SetAxis(InputAxisType._MouseY, UnityEngine.Input.GetAxis("Vertical"));
            inputManager.SetAxis(InputAxisType._MouseScrollWheel, 0);
            inputManager.SetAxis(InputAxisType._Horizontal, UnityEngine.Input.GetAxis("Horizontal"));
            inputManager.SetAxis(InputAxisType._Vertical, UnityEngine.Input.GetAxis("Vertical"));
            if (UnityEngine.Input.touchCount == 1)
                inputManager.SetVirtualMousePosition(UnityEngine.Input.GetTouch(0).position);
            else
                inputManager.SetVirtualMousePosition(Vector3.zero);
        }
        public override void OnShutdown()
        {
            inputManager.DeregisterVirtualButton(InputButtonType._MouseLeft);
            inputManager.DeregisterVirtualButton(InputButtonType._MouseRight);
            inputManager.DeregisterVirtualButton(InputButtonType._MouseMiddle);
            inputManager.DeregisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
            inputManager.DeregisterVirtualAxis(InputAxisType._MouseX);
            inputManager.DeregisterVirtualAxis(InputAxisType._MouseY);
            inputManager.DeregisterVirtualAxis(InputAxisType._MouseScrollWheel);
            inputManager.DeregisterVirtualAxis(InputAxisType._Horizontal);
            inputManager.DeregisterVirtualAxis(InputAxisType._Vertical);
            inputManager.DeregisterVirtualAxis(InputAxisType._UpperLower);
        }

 
    }
}