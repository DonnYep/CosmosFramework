using UnityEngine;
using System.Collections;
using System;
namespace Cosmos.FSM
{
    public abstract class FSMBase
    {
        string name;
        public string Name { get { return name; }
            protected set { name = value ?? string.Empty; } }
        public abstract Type OwnerType { get; }
        public abstract int FSMStateCount { get; }
        public abstract bool IsRunning { get; }
        public abstract string CurrentStateName { get; }
        public abstract void Shutdown();
        public abstract void Update();
    }
}