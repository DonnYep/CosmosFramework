using System.Collections;
using System.Collections.Generic;
using Cosmos;
using Cosmos.FSM;

namespace Cosmos
{
    /// <summary>
    /// 有限状态机时间响应函数
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型</typeparam>
    /// <param name="fsm">有限状态机引用</param>
    /// <param name="sender">事件源，发送者</param>
    /// <param name="fsmData">用户自定义的数据</param>
    public delegate void FSMEventHandler<T>(IFSM<T> fsm,object sender, Variable fsmData)
        where T:class;
}