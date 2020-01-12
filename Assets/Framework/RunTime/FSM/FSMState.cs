using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    public class FSMState<T>
        where T:class
    {
        Dictionary<int, FSMEventHandler<T>> eventHandlers;
        public FSMState()
        {
            eventHandlers = new Dictionary<int, FSMEventHandler<T>>();
        }
    }
}