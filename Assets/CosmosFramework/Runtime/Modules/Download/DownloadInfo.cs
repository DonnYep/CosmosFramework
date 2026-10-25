using System;
using System.Runtime.InteropServices;

namespace Cosmos.Download
{
    /// <summary>
    /// 下载任务快照（对外只读视图）
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct DownloadInfo : IEquatable<DownloadInfo>
    {
        /// <summary>
        /// 下载Id
        /// </summary>
        public long DownloadId { get; private set; }
        /// <summary>
        /// 资源定位地址
        /// </summary>
        public string DownloadUrl { get; private set; }
        /// <summary>
        /// 下载后存储的绝对路径
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public DownloadState DownloadState { get; private set; }
        /// <summary>
        /// 已下载字节数（含断点偏移）
        /// </summary>
        public ulong DownloadedLength { get; private set; }
        /// <summary>
        /// 预期总字节数，-1 表示尚不可知
        /// </summary>
        public long ExpectedLength { get; private set; }
        /// <summary>
        /// 下载进度 0~1
        /// </summary>
        public float DownloadProgress { get; private set; }
        /// <summary>
        /// 任务耗时
        /// </summary>
        public TimeSpan DownloadTimeSpan { get; private set; }
        /// <summary>
        /// 当前速度（字节/秒）
        /// </summary>
        public long SpeedBytesPerSecond { get; private set; }
        /// <summary>
        /// 已尝试次数
        /// </summary>
        public int AttemptCount { get; private set; }
        /// <summary>
        /// 构造下载信息
        /// </summary>
        public DownloadInfo(long downloadId, string downloadUrl, string downloadPath, DownloadState downloadState,
            ulong downloadedLength, long expectedLength, float downloadProgress, TimeSpan timeSpan, long speedBytesPerSecond, int attemptCount)
        {
            DownloadId = downloadId;
            DownloadUrl = downloadUrl;
            DownloadPath = downloadPath;
            DownloadState = downloadState;
            DownloadedLength = downloadedLength;
            ExpectedLength = expectedLength;
            DownloadProgress = downloadProgress;
            DownloadTimeSpan = timeSpan;
            SpeedBytesPerSecond = speedBytesPerSecond;
            AttemptCount = attemptCount;
        }
        /// <summary>
        /// 兼容旧版的构造（无状态与速度信息）
        /// </summary>
        public DownloadInfo(long downloadId, string downloadUrl, string downloadPath, ulong downloadedLength, float downloadProgress, TimeSpan timeSpan)
            : this(downloadId, downloadUrl, downloadPath, DownloadState.Downloading, downloadedLength, -1L, downloadProgress, timeSpan, 0L, 0)
        {
        }
        public bool Equals(DownloadInfo other)
        {
            return DownloadUrl == other.DownloadUrl &&
                DownloadPath == other.DownloadPath;
        }
        public override string ToString()
        {
            return $"DownloadId:{DownloadId} ;State:{DownloadState} ;URL:{DownloadUrl} ;Path:{DownloadPath} ;Downloaded:{DownloadedLength}/{ExpectedLength} ;Progress:{DownloadProgress:0.00} ;Speed:{SpeedBytesPerSecond}B/s ;Attempt:{AttemptCount} ;Time:{DownloadTimeSpan}";
        }
    }
}
