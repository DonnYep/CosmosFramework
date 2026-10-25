using System;

namespace Cosmos.Download
{
    /// <summary>
    /// 下载进度事件参数
    /// </summary>
    public class DonwloadUpdateEventArgs : GameEventArgs
    {
        /// <summary>
        /// 当前任务快照
        /// </summary>
        public DownloadInfo DownloadInfo { get; private set; }
        /// <summary>
        /// 当前任务序号（从1开始）
        /// </summary>
        public int CurrentDownloadTaskIndex { get; private set; }
        /// <summary>
        /// 本批次任务总数
        /// </summary>
        public int DownloadTaskCount { get; private set; }
        /// <summary>
        /// 本批次耗时
        /// </summary>
        public TimeSpan TimeSpan { get; private set; }
        /// <summary>
        /// 整体进度 0~1（按字节加权）
        /// </summary>
        public float OverallProgress { get; private set; }
        /// <summary>
        /// 整体已下载字节
        /// </summary>
        public long TotalDownloadedBytes { get; private set; }
        /// <summary>
        /// 整体预期字节（仅统计已知长度的任务）
        /// </summary>
        public long TotalExpectedBytes { get; private set; }
        public override void Release()
        {
            DownloadInfo = default;
            CurrentDownloadTaskIndex = 0;
            DownloadTaskCount = 0;
            TimeSpan = TimeSpan.Zero;
            OverallProgress = 0f;
            TotalDownloadedBytes = 0L;
            TotalExpectedBytes = 0L;
        }
        public static DonwloadUpdateEventArgs Create(DownloadInfo downloadInfo, int currentTaskIndex, int taskCount, TimeSpan timeSpan)
        {
            return Create(downloadInfo, currentTaskIndex, taskCount, timeSpan, 0f, 0L, 0L);
        }
        public static DonwloadUpdateEventArgs Create(DownloadInfo downloadInfo, int currentTaskIndex, int taskCount, TimeSpan timeSpan,
            float overallProgress, long totalDownloadedBytes, long totalExpectedBytes)
        {
            var eventArgs = ReferencePool.Acquire<DonwloadUpdateEventArgs>();
            eventArgs.DownloadInfo = downloadInfo;
            eventArgs.CurrentDownloadTaskIndex = currentTaskIndex;
            eventArgs.DownloadTaskCount = taskCount;
            eventArgs.TimeSpan = timeSpan;
            eventArgs.OverallProgress = overallProgress;
            eventArgs.TotalDownloadedBytes = totalDownloadedBytes;
            eventArgs.TotalExpectedBytes = totalExpectedBytes;
            return eventArgs;
        }
        public static void Release(DonwloadUpdateEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
