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
            Facade.RegisterVirtualButton(InputButtonType.MouseLeft);
            Facade.RegisterVirtualButton(InputButtonType.MouseRight);
            Facade.RegisterVirtualButton(InputButtonType.MouseMiddle);
            Facade.RegisterVirtualButton(InputButtonType.MouseLeftDoubleClick);
            Facade.RegisterVirtualAxis(InputAxisType.MouseX);
            Facade.RegisterVirtualAxis(InputAxisType.MouseY);
            Facade.RegisterVirtualAxis(InputAxisType.MouseScrollWheel);
            Facade.RegisterVirtualAxis(InputAxisType.Horizontal);
            Facade.RegisterVirtualAxis(InputAxisType.Vertical);
            Facade.RegisterVirtualAxis(InputAxisType.UpperLower);
        }
        public override void OnRun()
        {
            if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
                Facade.SetButtonDown(InputButtonType.MouseLeft);
            else if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended)
                Facade.SetButtonUp(InputButtonType.MouseLeft);
            if (UnityEngine.Input.touchCount == 2 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began && UnityEngine.Input.GetTouch(1).phase == TouchPhase.Began)
                Facade.SetButtonDown(InputButtonType.MouseRight);
            else
                Facade.SetButtonUp(InputButtonType.MouseRight);
            Facade.SetButtonUp(InputButtonType.MouseMiddle);
            Facade.SetAxis(InputAxisType.MouseX, UnityEngine.Input.GetAxis("Horizontal"));
            Facade.SetAxis(InputAxisType.MouseY, UnityEngine.Input.GetAxis("Vertical"));
            Facade.SetAxis(InputAxisType.MouseScrollWheel, 0);
            Facade.SetAxis(InputAxisType.Horizontal, UnityEngine.Input.GetAxis("Horizontal"));
            Facade.SetAxis(InputAxisType.Vertical, UnityEngine.Input.GetAxis("Vertical"));
            if (UnityEngine.Input.touchCount == 1)
                Facade.SetVirtualMousePosition(UnityEngine.Input.GetTouch(0).position);
            else
                Facade.SetVirtualMousePosition(Vector3.zero);
        }
        public override void OnShutdown()
        {
            Facade.DeregisterVirtualButton(InputButtonType.MouseLeft);
            Facade.DeregisterVirtualButton(InputButtonType.MouseRight);
            Facade.DeregisterVirtualButton(InputButtonType.MouseMiddle);
            Facade.DeregisterVirtualButton(InputButtonType.MouseLeftDoubleClick);
            Facade.DeregisterVirtualAxis(InputAxisType.MouseX);
            Facade.DeregisterVirtualAxis(InputAxisType.MouseY);
            Facade.DeregisterVirtualAxis(InputAxisType.MouseScrollWheel);
            Facade.DeregisterVirtualAxis(InputAxisType.Horizontal);
            Facade.DeregisterVirtualAxis(InputAxisType.Vertical);
            Facade.DeregisterVirtualAxis(InputAxisType.UpperLower);
        }

 
    }
}