using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.FSM{
    public sealed class FSM<T,K> : IReference
        where T:class
        where K:class
    {
        Dictionary<FSMTranslation<T>, FSMState<K>> fsmMap = new Dictionary<FSMTranslation<T>, FSMState<K>>();
        public void Reset()
        {
        }
    }
}