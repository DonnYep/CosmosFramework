using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 一次资源加载操作的记录信息（用于调试与性能分析）。
    /// </summary>
    public class ResourceLoadRecord
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
        /// 所属包体名称
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
        /// 开始时间戳（ms）
        /// </summary>
        public long StartTimeMs;
        /// <summary>
        /// 结束时间戳（ms）
        /// </summary>
        public long EndTimeMs;
        /// <summary>
        /// 加载耗时（秒）
        /// </summary>
        public float DurationSeconds;
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeeded;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error;
        /// <summary>
        /// 完成时的引用计数
        /// </summary>
        public int RefCount;
        /// <summary>
        /// 加载起始调用栈（诊断开启时捕获，可定位泄漏源头）
        /// </summary>
        public string StackTrace;
    }
}
