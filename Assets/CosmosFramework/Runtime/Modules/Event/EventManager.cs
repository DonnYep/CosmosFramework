using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
namespace Cosmos.Event
{
    [Module]
    internal sealed partial class EventManager : Module, IEventManager
    {
        Dictionary<string, EventNode> eventDict;
        public int EventCount { get { return eventDict.Count; } }
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="eventKey">事件的key，可以是对象，字符</param>
        /// <param name="handler">事件处理者</param>
        public void AddListener(string eventKey, EventHandler<GameEventArgs> handler)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (eventDict.ContainsKey(eventKey))
            {
                eventDict[eventKey].EventHandler += handler;
            }
            else
            {
                eventDict.TryAdd(eventKey, new EventNode());
                eventDict[eventKey].EventHandler += handler;
            }
        }
        /// <summary>
        /// 移除事件，假如事件在字典中已经为空，则自动注销，无需手动
        /// </summary>
        /// <param name="eventKey">事件的key，可以是对象，字符</param>
        /// <param name="hander">事件处理者</param>
        public void RemoveListener(string eventKey, EventHandler<GameEventArgs> hander)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (eventDict.TryGetValue(eventKey, out var node))
            {
                node.EventHandler -= hander;
                if (node.ListenerCount <= 0)
                {
                    eventDict.Remove(eventKey);
                }
            }
        }
        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="sender">事件的触发者</param>
        /// <param name="args">事件处理类</param>
        public void DispatchEvent(string eventKey, object sender, GameEventArgs args)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (eventDict.TryGetValue(eventKey, out var node))
            {
                eventDict[eventKey].DispatchEvent(sender, args);
            }
            else
                throw new ArgumentNullException($"EventKey {eventKey} has not  registered !");
        }
        /// <summary>
        /// 注销并移除事件
        /// </summary>
        public bool DeregisterEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            return eventDict.Remove(eventKey);
        }
        /// <summary>
        /// 在事件中心注册一个空的事件
        /// 当前设计是为事件的触发者设计，空事件可以使其他订阅者订阅
        /// </summary>
        public void RegisterEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (!eventDict.ContainsKey(eventKey))
            {
                eventDict.TryAdd(eventKey, new EventNode());
            }
        }
        /// <summary>
        /// 清空已经注册的事件
        /// </summary>
        public void ClearEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (eventDict.TryGetValue(eventKey, out var node))
            {
                node.Clear();
            }
        }
        /// <summary>
        /// 注销所有除框架以外的事件
        /// </summary>
        public void ClearAllEvent()
        {
            foreach (var node in eventDict)
            {
                node.Value.Clear();
            }
            eventDict.Clear();
        }
        /// <summary>
        /// 判断事件是否注册
        /// </summary>
        public bool HasEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            return eventDict.ContainsKey(eventKey);
        }
        /// <summary>
        /// 获取事件的信息；
        /// </summary>
        public EventInfo GetEventInfo(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (eventDict.TryGetValue(eventKey, out var node))
            {
                return EventInfo.Create(eventKey, node.ListenerCount);
            }
            return EventInfo.Default;
        }
        protected override void OnInitialization()
        {
            eventDict = new Dictionary<string, EventNode>();
        }
    }
}