using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
namespace Cosmos.FSM{
    public sealed class FSMManager : Module<FSMManager>
    {
        Dictionary<object, FSMBase> fsmMap = new Dictionary<object, FSMBase>();
        protected override void InitModule()
        {
            RegisterModule(CFModule.FSM);
        }
        public void RegisterFSM(FSMBase fsm)
        {

        }
        public bool HasFsm<T>()
            where T:class
        {
            return false;
        }
        public bool HasFsm(string name)
        {
            return false;
        }
        public IFSM<T> GetFsm<T>()
            where T:class
        {
            return null;
        }
        public IFSM<T> CreateFsm<T>(T owner,List<FSMState<T>> states)
            where T:class
        {
            return null;
        }
    }
}