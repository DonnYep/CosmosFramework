using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Event
{
    public interface IEventDispatcher
    {
        string DispatcherName { get; }
        void DispatchEvent();
    }
}