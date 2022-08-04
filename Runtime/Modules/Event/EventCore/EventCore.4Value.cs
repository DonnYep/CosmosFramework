using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmos
{
    public class EventCore<TKey, TValueA, TValueB, TValueC, TValueD, TDerived> : Singleton<TDerived>
            where TDerived : EventCore<TKey, TValueA, TValueB, TValueC, TValueD, TDerived>, new()
    {
        protected Dictionary<TKey, List<Action<TValueA, TValueB, TValueC, TValueD>>> eventDict = new Dictionary<TKey, List<Action<TValueA, TValueB, TValueC, TValueD>>>();
        #region Sync
        public virtual void AddEventListener(TKey key, Action<TValueA, TValueB, TValueC, TValueD> handler)
        {
            if (eventDict.ContainsKey(key))
                eventDict[key].Add(handler);
            else
            {
                List<Action<TValueA, TValueB, TValueC, TValueD>> handlerSet = new List<Action<TValueA, TValueB, TValueC, TValueD>>();
                handlerSet.Add(handler);
                eventDict.Add(key, handlerSet);
            }
        }
        public virtual void RemoveEventListener(TKey key, Action<TValueA, TValueB, TValueC, TValueD> handler)
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
        public void Dispatch(TKey key, TValueA valueA, TValueB valueB, TValueC valueC, TValueD valueD)
        {
            if (eventDict.ContainsKey(key))
            {
                var handlerSet = eventDict[key];
                int length = handlerSet.Count;
                for (int i = 0; i < length; i++)
                {
                    handlerSet[i]?.Invoke(valueA, valueB, valueC, valueD);
                }
            }
        }
        public void Dispatch(TKey key, TValueA valueA)
        {
            Dispatch(key, valueA, default, default, default);
        }
        public void Dispatch(TKey key, TValueA valueA, TValueB valueB)
        {
            Dispatch(key, valueA, valueB, default, default);
        }
        public void Dispatch(TKey key, TValueA valueA, TValueB valueB, TValueC valueC)
        {
            Dispatch(key, valueA, valueB, valueC, default);
        }
        #endregion
        #region Async
        public async virtual Task AddEventListenerAsync(TKey key, Action<TValueA, TValueB, TValueC, TValueD> handler)
        {
            await Task.Run(() =>
            {
                if (eventDict.ContainsKey(key))
                    eventDict[key].Add(handler);
                else
                {
                    List<Action<TValueA, TValueB, TValueC, TValueD>> handlerSet = new List<Action<TValueA, TValueB, TValueC, TValueD>>();
                    handlerSet.Add(handler);
                    eventDict.Add(key, handlerSet);
                }
            });
        }
        public async virtual Task RemoveEventListenerAsyncc(TKey key, Action<TValueA, TValueB, TValueC, TValueD> handler)
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
        public async Task DispatchAsync(TKey key, TValueA valueA, TValueB valueB, TValueC valueC, TValueD valueD)
        {
            await Task.Run(() =>
            {
                if (eventDict.ContainsKey(key))
                {
                    var handlerSet = eventDict[key];
                    int length = handlerSet.Count;
                    for (int i = 0; i < length; i++)
                    {
                        handlerSet[i]?.Invoke(valueA, valueB, valueC, valueD);
                    }
                }
            });
        }
        public async Task DispatchAsync(TKey key, TValueA valueA)
        {
            await DispatchAsync(key, valueA, default, default, default);
        }
        public async Task DispatchAsync(TKey key, TValueA valueA, TValueB valueB)
        {
            await DispatchAsync(key, valueA, valueB, default, default);
        }
        public async Task DispatchAsync(TKey key, TValueA valueA, TValueB valueB, TValueC valueC)
        {
            await DispatchAsync(key, valueA, valueB, valueC, default);
        }
        #endregion
    }
}
