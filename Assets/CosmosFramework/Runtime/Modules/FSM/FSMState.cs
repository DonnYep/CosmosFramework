using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    public abstract class FSMState<T> where T:class
    {
        #region Properties
        List<FSMTrigger<T>> triggerList = new List<FSMTrigger<T>>();
        Dictionary<FSMTrigger<T>, FSMState<T>> triggerStateDict 
            = new Dictionary<FSMTrigger<T>, FSMState<T>>();
        public void AddTrigger(FSMTrigger<T> trigger,FSMState<T> state)
        {
            if (triggerStateDict.ContainsKey(trigger))
                return;
            triggerStateDict.Add(trigger, state);
            triggerList.Add(trigger);
        }
        public void RemoveTrigger(FSMTrigger<T> trigger)
        {
            if (!triggerStateDict.ContainsKey(trigger))
                return;
            triggerStateDict.Remove(trigger);
            triggerList.Remove(trigger);
        }
        public FSMState<T> GetTriggeredState(FSMTrigger<T> trigger)
        {
            if (triggerStateDict.ContainsKey(trigger))
                return triggerStateDict[trigger];
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