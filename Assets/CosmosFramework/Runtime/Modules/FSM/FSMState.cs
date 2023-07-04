using System;
using System.Collections.Generic;
namespace Cosmos.FSM
{
    public abstract class FSMState<T> where T : class
    {
        #region Properties
        List<FSMTransition<T>> transitionList = new List<FSMTransition<T>>();
        Dictionary<FSMTransition<T>, Type> transitionStateDict
            = new Dictionary<FSMTransition<T>, Type>();
        public void AddTransition(FSMTransition<T> transition, Type stateType)
        {
            if (transitionStateDict.ContainsKey(transition))
                return;
            if (!this.GetType().IsAssignableFrom(stateType))
                throw new ArgumentException($"State type {stateType.FullName} is invalid !");
            transitionStateDict.Add(transition, stateType);
            transitionList.Add(transition);
        }
        public void RemoveTransition(FSMTransition<T> transition)
        {
            if (!transitionStateDict.ContainsKey(transition))
                return;
            transitionStateDict.Remove(transition);
            transitionList.Remove(transition);
        }
        public Type GetTransitionState(FSMTransition<T> transition)
        {
            if (transitionStateDict.ContainsKey(transition))
                return transitionStateDict[transition];
            return null;
        }
        #endregion
        #region Lifecycle
        public virtual void OnInitialization(IFSM<T> fsm) { }
        public abstract void OnStateEnter(IFSM<T> fsm);
        public abstract void OnStateStay(IFSM<T> fsm);
        public virtual void RefreshTransition(IFSM<T> fsm)
        {
            for (int i = 0; i < transitionList.Count; i++)
            {
                if (transitionList[i].Handler(fsm))
                {
                    fsm.ChangeState(GetTransitionState(transitionList[i]));
                    return;
                }
            }
        }
        public abstract void OnStateExit(IFSM<T> fsm);
        public virtual void OnTermination(IFSM<T> fsm) { }
        #endregion
        public virtual void Release()
        {
            transitionList.Clear();
            transitionStateDict.Clear();
        }
    }
}