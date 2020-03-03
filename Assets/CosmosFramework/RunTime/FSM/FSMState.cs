using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    public abstract class FSMState<T> where T:class
    {
        #region Properties
        List<FSMTranslation<T>> triggerList = new List<FSMTranslation<T>>();
        Dictionary<FSMTranslation<T>, FSMState<T>> triggerStateDict = new Dictionary<FSMTranslation<T>, FSMState<T>>();
        public void AddTranslation(FSMTranslation<T> trans,FSMState<T> state)
        {
            if (triggerStateDict.ContainsKey(trans))
                return;
            triggerStateDict.Add(trans, state);
            triggerList.Add(trans);
        }
        public void RemoveTranslation(FSMTranslation<T> trans)
        {
            if (!triggerStateDict.ContainsKey(trans))
                return;
            triggerStateDict.Remove(trans);
            triggerList.Remove(trans);
        }
        /// <summary>
        /// 获取被触发的状态
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public FSMState<T> GetTriggeredState(FSMTranslation<T> trans)
        {
            if (triggerStateDict.ContainsKey(trans))
                return triggerStateDict[trans];
            return null;
        }
        #endregion
        #region Lifecycle
        public abstract void OnInit(IFSM<T> fsm);
        public abstract void OnEnter(IFSM<T> fsm);
        public abstract void OnExit(IFSM<T> fsm);
        /// <summary>
        /// 有限状态机状态销毁或终止时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
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