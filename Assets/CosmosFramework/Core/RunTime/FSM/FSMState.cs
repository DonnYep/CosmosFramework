using UnityEngine;
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
        public void AddTrigger(FSMTrigger<T> fsmTgr,FSMState<T> state)
        {
            if (triggerStateDict.ContainsKey(fsmTgr))
                return;
            triggerStateDict.Add(fsmTgr, state);
            triggerList.Add(fsmTgr);
        }
        public void RemoveTrigger(FSMTrigger<T> fsmTgr)
        {
            if (!triggerStateDict.ContainsKey(fsmTgr))
                return;
            triggerStateDict.Remove(fsmTgr);
            triggerList.Remove(fsmTgr);
        }
        public FSMState<T> GetTriggeredState(FSMTrigger<T> fsmTgr)
        {
            if (triggerStateDict.ContainsKey(fsmTgr))
                return triggerStateDict[fsmTgr];
            return null;
        }
        #endregion
        #region Lifecycle
        /// <summary>
        /// 初始化状态;
        /// </summary>
        public abstract void OnInitialization(IFSM<T> fsm);
        /// <summary>
        ///进入状态事件 ;
        /// </summary>
        public abstract void OnEnter(IFSM<T> fsm);
        /// <summary>
        ///离开状态事件 ;
        /// </summary>
        public abstract void OnExit(IFSM<T> fsm);
        /// <summary>
        ///状态终结事件 ;
        /// </summary>
        public abstract void OnTermination(IFSM<T> fsm);
        /// <summary>
        ///状态检测事件； 
        /// </summary>
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
        /// <summary>
        /// 状态触发事件；
        /// </summary>
        public abstract void Action(IFSM<T> fsm);
        #endregion
    }
}