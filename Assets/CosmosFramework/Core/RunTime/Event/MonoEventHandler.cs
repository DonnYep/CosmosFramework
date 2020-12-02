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
        /// <summary>
        /// 默认的标准事件处理者，
        /// 空的虚函数
        /// </summary>
        protected virtual void EventHandler(object sender, GameEventArgs args) { }
        protected void DispatchEvent(string eventKey,GameEventArgs args)
        {
            GameManager.GetModule<IEventManager>().DispatchEvent(eventKey, this, args);
        }
        /// <summary>
        /// 注册事件，默认将EventHandler注册到事件中心
        /// 如果需要注册其他事件，则移步AddEventListener
        /// </summary>
        protected void AddDefaultEventListener(string eventKey)
        {
            GameManager.GetModule<IEventManager>().AddListener(eventKey, EventHandler);
        }
        protected void AddEventListener(string eventKey,EventHandler<GameEventArgs> handler)
        {
            GameManager.GetModule<IEventManager>().AddListener(eventKey, handler);
        }
        /// <summary>
        /// 注销事件，默认将EventHandler从事件中心注销
        /// 如果需要注销其他事件，则移步RemoveEventListener
        /// </summary>
        protected void RemoveDefaultEventListener(string eventKey)
        {
            GameManager.GetModule<IEventManager>().RemoveListener(eventKey, EventHandler);
        }
        protected void RemoveEventListener(string eventKey,EventHandler<GameEventArgs>handler)
        {
            GameManager.GetModule<IEventManager>().RemoveListener(eventKey, handler);
        }
    }
}