using System;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资产句柄。
    /// <code>
    /// var handle = CosmosEntry.ResourceManager.LoadAssetHandle&lt;GameObject&gt;("Prefabs/Cube");
    /// handle.Completed += h =&gt; Instantiate(h.Asset);
    /// yield return handle;               // 协程等待
    /// var cube = await handle.Task;      // async/await
    /// var asset = handle.WaitForCompletion();
    /// handle.Release();                  // 释放一次引用
    /// </code>
    /// </summary>
    public sealed class AssetHandle<T> : HandleBase
        where T : UnityEngine.Object
    {
        readonly TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
        Action<AssetHandle<T>> completed;

        /// <summary>
        /// 资源对象
        /// </summary>
        public UnityEngine.Object AssetObject
        {
            get { return IsValid ? Operation.AssetObject : null; }
        }
        /// <summary>
        /// 转换后的资源对象
        /// </summary>
        public T Asset
        {
            get { return AssetObject as T; }
        }
        /// <summary>
        /// 完成回调事件
        /// </summary>
        public event Action<AssetHandle<T>> Completed
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
        public new Task<T> Task
        {
            get { return taskCompletionSource.Task; }
        }

        /// <summary>
        /// 创建资产句柄（供 Provider 与资源桥使用）。
        /// </summary>
        public AssetHandle(ResourceProviderOperation operation) : base(operation)
        {
        }

        /// <summary>
        /// 同步等待加载完成。
        /// </summary>
        public T WaitForCompletion()
        {
            PumpUntilDone();
            return Asset;
        }

        internal override void InvokeCallback()
        {
            if (!IsValid)
            {
                taskCompletionSource.TrySetResult(null);
                completed?.Invoke(this);
                return;
            }
            if (Operation.Status == OperationStatus.Succeeded)
                taskCompletionSource.TrySetResult(Asset);
            else
                taskCompletionSource.TrySetException(new InvalidOperationException(Operation.Error ?? "asset load failed"));
            completed?.Invoke(this);
        }
    }
}
