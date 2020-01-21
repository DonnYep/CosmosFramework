using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.FSM{
    public sealed class FSM<T> : FSMBase,IFSM<T>
        where T : class
    {
        T owner;
        public T Owner { get { return owner; } }
        Dictionary<Type, FSMState<T>> fsmMap = new Dictionary<Type, FSMState<T>>();
        FSMState<T> currentState;
        FSMState<T> previousState;
        public override int FSMStateCount { get; }
        public override bool IsRunning { get; }
        public TState GetState<TState>()where TState : FSMState<T>
        {
            FSMState<T> state = null;
            if (fsmMap.TryGetValue(typeof(TState),out state))
            {
                return (TState)state;
            }
            return null;
        }
        internal void ChangeState<TState>()
            where TState:FSMState<T>
        {
            if(currentState==null)
            {
                Utility.DebugError("Current state is invaild");
            }
        }
    }
}