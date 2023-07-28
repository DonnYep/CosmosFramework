using System.Collections.Generic;
using System;
namespace Cosmos.Event
{
    //================================================
    /*
     * 1、事件中心；
     */
    //================================================
    [Module]
    internal sealed partial class EventManager : Module, IEventManager
    {
        Dictionary<string, EventNode> eventDict;
        ///<inheritdoc/>
        public int EventCount { get { return eventDict.Count; } }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public void RemoveListener(string eventKey, EventHandler<GameEventArgs> handler)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (eventDict.TryGetValue(eventKey, out var node))
            {
                node.EventHandler -= handler;
                if (node.ListenerCount <= 0)
                {
                    eventDict.Remove(eventKey);
                }
            }
        }
        ///<inheritdoc/>
        public void DispatchEvent(string eventKey, object sender, GameEventArgs args)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (eventDict.TryGetValue(eventKey, out var node))
            {
                eventDict[eventKey].DispatchEvent(sender, args);
            }
        }
        ///<inheritdoc/>
        public bool DeregisterEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            return eventDict.Remove(eventKey);
        }
        ///<inheritdoc/>
        public void RegisterEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (!eventDict.ContainsKey(eventKey))
            {
                eventDict.TryAdd(eventKey, new EventNode());
            }
        }
        ///<inheritdoc/>
        public void ClearEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            if (eventDict.TryGetValue(eventKey, out var node))
            {
                node.Clear();
            }
        }
        ///<inheritdoc/>
        public void ClearAllEvent()
        {
            foreach (var node in eventDict)
            {
                node.Value.Clear();
            }
            eventDict.Clear();
        }
        ///<inheritdoc/>
        public bool HasEvent(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                throw new ArgumentNullException("EventKey is invalid !");
            return eventDict.ContainsKey(eventKey);
        }
        ///<inheritdoc/>
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