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
    }
}