using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    public abstract class FSMState<T> where T:class
    {
        public abstract void OnInit(IFSM<T> fsm);
        public abstract void OnEnter(IFSM<T> fsm);
        public abstract void OnExit(IFSM<T> fsm);
        public abstract void Update(IFSM<T> fsm);
        public abstract void OnReason(IFSM<T> fsm);
        public abstract void OnTermination(IFSM<T> fsm);
        public virtual void ChangeState(IFSM<T> fsm)
        {
            FSM<T> fsmObject = (FSM <T>) fsm;
        }
        HashSet<FSMTranslation> hashTranslation = new HashSet<FSMTranslation>();
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
            var result= Utility.GetTypeInstance<FSMTranslation>(this.GetType().Assembly, translationName);
            hashTranslation.Add(result);
        }

    }
}