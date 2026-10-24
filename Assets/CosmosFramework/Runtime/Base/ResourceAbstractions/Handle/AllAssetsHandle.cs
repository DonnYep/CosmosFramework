using System;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 全部资产句柄（按包体/地址加载其中所有资产）。
    /// </summary>
    public sealed class AllAssetsHandle<T> : HandleBase
        where T : UnityEngine.Object
    {
        readonly TaskCompletionSource<T[]> taskCompletionSource = new TaskCompletionSource<T[]>();
        Action<AllAssetsHandle<T>> completed;
        T[] cachedAssets;

        /// <summary>
        /// 资产集合
        /// </summary>
        public T[] Assets
        {
            get
            {
                if (!IsDone)
                    return null;
                if (cachedAssets == null)
                    cachedAssets = Convert(IsValid ? Operation.AssetObjects : null);
                return cachedAssets;
            }
        }
        /// <summary>
        /// 完成回调事件
        /// </summary>
        public event Action<AllAssetsHandle<T>> Completed
        {
            add
            {
                if (IsDone)
                    value?.Invoke(this);
                else
                    completed += value;
            }
            remove { completed -= value; }
        }
        /// <summary>
        /// 异步任务，支持 await
        /// </summary>
        public new Task<T[]> Task
        {
            get { return taskCompletionSource.Task; }
        }

        /// <summary>
        /// 创建全部资产句柄（供 Provider 与资源桥使用）。
        /// </summary>
        public AllAssetsHandle(ResourceProviderOperation operation) : base(operation)
        {
        }

        /// <summary>
        /// 同步等待加载完成。
        /// </summary>
        public T[] WaitForCompletion()
        {
            PumpUntilDone();
            return Assets;
        }

        internal override void InvokeCallback()
        {
            var assets = Assets ?? new T[0];
            if (IsValid && Operation.Status != OperationStatus.Succeeded && assets.Length == 0)
                taskCompletionSource.TrySetException(new InvalidOperationException(Operation.Error ?? "all assets load failed"));
            else
                taskCompletionSource.TrySetResult(assets);
            completed?.Invoke(this);
        }

        static T[] Convert(UnityEngine.Object[] source)
        {
            if (source == null)
                return new T[0];
            var result = new T[source.Length];
            for (int i = 0; i < source.Length; i++)
                result[i] = source[i] as T;
            return result;
        }
    }
}
