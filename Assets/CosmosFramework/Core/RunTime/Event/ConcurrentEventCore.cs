using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    /// <summary>
    /// 线程安全事件Core；
    /// 此类型的事件可以与EventManager并行使用；
    /// EventManager为全局事件，当前此类事件可以是区域事件；
    /// </summary>
    /// <typeparam name="TKey">key的类型</typeparam>
    /// <typeparam name="TValue">value的类型</typeparam>
    /// <typeparam name="TDerived">派生类的类型</typeparam>
    public class ConcurrentEventCore<TKey, TValue, TDerived> : ConcurrentSingleton<TDerived>
        where TDerived : ConcurrentEventCore<TKey, TValue, TDerived>, new()
        where TValue : class
    {
        Dictionary<TKey, List<Action<TValue>>> eventDict = new Dictionary<TKey, List<Action<TValue>>>();
        public virtual void AddEventListener(TKey key, Action<TValue> handler)
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
        public virtual void RemoveEventListener(TKey key, Action<TValue> handler)
        {
            if (eventDict.ContainsKey(key))
            {
                var handlerSet = eventDict[key];
                handlerSet.Remove(handler);
                if (handlerSet.Count <= 0)
                    eventDict.Remove(key);
            }
        }
        public bool HasEventListened(TKey key)
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
            Dispatch(key, null);
        }
    }
}
