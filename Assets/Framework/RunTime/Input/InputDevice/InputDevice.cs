using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Input{
    public abstract class InputDevice : IInputDevice
    {
        public abstract void OnRun();
        public abstract void OnShutdown();
        public abstract void OnStart();
    }
}