using System;
namespace Cosmos.Reactive
{
    /// <summary>
    /// 可观察序列基类；
    /// <para>极简 reactive 基座：<see cref="Subject{T}"/> 负责多播、<see cref="ReactiveProperty{T}"/> 负责状态，
    /// 操作符（Where / Select / DistinctUntilChanged）通过扩展方法链式组合。</para>
    /// <para>线程约定：主线程派发；订阅返回 <see cref="IDisposable"/>，退订即释放。</para>
    /// </summary>
    public abstract class Observable<T> : IObservable<T>
    {
        /// <summary>
        /// 订阅。
        /// </summary>
        /// <param name="observer">观察者</param>
        /// <returns>退订句柄</returns>
        public abstract IDisposable Subscribe(IObserver<T> observer);
        /// <summary>
        /// 订阅（只关心值）。
        /// </summary>
        public IDisposable Subscribe(Action<T> onNext)
        {
            return Subscribe(onNext, null, null);
        }
        /// <summary>
        /// 订阅（值 + 错误）。
        /// </summary>
        public IDisposable Subscribe(Action<T> onNext, Action<Exception> onError)
        {
            return Subscribe(onNext, onError, null);
        }
        /// <summary>
        /// 订阅（值 + 错误 + 完成）。
        /// </summary>
        public IDisposable Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            if (onNext == null)
                throw new ArgumentNullException("OnNext handler is invalid !");
            return Subscribe(new AnonymousObserver<T>(onNext, onError, onCompleted));
        }
    }
    /// <summary>
    /// 由委托构造的可观察序列；用于实现自定义流 / 操作符。
    /// </summary>
    public sealed class AnonymousObservable<T> : Observable<T>
    {
        readonly Func<IObserver<T>, IDisposable> subscribe;
        public AnonymousObservable(Func<IObserver<T>, IDisposable> subscribe)
        {
            if (subscribe == null)
                throw new ArgumentNullException("Subscribe handler is invalid !");
            this.subscribe = subscribe;
        }
        /// <inheritdoc/>
        public override IDisposable Subscribe(IObserver<T> observer)
        {
            return subscribe(observer);
        }
    }
}
