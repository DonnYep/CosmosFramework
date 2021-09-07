using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    public abstract class FSMState<T> where T:class
    {
        #region Properties
        List<FSMTrigger<T>> triggerList = new List<FSMTrigger<T>>();
        Dictionary<FSMTrigger<T>, FSMState<T>> triggerStateDict = new Dictionary<FSMTrigger<T>, FSMState<T>>();
        public void AddTrigger(FSMTrigger<T> trans,FSMState<T> state)
        {
            if (triggerStateDict.ContainsKey(trans))
                return;
            triggerStateDict.Add(trans, state);
            triggerList.Add(trans);
        }
        public void RemoveTrigger(FSMTrigger<T> trans)
        {
            if (!triggerStateDict.ContainsKey(trans))
                return;
            triggerStateDict.Remove(trans);
            triggerList.Remove(trans);
        }
        public FSMState<T> GetTriggeredState(FSMTrigger<T> trans)
        {
            if (triggerStateDict.ContainsKey(trans))
                return triggerStateDict[trans];
            return null;
        }
        #endregion
        #region Lifecycle
        public abstract void OnInitialization(IFSM<T> fsm);
        public abstract void OnEnter(IFSM<T> fsm);
        public abstract void OnExit(IFSM<T> fsm);
        public abstract void OnTermination(IFSM<T> fsm);
        public virtual void Reason(IFSM<T> fsm)
        {
            for (int i = 0; i < triggerList.Count; i++)
            {
                if (triggerList[i].Handler(fsm))
                {
                    fsm.ChangeState(GetTriggeredState(triggerList[i]).GetType());
                    return;
                }
            }
        }
        public abstract void Action(IFSM<T> fsm);
        #endregion
    }
}