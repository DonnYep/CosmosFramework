using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
namespace Cosmos.FSM{
    public sealed class FSMManager : Module<FSMManager>
    {

        protected override void InitModule()
        {
            RegisterModule(CFModule.FSM);
        }
    }
}