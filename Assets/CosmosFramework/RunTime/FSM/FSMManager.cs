using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cosmos;
namespace Cosmos.FSM{
    /// <summary>
    /// fsmMgr设计成，轮询是在具体对象山给轮询的，fsmMgr作为一个Fsm的事件中心
    /// </summary>
    internal sealed class FSMManager : Module<FSMManager>
    {
        Dictionary<Type, FSMBase> fsmDict = new Dictionary<Type, FSMBase>();
        internal int FsmCount { get { return fsmDict.Count; } }
        internal FSMBase GetFSM<T>()
            where T:FSMBase
        {
            Type type = typeof(T).GetType();
            return GetFSM(type);
        }
        internal FSMBase GetFSM(Type type)
        {
            if (fsmDict.ContainsKey(type))
            {
                return fsmDict[type];
            }
            else return null;
        }
        internal FSMBase[] GetAllFSM<T>()
            where T:FSMBase
        {
            return GetAllFSM(typeof(T));
        }
        internal FSMBase[] GetAllFSM(Type type)
        {
            List<FSMBase> fsms = new List<FSMBase>();
            if (fsmDict.ContainsKey(type))
            {
                foreach (var fsm in fsmDict)
                {
                    fsms.Add(fsm.Value);
                }
            }
            return fsms.ToArray();
        }
        internal bool HasFSM<T>()
            where T : FSMBase
        {
            return HasFSM(typeof(T));
        }
        internal bool HasFSM(Type type)
        {
            return fsmDict.ContainsKey(type);
        }
        internal IFSM<T> CreateFSM<T>(T owner,params FSMState<T>[] states)
            where T :class
        {
           return CreateFSM(string.Empty, owner, states);
        }
        internal IFSM<T>CreateFSM<T>(string name,T owner,params FSMState<T>[] states)
            where T : class
        {
            if (!HasFSM(typeof(T)))
                return null;
            Type type = typeof(T).GetType();
            FSM<T> fsm = FSM<T>.Create(name, owner, states);
            fsmDict.Add(type, fsm);
            return fsm;
        }
        internal IFSM<T> CreateFSM<T>(T owner,List<FSMState<T>> states)
            where T:class
        {
            return CreateFSM(string.Empty, owner, states);
        }
        internal IFSM<T> CreateFSM<T>(string name,T owner, List<FSMState<T>> states)
            where T : class
        {
            if(!HasFSM(typeof(T)))
                return null;
            Type type = typeof(T).GetType();
            FSM<T> fsm = FSM<T>.Create(name, owner, states);
            fsmDict.Add(type, fsm);
            return fsm;
        }
        internal void DestoryFSM<T>()
            where T:class
        {
            DestoryFSM(typeof(T));
        }
        internal void DestoryFSM(Type type)
        {
            FSMBase fsm = null;
            if (fsmDict.TryGetValue(type, out fsm))
            {
                fsm.Shutdown();
                fsmDict.Remove(type);
            }
        }
       internal void ShutdownAllFSM()
        {
            foreach (var fsm in fsmDict)
            {
                fsm.Value.Shutdown();
            }
            fsmDict.Clear();
        }
    }
}