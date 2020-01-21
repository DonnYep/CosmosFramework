using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    public abstract class FSMState
    {
        public abstract void OnInit();
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void Update();
        public abstract void OnReason();
        public abstract void OnTermination();
    }
}