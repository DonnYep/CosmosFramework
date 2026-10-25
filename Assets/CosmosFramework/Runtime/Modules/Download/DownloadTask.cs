using System;
using System.IO;

namespace Cosmos.Download
{
    /// <summary>
    /// 下载任务（可变状态对象，由 <see cref="DownloadScheduler"/> 独占推进）
    /// </summary>
    internal sealed class DownloadTask
    {
        /// <summary>
        /// 下载Id
        /// </summary>
        public long DownloadId { get; set; }
        /// <summary>
        /// 资源定位地址
        /// </summary>
        public string DownloadUrl { get; set; }
        /// <summary>
        /// 下载后存储的绝对路径
        /// </summary>
        public string DownloadPath { get; set; }
        /// <summary>
        /// 临时文件路径（下载中的数据写在这里，完成后才移动到 <see cref="DownloadPath"/>）
        /// </summary>
        public string TempPath { get; set; }
        /// <summary>
        /// 写入模式
        /// </summary>
        public DownloadMode Mode { get; set; }
        /// <summary>
        /// 追加模式的起始偏移
        /// </summary>
        public long AppendOffset { get; set; }
        /// <summary>
        /// 优先级，数值越大越先调度
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 失败重试次数上限
        /// </summary>
        public int RetryLimit { get; set; }
        /// <summary>
        /// 单次尝试超时时间（秒）
        /// </summary>
        public float TimeoutSeconds { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public DownloadState State { get; set; }
        /// <summary>
        /// 本次尝试的起始字节偏移（断点续传时 &gt; 0）
        /// </summary>
        public long StartOffset { get; set; }
        /// <summary>
        /// 累计已下载字节（含断点偏移）
        /// </summary>
        public long DownloadedBytes { get; set; }
        /// <summary>
        /// 预期总字节数，-1 表示未知
        /// </summary>
        public long ExpectedBytes { get; set; }
        /// <summary>
        /// 已尝试次数
        /// </summary>
        public int AttemptCount { get; set; }
        /// <summary>
        /// 最近一次失败原因
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 重试倒计时（秒）
        /// </summary>
        public float RetryCountdown { get; set; }
        /// <summary>
        /// 当前传输对象
        /// </summary>
        public IDownloadTransfer Transfer { get; set; }
        /// <summary>
        /// 是否由"暂停"触发了当前取消
        /// </summary>
        public bool PauseRequested { get; set; }
        /// <summary>
        /// 当前速度（字节/秒）
        /// </summary>
        public long SpeedBytesPerSecond { get; set; }
        /// <summary>
        /// 进度事件节流用：上次派发进度时的调度器时间
        /// </summary>
        public float LastProgressTime { get; set; }
        /// <summary>
        /// 进度事件节流用：上次派发进度时的已下载字节
        /// </summary>
        public long LastProgressBytes { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 是否已经结束（完成/失败/取消）
        /// </summary>
        public bool IsHandled
        {
            get
            {
                return State == DownloadState.Completed || State == DownloadState.Failed || State == DownloadState.Canceled;
            }
        }
        /// <summary>
        /// 构造任务
        /// </summary>
        public DownloadTask(long downloadId, string downloadUrl, string downloadPath, string tempPath, DownloadOptions options, DownloadSettings settings)
        {
            DownloadId = downloadId;
            DownloadUrl = downloadUrl;
            DownloadPath = downloadPath;
            TempPath = tempPath;
            Mode = options.Mode;
            AppendOffset = options.ByteOffset;
            Priority = options.Priority;
            RetryLimit = options.RetryLimit >= 0 ? options.RetryLimit : settings.RetryLimit;
            TimeoutSeconds = options.TimeoutSeconds > 0f ? options.TimeoutSeconds : settings.TimeoutSeconds;
            State = DownloadState.Queued;
            ExpectedBytes = -1L;
            StartTime = DateTime.Now;
        }
        /// <summary>
        /// 转换对外快照
        /// </summary>
        /// <returns>下载信息</returns>
        public DownloadInfo ToInfo()
        {
            var progress = 0f;
            if (ExpectedBytes > 0)
                progress = (float)DownloadedBytes / ExpectedBytes;
            else if (State == DownloadState.Completed)
                progress = 1f;
            if (progress < 0f)
                progress = 0f;
            if (progress > 1f)
                progress = 1f;
            var endTime = EndTime == default(DateTime) ? DateTime.Now : EndTime;
            var elapsed = endTime - StartTime;
            if (elapsed < TimeSpan.Zero)
                elapsed = TimeSpan.Zero;
            return new DownloadInfo(DownloadId, DownloadUrl, DownloadPath, State,
                (ulong)(DownloadedBytes > 0 ? DownloadedBytes : 0), ExpectedBytes, progress, elapsed, SpeedBytesPerSecond, AttemptCount);
        }
        /// <summary>
        /// 当前临时文件的长度
        /// </summary>
        /// <returns>字节数，文件不存在时为0</returns>
        public long GetTempLength()
        {
            try
            {
                return File.Exists(TempPath) ? new FileInfo(TempPath).Length : 0L;
            }
            catch (Exception)
            {
                return 0L;
            }
        }
    }
}
