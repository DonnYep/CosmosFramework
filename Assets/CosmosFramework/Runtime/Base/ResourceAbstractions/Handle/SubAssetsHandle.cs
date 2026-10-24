using System;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 子资产句柄（一个资产文件内的多个子资产，如 SpriteSheet 切图、FBX 内的多个 Mesh）。
    /// </summary>
    public sealed class SubAssetsHandle<T> : HandleBase
        where T : UnityEngine.Object
    {
        readonly TaskCompletionSource<T[]> taskCompletionSource = new TaskCompletionSource<T[]>();
        Action<SubAssetsHandle<T>> completed;
        T[] cachedAssets;

        /// <summary>
        /// 子资产集合
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
        public event Action<SubAssetsHandle<T>> Completed
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
        /// 创建子资产句柄（供 Provider 与资源桥使用）。
        /// </summary>
        public SubAssetsHandle(ResourceProviderOperation operation) : base(operation)
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
                taskCompletionSource.TrySetException(new InvalidOperationException(Operation.Error ?? "sub assets load failed"));
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
