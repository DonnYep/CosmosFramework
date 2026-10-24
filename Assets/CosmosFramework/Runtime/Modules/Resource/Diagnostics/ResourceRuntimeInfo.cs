using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源运行时信息（调试窗口用）。
    /// </summary>
    public sealed class ResourceRuntimeInfo
    {
        /// <summary>
        /// 当前 Provider 名称
        /// </summary>
        public string ProviderName;
        /// <summary>
        /// Provider 是否初始化完成
        /// </summary>
        public bool ProviderInitialized;
        /// <summary>
        /// 当前激活的加载操作数
        /// </summary>
        public int ActiveOperationCount;
        /// <summary>
        /// 累计加载次数
        /// </summary>
        public int TotalLoadCount;
        /// <summary>
        /// 累计失败次数
        /// </summary>
        public int FailedLoadCount;
        /// <summary>
        /// 诊断记录条数
        /// </summary>
        public int RecordCount;
        /// <summary>
        /// 当前被名称索引持有的资产
        /// </summary>
        public List<LoadedAssetInfo> LoadedAssets = new List<LoadedAssetInfo>();

        public override string ToString()
        {
            return $"provider={ProviderName} init={ProviderInitialized} active={ActiveOperationCount} total={TotalLoadCount} failed={FailedLoadCount} loaded={LoadedAssets.Count}";
        }
    }

    /// <summary>
    /// 已加载资产信息。
    /// </summary>
    public sealed class LoadedAssetInfo
    {
        public string Key;
        public string AssetTypeName;
        public string LoadMode;
        public int RefCount;
        public float DurationSeconds;
        public bool Succeeded;
        public string Error;
    }
}
