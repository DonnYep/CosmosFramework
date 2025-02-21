using System.Collections;

namespace Cosmos.Resource
{
    public abstract class HandleBase : IEnumerator
    {
        private readonly AssetInfo assetInfo;
        internal ProviderBase Provider { private set; get; }
        /// <summary>
        /// 加载进度
        /// </summary>
        public float Progress
        {
            get
            {
                if (IsValidWithWarning == false)
                    return 0;
                return Provider.Progress;
            }
        }
        /// <summary>
        /// 是否加载完毕
        /// </summary>
        public bool IsDone
        {
            get
            {
                if (IsValidWithWarning == false)
                    return false;
                return Provider.IsDone;
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
        /// <summary>
        /// 句柄是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (Provider != null && Provider.IsDestroyed == false)
                    return true;
                else
                    return false;
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
