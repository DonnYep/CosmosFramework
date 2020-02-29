using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    public abstract class FSMState<T> where T:class
    {
        #region Lifecycle
        public abstract void OnInit(IFSM<T> fsm);
        public abstract void OnEnter(IFSM<T> fsm);
        public abstract void OnExit(IFSM<T> fsm);
        public abstract void Action(IFSM<T> fsm);
        public abstract void OnTermination(IFSM<T> fsm);
        public virtual void Reason(IFSM<T> fsm)
        {
            for (int i = 0; i < triggerList.Count; i++)
            {
                if (triggerList[i].Handler(fsm))
                {
                    fsm.ChangeState(GetOutputState(triggerList[i]));
                    return;
                }
            }
        }
        public virtual void ChangeState<TState>(IFSM<T> fsm)
            where TState:FSMState<T>
        {
            FSM<T> fsmObject = (FSM <T>) fsm;
            if (fsmObject != null)
                fsmObject.ChangeState<TState>();
        }
        #endregion
        #region LegacyFsm
        HashSet<FSMTranslation<T>> hashTranslation = new HashSet<FSMTranslation<T>>();
        Dictionary<string, string> stateMap = new Dictionary<string, string>();
        public void AddTranslation(string translationName,string stateName)
        {
            if (stateMap.ContainsKey(translationName))
                stateMap[translationName] = stateName;
            else
            {
                stateMap.Add(translationName, stateName);
                AddTranslationObject(translationName);
            }
        }
        public void RemoveTranslation(string translationName)
        {
            if (stateMap.ContainsKey(translationName))
            {
                stateMap.Remove(translationName);
                RemoveTranslationObject(translationName);
            }
        }
        void RemoveTranslationObject( string translationName)
        {
            hashTranslation.RemoveWhere(t => t.TranslationName == translationName);
        }
        void AddTranslationObject( string translationName)
        {
            var result= Utility.GetTypeInstance<FSMTranslation<T>>(this.GetType().Assembly, translationName);
            hashTranslation.Add(result);
        }
        #endregion
        #region NewFSM
        List<FSMTranslation<T>> triggerList = new List<FSMTranslation<T>>();
        Dictionary<FSMTranslation<T>, FSMState<T>> stateDictMap = new Dictionary<FSMTranslation<T>, FSMState<T>>();
        public void AddTranslation(FSMTranslation<T> trans,FSMState<T> state)
        {
            if (stateDictMap.ContainsKey(trans))
                return;
            stateDictMap.Add(trans, state);
            triggerList.Add(trans);
        }
        public void RemoveTranslation(FSMTranslation<T> trans)
        {
            if (!stateDictMap.ContainsKey(trans))
                return;
            stateDictMap.Remove(trans);
            triggerList.Remove(trans);
        }
        public FSMState<T> GetOutputState(FSMTranslation<T> trans)
        {
            if (stateDictMap.ContainsKey(trans))
                return stateDictMap[trans];
            return null;
        }

        #endregion
    }
}