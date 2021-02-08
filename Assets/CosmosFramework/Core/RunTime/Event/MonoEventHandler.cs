using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cosmos.Event;
namespace Cosmos
{
    /// <summary>
    /// 继承自Mono下的事件处理基类
    /// 此类分装了事件派发的方法，派生类调用即可
    /// </summary>
    public abstract class MonoEventHandler : MonoBehaviour
    {
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void Awake() { }
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void OnDestroy() { }
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void OnValidate() { }
        protected void DispatchEvent(string eventKey,GameEventArgs args)
        {
            GameManager.GetModule<IEventManager>().DispatchEvent(eventKey, this, args);
        }
        protected void AddEventListener(string eventKey,EventHandler<GameEventArgs> handler)
        {
            GameManager.GetModule<IEventManager>().AddListener(eventKey, handler);
        }
        protected void RemoveEventListener(string eventKey,EventHandler<GameEventArgs>handler)
        {
            GameManager.GetModule<IEventManager>().RemoveListener(eventKey, handler);
        }
    }
}