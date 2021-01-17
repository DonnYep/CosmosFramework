using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    /// <summary>
    /// 普通事件Core
    /// 此类型的事件可以与EventManager并行使用；
    /// EventManager为全局事件，当前此类事件可以是区域事件；
    /// </summary>
    /// <typeparam name="TKey">key的类型</typeparam>
    /// <typeparam name="TValue">value的类型</typeparam>
    /// <typeparam name="TDerived">派生类的类型</typeparam>
    public class EventCore<TKey, TValue, TDerived> : Singleton<TDerived>
        where TDerived : EventCore<TKey, TValue, TDerived>, new()
        where TValue : class
    {
        Dictionary<TKey, List<Action<TValue>>> eventDict = new Dictionary<TKey, List<Action<TValue>>>();
        #region Sync

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
        #endregion
        #region Async
        public async virtual Task AddEventListenerAsync(TKey key, Action<TValue> handler)
        {
            await Task.Run(() =>
            {
                if (eventDict.ContainsKey(key))
                    eventDict[key].Add(handler);
                else
                {
                    List<Action<TValue>> handlerSet = new List<Action<TValue>>();
                    handlerSet.Add(handler);
                    eventDict.Add(key, handlerSet);
                }
            });
        }
        public async virtual Task RemoveEventListenerAsyncc(TKey key, Action<TValue> handler)
        {
            await Task.Run(() =>
            {
                if (eventDict.ContainsKey(key))
                {
                    var handlerSet = eventDict[key];
                    handlerSet.Remove(handler);
                    if (handlerSet.Count <= 0)
                        eventDict.Remove(key);
                }
            });
        }
        public async Task<bool> HasEventListenedAsync(TKey key)
        {
            return await Task.Run<bool>(() => { return eventDict.ContainsKey(key); });
        }
        public async Task DispatchAsync(TKey key, TValue value)
        {
            await Task.Run(() =>
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
            });
        }
        public async Task DispatchAsync(TKey key)
        {
            await DispatchAsync(key, null);
        }
        #endregion
    }
}