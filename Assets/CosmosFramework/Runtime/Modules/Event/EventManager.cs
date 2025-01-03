using System.Collections.Generic;
using System;
namespace Cosmos.Event
{
    //================================================
    /*
     * 1、事件中心。使用数据作为事件的key。
     */
    //================================================
    [Module]
    internal sealed partial class EventManager : Module, IEventManager
    {
        readonly Dictionary<string, EventNodeBase> eventDict = new Dictionary<string, EventNodeBase>();
        ///<inheritdoc/>
        public int EventCount { get { return eventDict.Count; } }
        public void AddEvent<T>(Action<T> handler)
            where T : GameEventArgs
        {
            var eventKey = typeof(T).FullName;
            EventNode<T> node = default;
            if (!eventDict.TryGetValue(eventKey, out var nodeBase))
            {
                node = new EventNode<T>();
                nodeBase = node;
                eventDict.TryAdd(eventKey, nodeBase);
            }
            node = nodeBase as EventNode<T>;
            node.EventHandler += handler;
        }
        ///<inheritdoc/>
        public void RemoveEvent<T>(Action<T> handler)
            where T : GameEventArgs
        {
            var eventKey = typeof(T).FullName;
            if (eventDict.TryGetValue(eventKey, out var nodeBase))
            {
                var node = nodeBase as EventNode<T>;
                node.EventHandler -= handler;
                if (node.ListenerCount <= 0)
                {
                    eventDict.Remove(eventKey);
                }
            }
        }
        ///<inheritdoc/>
        public void RemoveAllEvents<T>()
            where T : GameEventArgs
        {
            var eventKey = typeof(T).FullName;
            if (eventDict.TryRemove(eventKey, out var nodeBase))
            {
                nodeBase.Clear();
            }
        }
        ///<inheritdoc/>
        public void RemoveAllEvents()
        {
            foreach (var node in eventDict)
            {
                node.Value.Clear();
            }
            eventDict.Clear();
        }
        ///<inheritdoc/>
        public void Dispatch<T>(T args)
            where T : GameEventArgs
        {
            var eventKey = typeof(T).FullName;
            if (eventDict.TryGetValue(eventKey, out var nodeBase))
            {
                var node = nodeBase as EventNode<T>;
                node.Dispatch(args);
            }
        }
        ///<inheritdoc/>
        public bool HasEvent<T>()
            where T : GameEventArgs
        {
            var eventKey = typeof(T).FullName;
            return eventDict.ContainsKey(eventKey);
        }
        ///<inheritdoc/>
        public EventInfo GetEventInfo<T>()
            where T : GameEventArgs
        {
            var eventKey = typeof(T).FullName;
            if (eventDict.TryGetValue(eventKey, out var node))
            {
                return EventInfo.Create(eventKey, node.ListenerCount);
            }
            return EventInfo.Default;
        }
    }
}