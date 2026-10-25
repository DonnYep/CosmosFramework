using System;
using System.Collections.Generic;
namespace Cosmos.Reactive
{
    /// <summary>
    /// 可观察序列的扩展：订阅适配与极简操作符。
    /// <para>操作符返回新的 <see cref="Observable{T}"/>，可链式组合：
    /// <c>source.Where(x =&gt; x &gt; 0).Select(x =&gt; x * 2).Subscribe(OnValue)</c>。</para>
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// 订阅（只关心值）。
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return Subscribe(source, onNext, null, null);
        }
        /// <summary>
        /// 订阅（值 + 错误）。
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return Subscribe(source, onNext, onError, null);
        }
        /// <summary>
        /// 订阅（值 + 错误 + 完成）。
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            if (source == null)
                throw new ArgumentNullException("Source is invalid !");
            if (onNext == null)
                throw new ArgumentNullException("OnNext handler is invalid !");
            return source.Subscribe(new AnonymousObserver<T>(onNext, onError, onCompleted));
        }
        /// <summary>
        /// 过滤：只有满足条件的值会继续传递。
        /// </summary>
        public static Observable<T> Where<T>(this IObservable<T> source, Predicate<T> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("Source is invalid !");
            if (predicate == null)
                throw new ArgumentNullException("Predicate is invalid !");
            return new AnonymousObservable<T>(observer => source.Subscribe(new ForwardObserver<T>(
                value =>
                {
                    if (predicate(value))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted)));
        }
        /// <summary>
        /// 映射：把值转换成另一种类型。
        /// </summary>
        public static Observable<TResult> Select<T, TResult>(this IObservable<T> source, Func<T, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException("Source is invalid !");
            if (selector == null)
                throw new ArgumentNullException("Selector is invalid !");
            return new AnonymousObservable<TResult>(observer => source.Subscribe(new ForwardObserver<T>(
                value => observer.OnNext(selector(value)),
                observer.OnError,
                observer.OnCompleted)));
        }
        /// <summary>
        /// 去重：只有与上一个值不同的值会继续传递。
        /// </summary>
        public static Observable<T> DistinctUntilChanged<T>(this IObservable<T> source, IEqualityComparer<T> equalityComparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("Source is invalid !");
            var comparer = equalityComparer ?? EqualityComparer<T>.Default;
            return new AnonymousObservable<T>(observer =>
            {
                var hasValue = false;
                var lastValue = default(T);
                return source.Subscribe(new ForwardObserver<T>(
                    value =>
                    {
                        if (hasValue && comparer.Equals(lastValue, value))
                            return;
                        hasValue = true;
                        lastValue = value;
                        observer.OnNext(value);
                    },
                    observer.OnError,
                    observer.OnCompleted));
            });
        }
    }
}
