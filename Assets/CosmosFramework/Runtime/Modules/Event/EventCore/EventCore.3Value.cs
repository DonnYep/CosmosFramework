using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos
{
    public class EventCore<TKey, TValueA, TValueB, TValueC,TDerived> : Singleton<TDerived>
            where TDerived : EventCore<TKey, TValueA, TValueB,TValueC, TDerived>, new()
    {
        protected Dictionary<TKey, List<Action<TValueA, TValueB,TValueC>>> eventDict = new Dictionary<TKey, List<Action<TValueA, TValueB, TValueC>>>();
        #region Sync
        public virtual void AddEventListener(TKey key, Action<TValueA, TValueB,TValueC> handler)
        {
            if (eventDict.ContainsKey(key))
                eventDict[key].Add(handler);
            else
            {
                List<Action<TValueA, TValueB, TValueC>> handlerSet = new List<Action<TValueA, TValueB, TValueC>>();
                handlerSet.Add(handler);
                eventDict.Add(key, handlerSet);
            }
        }
        public virtual void RemoveEventListener(TKey key, Action<TValueA, TValueB, TValueC> handler)
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
        public void Dispatch(TKey key, TValueA valueA, TValueB valueB,TValueC valueC)
        {
            if (eventDict.ContainsKey(key))
            {
                var handlerSet = eventDict[key];
                int length = handlerSet.Count;
                for (int i = 0; i < length; i++)
                {
                    handlerSet[i]?.Invoke(valueA, valueB,valueC);
                }
            }
        }
        public void Dispatch(TKey key, TValueA valueA)
        {
            Dispatch(key, valueA, default,default);
        }
        public void Dispatch(TKey key, TValueA valueA,TValueB  valueB)
        {
            Dispatch(key, valueA, valueB,default);
        }
        #endregion
        #region Async
        public async virtual Task AddEventListenerAsync(TKey key, Action<TValueA, TValueB,TValueC> handler)
        {
            await Task.Run(() =>
            {
                if (eventDict.ContainsKey(key))
                    eventDict[key].Add(handler);
                else
                {
                    List<Action<TValueA, TValueB,TValueC>> handlerSet = new List<Action<TValueA, TValueB, TValueC>>();
                    handlerSet.Add(handler);
                    eventDict.Add(key, handlerSet);
                }
            });
        }
        public async virtual Task RemoveEventListenerAsyncc(TKey key, Action<TValueA, TValueB,TValueC> handler)
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
        public async Task DispatchAsync(TKey key, TValueA valueA, TValueB valueB,TValueC valueC)
        {
            await Task.Run(() =>
            {
                if (eventDict.ContainsKey(key))
                {
                    var handlerSet = eventDict[key];
                    int length = handlerSet.Count;
                    for (int i = 0; i < length; i++)
                    {
                        handlerSet[i]?.Invoke(valueA, valueB,valueC);
                    }
                }
            });
        }
        public async Task DispatchAsync(TKey key, TValueA valueA)
        {
            await DispatchAsync(key, valueA, default,default);
        }
        public async Task DispatchAsync(TKey key, TValueA valueA,TValueB valueB)
        {
            await DispatchAsync(key, valueA, valueB, default);
        }
        #endregion
    }
}
