using System.Collections;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源操作句柄基类。
    /// <para>句柄是运行时唯一的资源交互对象，支持协程等待、事件回调、async/await以及同步等待。</para>
    /// </summary>
    public abstract class HandleBase : IEnumerator
    {
        readonly AssetInfo assetInfo;
        internal ProviderBase Provider { get; private set; }
        /// <summary>
        /// 加载进度
        /// </summary>
        public float Progress
        {
            get
            {
                if (IsValid == false)
                    return 0;
                return Provider.Progress;
            }
        }
        /// <summary>
        /// 是否加载完毕
        /// <para>Provider被销毁（加载被终止/引用归零）时视为已结束，避免协程永久挂起。</para>
        /// </summary>
        public bool IsDone
        {
            get
            {
                if (IsValid == false)
                    return true;
                return Provider.IsDone;
            }
        }
        /// <summary>
        /// 异步任务
        /// </summary>
        public Task Task
        {
            get
            {
                if (Provider == null)
                    return null;
                return Provider.Task;
            }
        }
        /// <summary>
        /// 句柄是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return Provider != null && Provider.IsDestroyed == false;
            }
        }
        internal bool IsValidWithWarning
        {
            get
            {
                if (Provider != null && Provider.IsDestroyed == false)
                {
                    return true;
                }
                else
                {
                    if (Provider == null)
                        UnityEngine.Debug.LogError($"Operation handle is released : {assetInfo.AssetPath}");
                    else if (Provider.IsDestroyed)
                        UnityEngine.Debug.LogError($"Provider is destroyed : {assetInfo.AssetPath}");
                    return false;
                }
            }
        }
        internal HandleBase(ProviderBase provider)
        {
            Provider = provider;
            assetInfo = provider.MainAssetInfo;
        }
        internal abstract void InvokeCallback();
        /// <summary>
        /// 获取资源信息
        /// </summary>
        public AssetInfo GetAssetInfo()
        {
            return assetInfo;
        }
        /// <summary>
        /// 释放句柄，减少一个引用计数。
        /// <para>当引用计数归零时，Provider自动销毁并释放对应资源。</para>
        /// </summary>
        public void Release()
        {
            if (Provider == null)
                return;
            Provider.ReleaseHandle(this);
            OnRelease();
            Provider = null;
        }
        /// <summary>
        /// 将句柄与Provider解绑（内部使用）
        /// </summary>
        internal void SetProviderNull()
        {
            Provider = null;
        }
        protected virtual void OnRelease() { }
        #region 协程
        bool IEnumerator.MoveNext()
        {
            return !IsDone;
        }
        void IEnumerator.Reset()
        {
        }
        object IEnumerator.Current
        {
            get { return Provider; }
        }
        #endregion
    }
}
