using System;
namespace Cosmos.FSM
{
    public interface IFSMGroup
    {
        bool IsPause { get; }
        void OnPause();
        void OnUnPause();
        void AddFSM(FSMBase fsm);
        void DestoryFSM(Predicate<FSMBase> predicate);
        void DestoryFSM(FSMBase fsm);
        void SetRefreshInterval(int interval);
        bool HasFSM(Predicate<FSMBase> predicate);
        bool HasFSM(FSMBase fsm);
        FSMBase GetFSM(Predicate<FSMBase> predicate);
        int GetFSMCount();
        void DestoryAllFSM();
    }
}
