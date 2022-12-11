namespace Cosmos.Download
{
    /// <summary>
    /// download模块数据；
    /// </summary>
    internal class DownloadDataProxy
    {
        static float downloadTimeout;
        /// <summary>
        /// 终止时删除下载中的文件
        /// </summary>
        public static bool DeleteFileOnAbort { get; set; }
        /// <summary>
        /// 下载时追加写入；
        /// </summary>
        public static bool DownloadAppend { get; set; }
        /// <summary>
        ///任务过期时间，以秒为单位
        /// </summary>
        public static float DownloadTimeout
        {
            get { return downloadTimeout; }
            set
            {
                downloadTimeout = value;
                if (downloadTimeout < 0)
                    downloadTimeout = 0;
            }
        }
    }
}
