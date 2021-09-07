using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 标准事件模型；
    /// 线程安全事件Core；
    /// 此类型的事件可以与EventManager并行使用；
    /// EventManager为全局事件，当前此类事件可以是区域事件；
    /// </summary>
    /// <typeparam name="TKey">key的类型</typeparam>
    /// <typeparam name="TValueA">valueA的类型</typeparam>
    /// <typeparam name="TValueB">valueB的类型</typeparam>
    /// <typeparam name="TDerived">派生类的类型</typeparam>
    public class ConcurrentStandardEventCore<TKey, TValueA, TValueB, TDerived> : ConcurrentSingleton<TDerived>
                where TDerived : ConcurrentStandardEventCore<TKey, TValueA, TValueB, TDerived>, new()
        where TValueB : class
    {
        protected ConcurrentDictionary<TKey, List<Action<TValueA, TValueB>>> eventDict = new ConcurrentDictionary<TKey, List<Action<TValueA, TValueB>>>();
        #region Sync
        public virtual void AddEventListener(TKey key, Action<TValueA, TValueB> handler)
        {
            if (eventDict.ContainsKey(key))
                eventDict[key].Add(handler);
            else
            {
                List<Action<TValueA, TValueB>> handlerSet = new List<Action<TValueA, TValueB>>();
                handlerSet.Add(handler);
                eventDict.TryAdd(key, handlerSet);
            }
        }
        public virtual void RemoveEventListener(TKey key, Action<TValueA, TValueB> handler)
        {
            if (eventDict.ContainsKey(key))
            {
                var handlerSet = eventDict[key];
                handlerSet.Remove(handler);
                if (handlerSet.Count <= 0)
                    eventDict.TryRemove(key, out _);
            }
        }
        public bool HasEventListened(TKey key)
        {
            return eventDict.ContainsKey(key);
        }
        public void Dispatch(TKey key, TValueA valueA, TValueB valueB)
        {
            if (eventDict.ContainsKey(key))
            {
                var handlerSet = eventDict[key];
                int length = handlerSet.Count;
                for (int i = 0; i < length; i++)
                {
                    handlerSet[i]?.Invoke(valueA, valueB);
                }
            }
        }
        public void Dispatch(TKey key, TValueA valueA)
        {
            Dispatch(key, valueA, null);
        }
        #endregion
        #region Async
        public async virtual Task AddEventListenerAsync(TKey key, Action<TValueA, TValueB> handler)
        {
            await Task.Run(() =>
            {
                if (eventDict.ContainsKey(key))
                    eventDict[key].Add(handler);
                else
                {
                    List<Action<TValueA, TValueB>> handlerSet = new List<Action<TValueA, TValueB>>();
                    handlerSet.Add(handler);
                    eventDict.TryAdd(key, handlerSet);
                }
            });
        }
        public async virtual Task RemoveEventListenerAsyncc(TKey key, Action<TValueA, TValueB> handler)
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
        public async Task DispatchAsync(TKey key, TValueA valueA, TValueB valueB)
        {
            await Task.Run(() =>
            {
                if (eventDict.ContainsKey(key))
                {
                    var handlerSet = eventDict[key];
                    int length = handlerSet.Count;
                    for (int i = 0; i < length; i++)
                    {
                        handlerSet[i]?.Invoke(valueA, valueB);
                    }
                }
            });
        }
        public async Task DispatchAsync(TKey key, TValueA valueA)
        {
            await DispatchAsync(key, valueA, null);
        }
        #endregion
    }
}
