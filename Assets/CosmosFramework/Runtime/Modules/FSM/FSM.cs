﻿using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    //type.ToString()输出一个完全限定名，尝试使用反射机制获得对象
    internal sealed class FSM<T> : FSMBase, IFSM<T>, IReference
         where T : class
    {
        #region Properties
        T owner;
        public T Owner { get { return owner; } private set { owner = value; } }
        FSMState<T> currentState;
        public FSMState<T> CurrentState { get { return currentState; } }
        bool isDestoryed;
        public bool IsDestoryed { get { return isDestoryed; } private set { isDestoryed = value; } }
        FSMState<T> defaultState;
        public FSMState<T> DefaultState { get { return defaultState; } set { defaultState = value; } }
        /// <summary>
        /// state存储的类型为派生类
        /// </summary>
        Dictionary<Type, FSMState<T>> fsmStateDict = new Dictionary<Type, FSMState<T>>();
        public override int FSMStateCount { get { return fsmStateDict.Count; } }
        public override bool IsRunning { get { return currentState != null; } }
        public override Type OwnerType { get { return typeof(T); } }
        public override string CurrentStateName { get { return currentState != null ? currentState.GetType().FullName : "<NULL>"; } }
        #endregion

        #region Lifecycle
        public void Release()
        {
            if (currentState != null)
            {
                currentState.OnExit(this);
            }
            foreach (var state in fsmStateDict)
            {
                state.Value.OnTermination(this);
            }
            Name = null;
            IsDestoryed = true;
            fsmStateDict.Clear();
            currentState = null;
        }
        public void StartDefault()
        {
            if (IsRunning)
                return;
            if (defaultState == null)
                return;
            Type type = defaultState.GetType();
            if (!typeof(FSMState<T>).IsAssignableFrom(type))
                throw new ArgumentException("State type is invalid" + type.FullName);
            FSMState<T> state = GetState(type);
            if (state == null)
                return;
            currentState = state;
            currentState.OnEnter(this);
        }
        /// <summary>
        /// 进入状态
        /// </summary>
        public void Start(Type stateType)
        {
            if (IsRunning)
                return;
            if (stateType == null)
                return;
            if (!typeof(FSMState<T>).IsAssignableFrom(stateType))
                throw new ArgumentException("State type is invalid" + stateType.FullName);
            FSMState<T> state = GetState(stateType);
            if (state == null)
                return;
            currentState = state;
            currentState.OnEnter(this);
        }
        public void Start<TState>()
            where TState : FSMState<T>
        {
            if (IsRunning)
                return;
            var state = GetState<TState>();
            if (state == null)
                return;
            currentState = state;
            currentState.OnEnter(this);
        }
        /// <summary>
        /// FSM轮询，由拥有者轮询调用
        /// </summary>
        public override void OnRefresh()
        {
            if (Pause)
                return;
            currentState?.Reason(this);
            currentState?.Action(this);
        }
        public override void Shutdown()
        {
            ReferencePool.Release(this);
        }
        #endregion

        #region State
        public bool HasState(Type stateType)
        {
            if (stateType == null)
                throw new ArgumentNullException("State type is invalid !");
            if (!typeof(FSMState<T>).IsAssignableFrom(stateType))
                throw new ArgumentException("State type is invalid !" + stateType.FullName);
            return fsmStateDict.ContainsKey(stateType);
        }
        public bool HasState(FSMState<T> state)
        {
            return HasState(state.GetType());
        }
        public bool HasState<TState>()
            where TState : FSMState<T>
        {
            return HasState(typeof(TState));
        }
        public void ChangeState<TState>()
            where TState : FSMState<T>
        {
            ChangeState(typeof(TState));
        }
        public void ChangeState(Type stateType)
        {
            if (currentState == null)
                throw new ArgumentNullException("Current state is invalid");
            FSMState<T> state = null;
            state = GetState(stateType);
            if (state == null)
                throw new ArgumentNullException("FSM" + currentState.ToString() + " can not change state to " + state.ToString() + " which is not exist");
            currentState.OnExit(this);
            currentState = state;
            currentState.OnEnter(this);
        }
        public void GetAllState(out List<FSMState<T>> result)
        {
            result = null;
            foreach (var state in fsmStateDict)
            {
                result.Add(state.Value);
            }
        }
        public FSMState<T> GetState(Type stateType)
        {
            if (stateType == null)
                throw new ArgumentNullException("State type is invaild !");
            if (!typeof(FSMState<T>).IsAssignableFrom(stateType))
                throw new ArgumentNullException("State type is invaild !" + stateType.FullName);
            FSMState<T> state = null;
            if (fsmStateDict.TryGetValue(stateType, out state))
                return state;
            return null;
        }
        public TState GetState<TState>() where TState : FSMState<T>
        {
            FSMState<T> state = null;
            if (fsmStateDict.TryGetValue(typeof(TState), out state))
                return (TState)state;
            return null;
        }
        public FSMState<T>[] GetAllState()
        {
            List<FSMState<T>> states = new List<FSMState<T>>();
            foreach (var state in fsmStateDict)
            {
                states.Add(state.Value);
            }
            return states.ToArray();
        }
        #endregion

        internal static FSM<T> Create(string name, T owner, params FSMState<T>[] states)
        {
            if (states == null || states.Length < 1)
                throw new ArgumentNullException("FSM owner is invalid");
            //从引用池获得同类
            FSM<T> fsm = ReferencePool.Acquire<FSM<T>>();
            fsm.Name = name;
            fsm.Owner = owner;
            fsm.IsDestoryed = false;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i] == null)
                    throw new ArgumentNullException("FSM owner is invalid");
                Type type = states[i].GetType();
                if (fsm.HasState(type))
                    throw new ArgumentException("FSM state is is already exist " + states[i].GetType().FullName);
                else
                {
                    states[i].OnInitialization(fsm);
                    fsm.fsmStateDict.Add(type, states[i]);
                }
            }
            return fsm;
        }
        internal static FSM<T> Create(string name, T owner, IList<FSMState<T>> states)
        {
            if (states == null || states.Count < 1)
                throw new ArgumentNullException("FSM owner is invalid");
            //从引用池获得同类
            FSM<T> fsm = ReferencePool.Acquire<FSM<T>>();
            fsm.Name = name;
            fsm.Owner = owner;
            fsm.IsDestoryed = false;
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i] == null)
                    throw new ArgumentNullException("FSM owner is invalid");
                Type type = states[i].GetType();
                if (fsm.HasState(type))
                    throw new ArgumentException("FSM state is is already exist " + states[i].GetType().FullName);
                else
                {
                    states[i].OnInitialization(fsm);
                    fsm.fsmStateDict.Add(type, states[i]);
                }
            }
            return fsm;
        }
    }
}