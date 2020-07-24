using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cosmos.Event;

namespace Cosmos
{
    [DisallowMultipleComponent]
    public class  ActionDispatcher: MonoBehaviour
    {
        [Header("简易事件分发器，复杂逻辑请使用EventCenter")]
        [SerializeField] UnityEvent action;
        public UnityEvent Action { get { return action; }set { action = value; } }
        public string DispatcherName { get { return this.gameObject.name; } }
        public void DispatchEvent()
        {
            action.Invoke();
        }
    }
}