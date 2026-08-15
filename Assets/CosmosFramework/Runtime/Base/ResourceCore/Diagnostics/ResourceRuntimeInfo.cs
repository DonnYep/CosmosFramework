using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// 当前运行时加载状态信息（用于调试窗口与性能分析）。
    /// </summary>
    public class ResourceRuntimeInfo
    {
        /// <summary>
        /// 当前加载中的资产信息
        /// </summary>
        public readonly List<LoadedAssetDebugInfo> LoadedAssets = new List<LoadedAssetDebugInfo>();
        /// <summary>
        /// 当前已加载的包体数量
        /// </summary>
        public int LoadedBundleCount;
        /// <summary>
        /// 当前激活的加载流程数量
        /// </summary>
        public int ActiveProviderCount;
        /// <summary>
        /// 累计加载次数
        /// </summary>
        public int TotalLoadCount;
        /// <summary>
        /// 累计失败次数
        /// </summary>
        public int FailedLoadCount;
        /// <summary>
        /// 加载记录数量
        /// </summary>
        public int RecordCount;
    }

    /// <summary>
    /// 单个已加载资产的调试信息
    /// </summary>
    public class LoadedAssetDebugInfo
    {
        /// <summary>
        /// 资产路径
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetName;
        /// <summary>
        /// 所属包体
        /// </summary>
        public string BundleName;
        /// <summary>
        /// 资产类型
        /// </summary>
        public string AssetTypeName;
        /// <summary>
        /// 加载方式
        /// </summary>
        public string LoadMode;
        /// <summary>
        /// 当前引用计数
        /// </summary>
        public int RefCount;
        /// <summary>
        /// 加载耗时（秒）
        /// </summary>
        public float LoadDuration;
        /// <summary>
        /// 运行时内存占用（字节）
        /// </summary>
        public long MemorySize;
        /// <summary>
        /// 是否加载成功
        /// </summary>
        public bool Succeeded;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error;
    }
}
