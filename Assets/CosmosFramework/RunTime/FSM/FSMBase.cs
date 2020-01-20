using UnityEngine;
using System.Collections;
namespace Cosmos.FSM
{
    public abstract class FSMBase
    {
        string name;
        public string Name { get { return name; }
            protected set { name = value ?? string.Empty; } }
        public abstract int FSMStateCount { get; }
        public abstract bool IsRunning { get; }
     
    }
}