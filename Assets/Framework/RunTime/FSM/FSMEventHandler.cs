using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
namespace Cosmos.FSM
{
    /// <summary>
    /// 有限状态机时间响应函数
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型</typeparam>
    /// <param name="fsm">有限状态机引用</param>
    /// <param name="sender">事件源，发送者</param>
    /// <param name="userData">用户自定义的数据</param>
 public delegate void FSMEventHandler<T>(IFSM<T> fsm,object sender,object userData)
        where T:class;
}