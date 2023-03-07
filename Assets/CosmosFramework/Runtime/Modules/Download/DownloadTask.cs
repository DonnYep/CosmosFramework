using System;
namespace Cosmos.Download
{
    internal struct DownloadTask : IEquatable<DownloadTask>
    {
        public int DownloadId { get; private set; }
        /// <summary>
        /// URI绝对路径；
        /// </summary>
        public string DownloadUri { get; private set; }
        /// <summary>
        /// 本地资源的绝对路径；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// 下载任务的构造函数；
        /// </summary>
        /// <param name="downloadUri">URI绝对路径</param>
        /// <param name="downloadPath">本地资源的绝对路径</param>
        public DownloadTask(int dwnloadId, string downloadUri, string downloadPath)
        {
            DownloadId = dwnloadId;
            DownloadUri = downloadUri;
            DownloadPath = downloadPath;
        }
        public bool Equals(DownloadTask other)
        {
            bool result = false;
            if (this.GetType() == other.GetType())
            {
                result = this.DownloadUri == other.DownloadUri &&
                    this.DownloadPath == other.DownloadPath &&
                    this.DownloadId == other.DownloadId;
            }
            return result;
        }
    }
}
