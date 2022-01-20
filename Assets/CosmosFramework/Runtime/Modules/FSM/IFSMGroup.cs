using System;
using System.Collections.Generic;

namespace Cosmos.FSM
{
    /// <summary>
    /// 状态机池
    /// </summary>
    internal interface IFSMGroup:IRefreshable,IControllable
    {
        /// <summary>
        /// 轮询更新间隔
        /// </summary>
        float RefreshInterval { get;}
        List<FSMBase> FSMSet { get; }
        void AddFSM(FSMBase fsm);
        void SetRefreshInterval(float interval);
        void DestoryFSM(Predicate<FSMBase> predicate);
        void DestoryFSM(FSMBase fsm);
        void DestoryAllFSM();
        bool HasFSM(Predicate<FSMBase> predicate);
        bool HasFSM(FSMBase fsm);
        FSMBase GetFSM(Predicate<FSMBase> predicate);
        int GetFSMCount();
    }
}
