using System;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资产句柄。
    /// <para>支持协程等待：yield return handle；支持事件回调：handle.Completed += ...；支持async/await：await handle.Task。</para>
    /// </summary>
    public class AssetHandle<T> : HandleBase
        where T : UnityEngine.Object
    {
        readonly TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
        /// <summary>
        /// 资源对象
        /// </summary>
        public UnityEngine.Object AssetObject
        {
            get
            {
                if (IsValidWithWarning == false)
                    return null;
                return Provider.AssetObject;
            }
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
                if (Provider != null && Provider.IsDone)
                {
                    value?.Invoke(this);
                }
                else
                {
                    completed += value;
                }
            }
            remove { completed -= value; }
        }
        /// <summary>
        /// 异步任务，支持await
        /// </summary>
        public new Task<T> Task
        {
            get { return taskCompletionSource.Task; }
        }
        /// <summary>
        /// 同步等待加载完成
        /// </summary>
        public T WaitForCompletion()
        {
            if (IsValidWithWarning == false)
                return null;
            Provider.WaitForCompletion();
            return Asset;
        }
        Action<AssetHandle<T>> completed;
        internal AssetHandle(ProviderBase provider) : base(provider)
        {
            provider.AddHandle(this);
        }
        internal override void InvokeCallback()
        {
            if (Provider.Status == OperationStatus.Succeeded)
                taskCompletionSource.TrySetResult(Asset);
            else
                taskCompletionSource.TrySetException(new InvalidOperationException(Provider.Error ?? "Asset load failed !"));
            completed?.Invoke(this);
        }
    }
}

