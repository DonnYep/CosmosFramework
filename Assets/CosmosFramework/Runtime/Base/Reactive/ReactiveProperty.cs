using System;
using System.Collections.Generic;
namespace Cosmos.Reactive
{
    /// <summary>
    /// 只读响应式属性。
    /// </summary>
    public interface IReadOnlyReactiveProperty<T> : IObservable<T>
    {
        /// <summary>
        /// 当前值。
        /// </summary>
        T Value { get; }
    }
    /// <summary>
    /// 响应式属性：持有当前值，值变化时向订阅者推送；
    /// <para>订阅时会立即收到当前值，因此订阅者不需要额外读取一次。</para>
    /// <para>默认用 <see cref="EqualityComparer{T}.Default"/> 去重，相同值不触发推送。</para>
    /// </summary>
    public sealed class ReactiveProperty<T> : Observable<T>, IReadOnlyReactiveProperty<T>, IDisposable
    {
        readonly Subject<T> subject = new Subject<T>();
        readonly IEqualityComparer<T> comparer;
        T value;
        bool disposed;
        public ReactiveProperty() : this(default(T), null) { }
        public ReactiveProperty(T initialValue) : this(initialValue, null) { }
        public ReactiveProperty(T initialValue, IEqualityComparer<T> equalityComparer)
        {
            value = initialValue;
            comparer = equalityComparer ?? EqualityComparer<T>.Default;
        }
        /// <summary>
        /// 当前值；赋值时若与旧值不同则推送。
        /// </summary>
        public T Value
        {
            get { return value; }
            set
            {
                if (comparer.Equals(this.value, value))
                    return;
                this.value = value;
                subject.OnNext(value);
            }
        }
        /// <summary>
        /// 是否已释放。
        /// </summary>
        public bool IsDisposed { get { return disposed; } }
        /// <summary>
        /// 设置值但不推送（用于初始化 / 静默同步）。
        /// </summary>
        public void SetValueWithoutNotify(T newValue)
        {
            value = newValue;
        }
        /// <summary>
        /// 订阅：先注册，再立即推送当前值，避免漏掉注册瞬间的变化。
        /// </summary>
        public override IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("Observer is invalid !");
            var subscription = subject.Subscribe(observer);
            if (!disposed)
                observer.OnNext(value);
            return subscription;
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            subject.Dispose();
        }
    }
}
