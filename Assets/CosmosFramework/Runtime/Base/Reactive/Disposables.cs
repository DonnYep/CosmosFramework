using System;
namespace Cosmos.Reactive
{
    /// <summary>
    /// 常用的一次性句柄。
    /// </summary>
    public static class Disposable
    {
        /// <summary>
        /// 什么都不做的句柄。
        /// </summary>
        public static readonly IDisposable Empty = new EmptyDisposable();
        /// <summary>
        /// 用委托构造句柄。
        /// </summary>
        public static IDisposable Create(Action onDispose)
        {
            if (onDispose == null)
                throw new ArgumentNullException("Dispose handler is invalid !");
            return new ActionDisposable(onDispose);
        }
        sealed class EmptyDisposable : IDisposable
        {
            public void Dispose() { }
        }
        sealed class ActionDisposable : IDisposable
        {
            Action onDispose;
            public ActionDisposable(Action onDispose)
            {
                this.onDispose = onDispose;
            }
            public void Dispose()
            {
                var action = onDispose;
                onDispose = null;
                action?.Invoke();
            }
        }
    }
    /// <summary>
    /// 句柄集合：统一释放一组订阅（例如一个界面 / 一个实体持有的所有订阅）。
    /// </summary>
    public sealed class CompositeDisposable : IDisposable
    {
        readonly System.Collections.Generic.List<IDisposable> disposables = new System.Collections.Generic.List<IDisposable>();
        bool disposed;
        /// <summary>
        /// 句柄数量。
        /// </summary>
        public int Count { get { return disposables.Count; } }
        /// <summary>
        /// 是否已释放。
        /// </summary>
        public bool IsDisposed { get { return disposed; } }
        public CompositeDisposable() { }
        public CompositeDisposable(params IDisposable[] items)
        {
            if (items == null)
                return;
            for (int i = 0; i < items.Length; i++)
                Add(items[i]);
        }
        /// <summary>
        /// 加入句柄；若集合已释放，则立即释放该句柄。
        /// </summary>
        public void Add(IDisposable disposable)
        {
            if (disposable == null)
                return;
            if (disposed)
            {
                disposable.Dispose();
                return;
            }
            disposables.Add(disposable);
        }
        /// <summary>
        /// 移除并释放单个句柄。
        /// </summary>
        public bool Remove(IDisposable disposable)
        {
            if (disposable == null)
                return false;
            if (!disposables.Remove(disposable))
                return false;
            disposable.Dispose();
            return true;
        }
        /// <summary>
        /// 释放全部句柄并清空。
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < disposables.Count; i++)
                disposables[i].Dispose();
            disposables.Clear();
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            Clear();
        }
    }
}
