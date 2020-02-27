using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
namespace Cosmos.FSM{
    /// <summary>
    /// fsmMgr设计成，轮询是在具体对象山给轮询的，fsmMgr作为一个Fsm的事件中心
    /// </summary>
    public sealed class FSMManager : Module<FSMManager>
    {
        Dictionary<string, FSMBase> fsms = new Dictionary<string, FSMBase>();
        public int FsmCount { get { return fsms.Count; } }
        protected override void InitModule()
        {
            RegisterModule(CFModules.FSM);
        }
        public void RegisterFSM(FSMBase fsm)
        {
            if (!fsms.ContainsKey(fsm.Name))
                fsms.Add(fsm.Name, fsm);
            else
                Utility.DebugError("FSMManager\n"+"Fsm " + fsm.Name + " is already registered !");
        }
        public void DeregisterFSM(FSMBase fsm)
        {
            if (fsms.ContainsKey(fsm.Name))
                fsms.Remove(fsm.Name);
            else
                Utility.DebugError("FSMManager\n" + "Fsm " + fsm.Name + "not registered !");
        }
        public FSMBase GetFsm( string name)
        {
            if (fsms.ContainsKey(name))
                return fsms[name];
            else return null;
        }
        public FSMBase[] GetAllFsm()
        {
            List<FSMBase> fsmList = new List<FSMBase>();
            foreach (var fsm in fsms)
            {
                fsmList.Add(fsm.Value);
            }
            return fsmList.ToArray();
        }
        public bool HasFsm(string name)
        {
            return fsms.ContainsKey(name);
        }
        //public bool HasFsm<T>()
        //    where T:class
        //{
        //    return false;
        //}
        //public IFSM<T> GetFsm<T>()
        //    where T:class
        //{
        //    return null;
        //}
        //public IFSM<T> CreateFsm<T>(T owner,List<FSMState<T>> states)
        //    where T:class
        //{
        //    return null;
        //}
    }
}