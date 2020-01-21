using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.FSM
{
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
    }
}