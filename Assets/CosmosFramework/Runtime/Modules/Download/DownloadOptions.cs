using System.Runtime.InteropServices;

namespace Cosmos.Download
{
    /// <summary>
    /// 单个下载任务的参数
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct DownloadOptions
    {
        /// <summary>
        /// 写入模式
        /// </summary>
        public DownloadMode Mode { get; set; }
        /// <summary>
        /// 追加模式的起始字节偏移（仅 <see cref="DownloadMode.Append"/> 使用）
        /// </summary>
        public long ByteOffset { get; set; }
        /// <summary>
        /// 优先级，数值越大越先被调度
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 失败重试次数，&lt;0 表示使用模块默认值
        /// </summary>
        public int RetryLimit { get; set; }
        /// <summary>
        /// 单次尝试的超时时间（秒），&lt;=0 表示使用模块默认值
        /// </summary>
        public float TimeoutSeconds { get; set; }
        /// <summary>
        /// 默认参数：断点续传 + 默认优先级
        /// </summary>
        public readonly static DownloadOptions Default = new DownloadOptions()
        {
            Mode = DownloadMode.ResumeIfPossible,
            ByteOffset = 0,
            Priority = 0,
            RetryLimit = -1,
            TimeoutSeconds = 0f,
        };
    }
}
