using System;

namespace Cosmos.Resource
{
    public class AssetHandle<T> : HandleBase
        where T : UnityEngine.Object
    {
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
        public event Action<AssetHandle<T>> Completed
        {
            add
            {
                if (Provider.IsDone)
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

        public T WaitForCompletion()
        {
            if (IsValidWithWarning == false)
                return null;
            Provider.WaitForCompletion();
            return (T)Provider.AssetObject;
        }
        Action<AssetHandle<T>> completed;
        internal AssetHandle(ProviderBase provider) : base(provider)
        {
            provider.AddHandle(this);
        }
        internal override void InvokeCallback()
        {
            completed?.Invoke(this);
        }
    }
}
