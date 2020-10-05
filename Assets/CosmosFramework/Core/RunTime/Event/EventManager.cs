using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using Object = UnityEngine.Object;

namespace Cosmos.Event
{
    [Module]
    internal sealed class EventManager : Module<EventManager>
    {
#if SERVER
        /// <summary>
        /// 支持并发的字典
        /// </summary>
        ConcurrentDictionary<string, Action<object, GameEventArgs>> concurrentEventDict
            = new ConcurrentDictionary<string, Action<object, GameEventArgs>>();
#else
        Dictionary<string, Action<object, GameEventArgs>> concurrentEventDict = new Dictionary<string, Action<object, GameEventArgs>>();
#endif
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="eventKey">事件的key，可以是对象，字符</param>
        /// <param name="handler">事件处理者</param>
        /// <param name="callBack">只有事件注册成功才执行回调函数</param>
        internal void AddListener(string eventKey, Action<object, GameEventArgs> handler)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                Utility.Debug.LogInfo("Event key is  empty", MessageColor.RED);
                return;
            }
            if (concurrentEventDict.ContainsKey(eventKey))
            {
                concurrentEventDict[eventKey] += handler;
            }
            else
            {
                concurrentEventDict.TryAdd(eventKey, null);
                concurrentEventDict[eventKey] += handler;
            }
        }
        /// <summary>
        /// 移除事件，假如事件在字典中已经为空，则自动注销，无需手动
        /// </summary>
        /// <param name="eventKey">事件的key，可以是对象，字符</param>
        /// <param name="hander">事件处理者</param>
        internal void RemoveListener(string eventKey, Action<object, GameEventArgs> hander)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                Utility.Debug.LogInfo("Event key is  empty", MessageColor.RED);
                return;
            }
            if (concurrentEventDict.ContainsKey(eventKey))
            {
                concurrentEventDict[eventKey] -= hander;
                if (concurrentEventDict[eventKey] == null)
                {
#if SERVER
                    Action<object, GameEventArgs> handler;
                    concurrentEventDict.TryRemove(eventKey, out handler);
                    handler = null;
#else
                    concurrentEventDict.Remove(eventKey);
#endif
                }
            }
        }
        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="sender">事件的触发者</param>
        /// <param name="args">事件处理类</param>
        internal void DispatchEvent(string eventKey, object sender, GameEventArgs args)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                Utility.Debug.LogInfo("Event key is  empty", MessageColor.MAROON);
                return;
            }
            if (concurrentEventDict.ContainsKey(eventKey))
            {
                if (concurrentEventDict[eventKey] != null)
                {
                    concurrentEventDict[eventKey](sender, args);
                }
            }
            else
                Utility.Debug.LogInfo("EventManager  " + "Event:" + eventKey + " has not  registered", MessageColor.RED);
        }
        /// <summary>
        /// 注销并移除事件
        /// </summary>
        internal void DeregisterEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                Utility.Debug.LogInfo("Event key is  empty", MessageColor.RED);
                return;
            }
            if (concurrentEventDict.ContainsKey(eventKey))
            {
                concurrentEventDict[eventKey] = null;
#if SERVER
                Action<object, GameEventArgs> handler;
                concurrentEventDict.TryRemove(eventKey, out handler);
                handler = null;
#else
                concurrentEventDict.Remove(eventKey);
#endif
            }
        }
        /// <summary>
        /// 在事件中心注册一个空的事件
        /// 当前设计是为事件的触发者设计，空事件可以使其他订阅者订阅
        /// </summary>
        internal void RegisterEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                Utility.Debug.LogInfo("Event key is  empty", MessageColor.RED);
                return;
            }
            if (!concurrentEventDict.ContainsKey(eventKey))
            {
                concurrentEventDict.TryAdd(eventKey, null);
            }
        }
        /// <summary>
        /// 清空已经注册的事件
        /// </summary>
        internal void ClearEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                Utility.Debug.LogInfo("Event key is  empty", MessageColor.RED);
                return;
            }
            if (concurrentEventDict.ContainsKey(eventKey))
            {
                concurrentEventDict[eventKey] = null;
            }
        }
        /// <summary>
        /// 注销所有除框架以外的事件
        /// </summary>
        internal void ClearAllEvent()
        {
            foreach (var key in concurrentEventDict.Keys)
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
        /// 判断事件是否注册
        /// </summary>
        internal bool HasEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                Utility.Debug.LogInfo("Event key is  empty", MessageColor.RED);
                return false;
            }
            if (concurrentEventDict.ContainsKey(eventKey))
                return true;
            else
                return false;
        }
    }
}