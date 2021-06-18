using System.Collections;
using System;
namespace Cosmos.FSM
{
    public abstract class FSMBase:IRefreshable
    {
        string name;
        public string Name { get { return name; }
            protected set { name = value ?? string.Empty; } }
        public abstract Type OwnerType { get; }
        public abstract int FSMStateCount { get; }
        public abstract bool IsRunning { get; }
        public abstract string CurrentStateName { get; }
        public bool Pause { get;set; }
        public abstract void OnRefresh();
        public abstract void Shutdown();
    }
}