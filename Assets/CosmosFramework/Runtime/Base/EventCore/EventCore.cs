using System;
using System.Collections.Generic;
namespace Cosmos
{
    /// <summary>
    /// 普通事件Core
    /// </summary>
    /// <typeparam name="TKey">key的类型</typeparam>
    /// <typeparam name="TValue">value的类型</typeparam>
    /// <typeparam name="TDerived">派生类的类型</typeparam>
    public class EventCore<TKey, TValue, TDerived> : Singleton<TDerived>
        where TDerived : EventCore<TKey, TValue, TDerived>, new()
    {
        Dictionary<TKey, List<Action<TValue>>> eventDict = new Dictionary<TKey, List<Action<TValue>>>();
        #region Sync
        public virtual void AddEvent(TKey key, Action<TValue> handler)
        {
            if (eventDict.ContainsKey(key))
                eventDict[key].Add(handler);
            else
            {
                List<Action<TValue>> handlerSet = new List<Action<TValue>>();
                handlerSet.Add(handler);
                eventDict.Add(key, handlerSet);
            }
        }
        public virtual void RemoveEvent(TKey key, Action<TValue> handler)
        {
            if (eventDict.ContainsKey(key))
            {
                var handlerSet = eventDict[key];
                handlerSet.Remove(handler);
                if (handlerSet.Count <= 0)
                    eventDict.Remove(key);
            }
        }
        public bool HasEvent(TKey key)
        {
            return eventDict.ContainsKey(key);
        }
        public void Dispatch(TKey key, TValue value)
        {
            if (eventDict.ContainsKey(key))
            {
                var handlerSet = eventDict[key];
                int length = handlerSet.Count;
                for (int i = 0; i < length; i++)
                {
                    handlerSet[i]?.Invoke(value);
                }
            }
        }
        public void Dispatch(TKey key)
        {
            Dispatch(key, default);
        }
        #endregion
    }
}