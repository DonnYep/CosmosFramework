using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    /// <summary>
    /// 有限状态机接口
    /// </summary>
    /// <typeparam name="T">持有者类型</typeparam>
    public interface IFSM<T>
        where T:class
    {
        /// <summary>
        /// 状态机持有者
        /// </summary>
        T Owner { get; }
        /// <summary>
        /// 状态机中的状态数量
        /// </summary>
        int FSMStateCount { get; }
        bool IsRunning { get; }
        bool IsDestoryed { get; }
        void ChangeState<TState>()where TState :FSMState<T>;
        void ChangeState(Type stateType);
    }
}