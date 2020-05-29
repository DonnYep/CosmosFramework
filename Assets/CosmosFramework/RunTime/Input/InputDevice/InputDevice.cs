using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public abstract class InputDevice : Input. IInputDevice
    {
        public abstract void OnStart();
        public abstract void OnRun();
        public abstract void OnShutdown();
    }
}