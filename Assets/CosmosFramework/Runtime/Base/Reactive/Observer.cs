using System;
namespace Cosmos.Reactive
{
    /// <summary>
    /// 由委托构成的观察者；用于把 <see cref="Observable{T}.Subscribe(Action{T})"/> 适配成 <see cref="IObserver{T}"/>。
    /// </summary>
    internal sealed class AnonymousObserver<T> : IObserver<T>
    {
        readonly Action<T> onNext;
        readonly Action<Exception> onError;
        readonly Action onCompleted;
        public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }
        public void OnNext(T value)
        {
            onNext?.Invoke(value);
        }
        public void OnError(Exception error)
        {
            onError?.Invoke(error);
        }
        public void OnCompleted()
        {
            onCompleted?.Invoke();
        }
    }
    /// <summary>
    /// 转发观察者；操作符内部把收到的值按规则处理后转交给下游。
    /// </summary>
    internal sealed class ForwardObserver<T> : IObserver<T>
    {
        readonly Action<T> onNext;
        readonly Action<Exception> onError;
        readonly Action onCompleted;
        public ForwardObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }
        public void OnNext(T value)
        {
            onNext?.Invoke(value);
        }
        public void OnError(Exception error)
        {
            onError?.Invoke(error);
        }
        public void OnCompleted()
        {
            onCompleted?.Invoke();
        }
    }
}
