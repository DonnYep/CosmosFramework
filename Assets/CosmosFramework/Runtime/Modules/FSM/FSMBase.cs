using System.Collections;
using System;
namespace Cosmos.FSM
{
    public abstract class FSMBase:IRefreshable,IControllable
    {
        string name;
        public string Name { get { return name; }
            protected set { name = value ?? string.Empty; } }
        public abstract Type OwnerType { get; }
        public abstract int FSMStateCount { get; }
        public abstract bool IsRunning { get; }
        public abstract string CurrentStateName { get; }
        public bool IsPause { get;protected set; }
        public void OnPause(){IsPause = true;}
        public abstract void OnRefresh();
        public void OnUnPause(){IsPause = false;}
        public abstract void Shutdown();
    }
}