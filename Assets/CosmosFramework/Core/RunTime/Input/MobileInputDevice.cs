using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Input;
namespace Cosmos
{
    public class MobileInputDevice : InputDevice
    {
        public override void OnStart()
        {
            Facade.RegisterVirtualButton(InputButtonType._MouseLeft);
            Facade.RegisterVirtualButton(InputButtonType._MouseRight);
            Facade.RegisterVirtualButton(InputButtonType._MouseMiddle);
            Facade.RegisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
            Facade.RegisterVirtualAxis(InputAxisType._MouseX);
            Facade.RegisterVirtualAxis(InputAxisType._MouseY);
            Facade.RegisterVirtualAxis(InputAxisType._MouseScrollWheel);
            Facade.RegisterVirtualAxis(InputAxisType._Horizontal);
            Facade.RegisterVirtualAxis(InputAxisType._Vertical);
            Facade.RegisterVirtualAxis(InputAxisType._UpperLower);
        }
        public override void OnRun()
        {
            if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
                Facade.SetButtonDown(InputButtonType._MouseLeft);
            else if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended)
                Facade.SetButtonUp(InputButtonType._MouseLeft);
            if (UnityEngine.Input.touchCount == 2 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began && UnityEngine.Input.GetTouch(1).phase == TouchPhase.Began)
                Facade.SetButtonDown(InputButtonType._MouseRight);
            else
                Facade.SetButtonUp(InputButtonType._MouseRight);
            Facade.SetButtonUp(InputButtonType._MouseMiddle);
            Facade.SetAxis(InputAxisType._MouseX, UnityEngine.Input.GetAxis("Horizontal"));
            Facade.SetAxis(InputAxisType._MouseY, UnityEngine.Input.GetAxis("Vertical"));
            Facade.SetAxis(InputAxisType._MouseScrollWheel, 0);
            Facade.SetAxis(InputAxisType._Horizontal, UnityEngine.Input.GetAxis("Horizontal"));
            Facade.SetAxis(InputAxisType._Vertical, UnityEngine.Input.GetAxis("Vertical"));
            if (UnityEngine.Input.touchCount == 1)
                Facade.SetVirtualMousePosition(UnityEngine.Input.GetTouch(0).position);
            else
                Facade.SetVirtualMousePosition(Vector3.zero);
        }
        public override void OnShutdown()
        {
            Facade.DeregisterVirtualButton(InputButtonType._MouseLeft);
            Facade.DeregisterVirtualButton(InputButtonType._MouseRight);
            Facade.DeregisterVirtualButton(InputButtonType._MouseMiddle);
            Facade.DeregisterVirtualButton(InputButtonType._MouseLeftDoubleClick);
            Facade.DeregisterVirtualAxis(InputAxisType._MouseX);
            Facade.DeregisterVirtualAxis(InputAxisType._MouseY);
            Facade.DeregisterVirtualAxis(InputAxisType._MouseScrollWheel);
            Facade.DeregisterVirtualAxis(InputAxisType._Horizontal);
            Facade.DeregisterVirtualAxis(InputAxisType._Vertical);
            Facade.DeregisterVirtualAxis(InputAxisType._UpperLower);
        }

 
    }
}