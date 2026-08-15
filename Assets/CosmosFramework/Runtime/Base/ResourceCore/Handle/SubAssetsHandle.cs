using System;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资产及其子资产句柄。
    /// </summary>
    public class SubAssetsHandle<T> : HandleBase
        where T : UnityEngine.Object
    {
        readonly TaskCompletionSource<T[]> taskCompletionSource = new TaskCompletionSource<T[]>();
        /// <summary>
        /// 资源对象集合
        /// </summary>
        public T[] AssetObjects
        {
            get
            {
                if (IsValidWithWarning == false)
                    return null;
                return ConvertAssets();
            }
        }
        /// <summary>
        /// 完成回调事件
        /// </summary>
        public event Action<SubAssetsHandle<T>> Completed
        {
            add
            {
                if (Provider != null && Provider.IsDone)
                    value?.Invoke(this);
                else
                    completed += value;
            }
            remove { completed -= value; }
        }
        /// <summary>
        /// 异步任务，支持await
        /// </summary>
        public new Task<T[]> Task
        {
            get { return taskCompletionSource.Task; }
        }
        /// <summary>
        /// 同步等待加载完成
        /// </summary>
        public T[] WaitForCompletion()
        {
            if (IsValidWithWarning == false)
                return null;
            Provider.WaitForCompletion();
            return ConvertAssets();
        }
        Action<SubAssetsHandle<T>> completed;
        internal SubAssetsHandle(ProviderBase provider) : base(provider)
        {
            provider.AddHandle(this);
        }
        T[] ConvertAssets()
        {
            var objs = Provider.AllAssetObjects;
            if (objs == null || objs.Length == 0)
                return null;
            var result = new T[objs.Length];
            var count = objs.Length;
            for (int i = 0; i < count; i++)
            {
                result[i] = objs[i] as T;
            }
            return result;
        }
        internal override void InvokeCallback()
        {
            if (Provider.Status == OperationStatus.Succeeded)
                taskCompletionSource.TrySetResult(ConvertAssets());
            else
                taskCompletionSource.TrySetException(new InvalidOperationException(Provider.Error ?? "SubAssets load failed !"));
            completed?.Invoke(this);
        }
    }
}
