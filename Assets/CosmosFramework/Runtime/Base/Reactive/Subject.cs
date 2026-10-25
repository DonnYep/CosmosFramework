using System;
using System.Collections.Generic;
namespace Cosmos.Reactive
{
    /// <summary>
    /// 多播主体：既是可观察序列，也是观察者；
    /// <para>派发时对已退订的观察者做惰性跳过，允许在回调里订阅 / 退订其他订阅。</para>
    /// <para>线程约定：主线程；派发过程中抛出的异常会向调用方传播（与普通委托事件一致）。</para>
    /// </summary>
    public sealed class Subject<T> : Observable<T>, IObserver<T>, IDisposable
    {
        sealed class Subscription : IDisposable
        {
            public readonly IObserver<T> Observer;
            public bool Active = true;
            public Subscription(IObserver<T> observer)
            {
                Observer = observer;
            }
            public void Dispose()
            {
                Active = false;
            }
        }

        readonly List<Subscription> subscriptions = new List<Subscription>();
        bool disposed;
        bool completed;
        Exception error;

        /// <summary>
        /// 是否已释放。
        /// </summary>
        public bool IsDisposed { get { return disposed; } }
        /// <summary>
        /// 是否已完成（含出错）。
        /// </summary>
        public bool IsCompleted { get { return completed; } }
        /// <summary>
        /// 当前有效订阅数量。
        /// </summary>
        public int ObserverCount
        {
            get
            {
                var count = 0;
                for (int i = 0; i < subscriptions.Count; i++)
                {
                    if (subscriptions[i].Active)
                        count++;
                }
                return count;
            }
        }
        /// <inheritdoc/>
        public override IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("Observer is invalid !");
            if (disposed)
                return Disposable.Empty;
            if (completed)
            {
                //已结束的序列：新订阅立即收到结束信号，避免永久等待
                if (error != null)
                    observer.OnError(error);
                else
                    observer.OnCompleted();
                return Disposable.Empty;
            }
            var subscription = new Subscription(observer);
            subscriptions.Add(subscription);
            return subscription;
        }
        /// <summary>
        /// 派发一个值。
        /// </summary>
        public void OnNext(T value)
        {
            if (disposed || completed)
                return;
            var count = subscriptions.Count;
            var hasInactive = false;
            for (int i = 0; i < count; i++)
            {
                var subscription = subscriptions[i];
                if (!subscription.Active)
                {
                    hasInactive = true;
                    continue;
                }
                subscription.Observer.OnNext(value);
            }
            if (hasInactive)
                Compact();
        }
        /// <summary>
        /// 派发错误；订阅者收到后序列结束。
        /// </summary>
        public void OnError(Exception exception)
        {
            if (disposed || completed)
                return;
            error = exception;
            completed = true;
            var count = subscriptions.Count;
            for (int i = 0; i < count; i++)
            {
                var subscription = subscriptions[i];
                if (subscription.Active)
                    subscription.Observer.OnError(exception);
            }
            subscriptions.Clear();
        }
        /// <summary>
        /// 完成序列；订阅者收到完成信号后序列结束。
        /// </summary>
        public void OnCompleted()
        {
            if (disposed || completed)
                return;
            completed = true;
            var count = subscriptions.Count;
            for (int i = 0; i < count; i++)
            {
                var subscription = subscriptions[i];
                if (subscription.Active)
                    subscription.Observer.OnCompleted();
            }
            subscriptions.Clear();
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            subscriptions.Clear();
        }
        void Compact()
        {
            var writeIndex = 0;
            for (int readIndex = 0; readIndex < subscriptions.Count; readIndex++)
            {
                var subscription = subscriptions[readIndex];
                if (!subscription.Active)
                    continue;
                subscriptions[writeIndex++] = subscription;
            }
            if (writeIndex < subscriptions.Count)
                subscriptions.RemoveRange(writeIndex, subscriptions.Count - writeIndex);
        }
    }
}
