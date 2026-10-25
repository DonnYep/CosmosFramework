namespace Cosmos.Download
{
    /// <summary>
    /// 下载任务状态
    /// </summary>
    public enum DownloadState : byte
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        None = 0,
        /// <summary>
        /// 已入队，等待并发槽
        /// </summary>
        Queued = 1,
        /// <summary>
        /// 传输中
        /// </summary>
        Downloading = 2,
        /// <summary>
        /// 传输完成，正在校验文件长度
        /// </summary>
        Verifying = 3,
        /// <summary>
        /// 已完成（临时文件已提交到目标路径）
        /// </summary>
        Completed = 4,
        /// <summary>
        /// 已失败（重试次数耗尽）
        /// </summary>
        Failed = 5,
        /// <summary>
        /// 已取消
        /// </summary>
        Canceled = 6,
        /// <summary>
        /// 已暂停（临时文件保留，恢复后继续）
        /// </summary>
        Paused = 7,
        /// <summary>
        /// 失败后等待重试（退避中）
        /// </summary>
        WaitingRetry = 8,
    }
}
