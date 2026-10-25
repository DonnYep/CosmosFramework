using System;
using System.Collections.Generic;
using Cosmos.Reactive;
namespace Cosmos.Event
{
    //================================================
    /*
     * 1、事件中心（reactive）。使用数据类型作为事件的 key。
     * 2、每个事件类型对应一个 Subject<T>：订阅返回 IDisposable，退订即释放。
     * 3、Observe<T>() 暴露事件流，可接 Where / Select / DistinctUntilChanged 等操作符。
     */
    //================================================
    [Module(Priority = ModuleConstant.PRIORITY_EVENT)]
    internal sealed class EventManager : Module, IEventManager
    {
        /// <summary>
        /// 事件类型 === Subject&lt;T&gt;；
        /// </summary>
        readonly Dictionary<Type, object> subjectDict = new Dictionary<Type, object>();
        ///<inheritdoc/>
        public int EventCount { get { return subjectDict.Count; } }
        ///<inheritdoc/>
        public IDisposable Subscribe<T>(Action<T> handler)
            where T : GameEventArgs
        {
            if (handler == null)
                throw new ArgumentNullException("Handler is invalid !");
            return GetSubject<T>().Subscribe(handler);
        }
        ///<inheritdoc/>
        public void Publish<T>(T args)
            where T : GameEventArgs
        {
            if (args == null)
                throw new ArgumentNullException("EventArgs is invalid !");
            if (subjectDict.TryGetValue(typeof(T), out var subject))
                ((Subject<T>)subject).OnNext(args);
        }
        ///<inheritdoc/>
        public IObservable<T> Observe<T>()
            where T : GameEventArgs
        {
            return GetSubject<T>();
        }
        ///<inheritdoc/>
        public bool HasEvent<T>()
            where T : GameEventArgs
        {
            return subjectDict.TryGetValue(typeof(T), out var subject) && ((Subject<T>)subject).ObserverCount > 0;
        }
        ///<inheritdoc/>
        public EventInfo GetEventInfo<T>()
            where T : GameEventArgs
        {
            if (subjectDict.TryGetValue(typeof(T), out var subject))
                return EventInfo.Create(typeof(T), ((Subject<T>)subject).ObserverCount);
            return EventInfo.Default;
        }
        ///<inheritdoc/>
        public void RemoveAllEvents<T>()
            where T : GameEventArgs
        {
            if (subjectDict.Remove(typeof(T), out var subject))
                ((Subject<T>)subject).Dispose();
        }
        ///<inheritdoc/>
        public void RemoveAllEvents()
        {
            foreach (var subject in subjectDict.Values)
            {
                (subject as IDisposable)?.Dispose();
            }
            subjectDict.Clear();
        }
        Subject<T> GetSubject<T>()
            where T : GameEventArgs
        {
            if (!subjectDict.TryGetValue(typeof(T), out var subject))
            {
                subject = new Subject<T>();
                subjectDict.Add(typeof(T), subject);
            }
            return (Subject<T>)subject;
        }
    }
}
