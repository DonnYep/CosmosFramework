using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.FSM
{
    /// <summary>
    /// 有限状态机接口
    /// </summary>
    /// <typeparam name="T">持有者类型</typeparam>
    public interface IFSM<T>
        where T : class
    {
        /// <summary>
        /// 状态机持有者
        /// </summary>
        T Owner { get; }
        /// <summary>
        /// 状态机中的状态数量
        /// </summary>
        int FSMStateCount { get; }
        FSMState<T> CurrentState { get; }
        Variable CurrentData { get; }
        bool IsRunning { get; }
        void ChangeState<TState>() where TState : FSMState<T>;
        void ChangeState(Type stateType);
        bool HasState(Type stateType);
        bool HasState(FSMState<T> state);
        bool HasState<TState>()where TState : FSMState<T>;
        void GetAllState(out List<FSMState<T>> result);
        FSMState<T> GetState(Type stateType);
        TState GetState<TState>() where TState : FSMState<T>;
        FSMState<T>[] GetAllState();
        void SetData(string dataName, Variable data);
        void SetData<TData>(string dataName, TData data) where TData :Variable;
        Variable GetData(string dataName);
        TData GetData<TData>(string dataName) where TData : Variable;
        bool HasData(string dataName);
        void RemoveData(string dataName);
    }
}