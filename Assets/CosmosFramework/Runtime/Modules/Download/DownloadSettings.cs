using UnityEngine;

namespace Cosmos.Download
{
    /// <summary>
    /// 下载调度参数（模块级，所有任务共享）
    /// </summary>
    public sealed class DownloadSettings
    {
        int maxConcurrent = 3;
        int retryLimit = 2;
        float retryDelaySeconds = 1f;
        float timeoutSeconds = 30f;
        float progressIntervalSeconds = 0.1f;
        string tempFileExtension = ".download";
        /// <summary>
        /// 最大并发下载数（&gt;=1）
        /// </summary>
        public int MaxConcurrent
        {
            get { return maxConcurrent; }
            set { maxConcurrent = Mathf.Max(1, value); }
        }
        /// <summary>
        /// 默认失败重试次数（&gt;=0）
        /// </summary>
        public int RetryLimit
        {
            get { return retryLimit; }
            set { retryLimit = Mathf.Max(0, value); }
        }
        /// <summary>
        /// 重试退避基数（秒）：第 n 次重试等待 base * 2^(n-1)
        /// </summary>
        public float RetryDelaySeconds
        {
            get { return retryDelaySeconds; }
            set { retryDelaySeconds = Mathf.Max(0f, value); }
        }
        /// <summary>
        /// 单次尝试的默认超时时间（秒），&lt;=0 表示不限时
        /// </summary>
        public float TimeoutSeconds
        {
            get { return timeoutSeconds; }
            set { timeoutSeconds = Mathf.Max(0f, value); }
        }
        /// <summary>
        /// 进度事件的最小间隔（秒），用于给 UI 限流；&lt;=0 表示每帧都发
        /// </summary>
        public float ProgressIntervalSeconds
        {
            get { return progressIntervalSeconds; }
            set { progressIntervalSeconds = Mathf.Max(0f, value); }
        }
        /// <summary>
        /// 临时文件后缀；下载完成后临时文件会被移动到目标路径
        /// </summary>
        public string TempFileExtension
        {
            get { return tempFileExtension; }
            set { tempFileExtension = string.IsNullOrEmpty(value) ? ".download" : value; }
        }
    }
}
