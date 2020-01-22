using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.FSM{
    public sealed class FSM<T> : FSMBase,IFSM<T>
        where T : class
    {
        T owner;
        public T Owner { get { return owner; } protected set { owner = value; } }
        public Type OwnerType { get { return typeof(T); } }
        Dictionary<Type, FSMState<T>> fsmStates = new Dictionary<Type, FSMState<T>>();
        Dictionary<string, FSMData> fsmDatas = new Dictionary<string, FSMData>();
        FSMState<T> currentState;
        FSMState<T> previousState;
        FSMData data;
        public override int FSMStateCount { get { return fsmStates.Count; } }
        public override bool IsRunning { get { return currentState != null; } }
        public FSMState<T> CurrentState { get { return currentState; } }
        public FSMData CurrentData { get { return data; } }
        public string CurrentStateName{get{return currentState != null ? currentState.GetType().FullName : string.Empty;}}
        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public void UpdateHandler(object sender,GameEventArgs arg)
        {

        }
        public void SetData(string dataName, FSMData data)
        {
            if (string.IsNullOrEmpty(dataName))
                Utility.DebugError("Data name is invalid !");
            fsmDatas[dataName] = data;
        }
        public void SetData<TData>(string dataName,TData data)
            where TData:FSMData
        {
            if (string.IsNullOrEmpty(dataName))
                Utility.DebugError("Data name is invalid !");
            fsmDatas[dataName] = data;
        }
        public FSMData GetData(string dataName)
        {
            if (string.IsNullOrEmpty(dataName))
                Utility.DebugError("Data name is invalid !");
            FSMData data = null;
            if(fsmDatas.TryGetValue(dataName,out data))
            {
                return data;
            }
            return null;
        }
        public TData GetData<TData>(string dataName)
            where TData:FSMData
        {
            return (TData)GetData(dataName);
        }
        public bool HasData(string dataName)
        {
            if (string.IsNullOrEmpty(dataName))
                Utility.DebugError("Data name is invalid !");
            return fsmDatas.ContainsKey(dataName);
        }
        public void RemoveData(string dataName)
        {
            if (string.IsNullOrEmpty(dataName))
            {
                Utility.DebugError("Data name is invalid !");
                return;
            }
            if (fsmDatas.ContainsKey(dataName))
                fsmDatas.Remove(dataName);
            else
                Utility.DebugError("Fsm data :" + dataName + " not set !");
        }
        public void Renewal()
        {
            if (data != null)
                data.OnRenewal();
        }
        public FSMState<T> GetState(Type stateType)
        {
            if (stateType == null)
            {
                Utility.DebugError("State type is invaild !");
                return null;
            }
            if(!typeof(FSM<T>).IsAssignableFrom(stateType))
            {
                Utility.DebugError("State type is invaild !"+"\n"+stateType.FullName);
                return null;
            }
            FSMState<T> state = null;
            if (fsmStates.TryGetValue(stateType, out state))
                return state;
            return null;

        }
        public TState GetState<TState>()where TState : FSMState<T>
        {
            FSMState<T> state = null;
            if (fsmStates.TryGetValue(typeof(TState),out state))
                return (TState)state;
            return null;
        }
        public FSMState<T>[] GetAllStates()
        {
            List<FSMState<T>> states = new List<FSMState<T>>();
            foreach (var state in fsmStates)
            {
                states.Add(state.Value);
            }
            return states.ToArray();
        }
        public void GetAllState(ref List<FSMState<T>> result)
        {
            if (result == null)
            {
                Utility.DebugError("Result is invalid !");
            }
            result.Clear();
            foreach (var state in fsmStates)
            {
                result.Add(state.Value);
            }
        }
        public bool HasState(Type stateType)
        {
            if (stateType == null)
            {
                Utility.DebugError("State type is invalid !");
                return false;
            }
            if(!typeof(FSMState<T>).IsAssignableFrom(stateType))
            {
                Utility.DebugError("State type is invalid !" + "\n" + stateType.FullName);
                return false;
            }
            return fsmStates.ContainsKey(stateType);

        }
        internal void ChangeState<TState>()
            where TState:FSMState<T>
        {
            if(currentState==null)
            {
                Utility.DebugError("Current state is invalid");
            }
        }
        public void Clear()
        {

        }
    }
}