using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.FSM{
    public sealed class FSM<T> : FSMBase
        where T : class
    {
        T owner;
        public T Owner { get { return owner; } }

        public override int FSMStateCount { get; }
        public override bool IsRunning { get; }
    }
}