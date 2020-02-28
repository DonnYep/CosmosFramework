using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
namespace Cosmos.Event
{
    public sealed class EventManager : Module<EventManager>
    {
        Dictionary<string, CFAction<object, GameEventArgs>> eventMap = new Dictionary<string, CFAction<object, GameEventArgs>>();
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="eventKey">事件的key，可以是对象，字符</param>
        /// <param name="handler">事件处理者</param>
        /// <param name="callBack">只有事件注册成功才执行回调函数</param>
        public void AddListener(string eventKey,CFAction<object,GameEventArgs> handler)
        {
            if(eventMap.ContainsKey(eventKey))
            {
                eventMap[eventKey] += handler;
            }
            else
            {
                eventMap.Add(eventKey,null);
                eventMap[eventKey] += handler;
            }
        }
        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="eventKey">事件的key，可以是对象，字符</param>
        /// <param name="hander">事件处理者</param>
        public void RemoveListener(string eventKey,CFAction<object,GameEventArgs> hander)
        {
            if (eventMap.ContainsKey(eventKey))
            {
                eventMap[eventKey] -= hander;
            }
        }
        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="sender">事件的触发者</param>
        /// <param name="arg">事件处理类</param>
        public void DispatchEvent(string eventKey,object sender, GameEventArgs arg)
        {
            if (eventMap.ContainsKey(eventKey))
            {
                if (eventMap[eventKey] != null)
                {
                    eventMap[eventKey](sender,arg);
                }
            }
            else
                Utility.DebugError("EventManager\n"+"Event:" +eventKey+ "\n is unregistered");
        }
        /// <summary>
        /// 注销并移除事件
        /// </summary>
        public void DeregisterEvent(string eventKey)
        {
            if (eventMap.ContainsKey(eventKey))
            {
                eventMap[eventKey] = null;
                eventMap.Remove(eventKey);
            }
        }
        /// <summary>
        /// 在事件中心注册一个空的事件
        /// 当前设计是为事件的触发者设计，空事件可以使其他订阅者订阅
        /// </summary>
        public void RegisterEvent(string eventKey)
        {
            if (!eventMap.ContainsKey(eventKey))
            {
                eventMap.Add(eventKey, null);
            }
        }
        /// <summary>
        /// 清空已经注册的事件
        /// </summary>
        public void ClearEvent(string eventKey)
        {
            if (eventMap.ContainsKey(eventKey))
            {
                eventMap[eventKey] = null;
            }
        }
        /// <summary>
        /// 注销所有除框架以外的事件
        /// </summary>
        public void ClearAllEvent()
        {
            foreach (var key in eventMap.Keys)
            {
                if (IsSystemEvent(key))
                {

                }
            }
        }
        //判断是否是CF框架中的模块事件
        bool IsSystemEvent(object key)
        {
            return false;
        }
        /// <summary>
        /// 判断时间是否注册
        /// </summary>
        public bool IsEventRegistered(string eventKey)
        {
            if (eventMap.ContainsKey(eventKey))
                return true;
            else
                return false;
        }
    }
}