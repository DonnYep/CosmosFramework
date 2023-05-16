using System;

namespace Cosmos.Download
{
    public struct DownloadInfo : IEquatable<DownloadInfo>
    {
        /// <summary>
        /// 下载数据的构造；
        /// </summary>
        /// <param name="downloadId"> 下载Id</param>
        /// <param name="downloadUri"> 资源定位地址</param>
        /// <param name="downloadPath">下载后存储的路径</param>c
        /// <param name="downloadedLength">length of downloaded file</param>
        /// <param name="timeSpan">download time</param>
        public DownloadInfo(int downloadId,string downloadUri, string downloadPath, ulong downloadedLength,float downloadProgress, TimeSpan timeSpan)
        {
            DownloadId= downloadId;
            DownloadUri = downloadUri;
            DownloadPath = downloadPath;
            DownloadedLength = downloadedLength;
            DownloadProgress= downloadProgress;
            DownloadTimeSpan = timeSpan;
        }
        public int  DownloadId{ get; private set; }
        /// <summary>
        /// 资源定位地址；
        /// </summary>
        public string DownloadUri { get; private set; }
        /// <summary>
        /// 下载后存储的路径；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// length of downloaded file
        /// </summary>
        public ulong DownloadedLength { get; private set; }
        /// <summary>
        /// Download progress
        /// </summary>
        public float DownloadProgress { get; private set; }
        /// <summary>
        /// Length of time spent downloading
        /// </summary>
        public TimeSpan DownloadTimeSpan { get; private set; }
        public bool Equals(DownloadInfo other)
        {
            return this.DownloadUri == other.DownloadUri &&
                this.DownloadPath == other.DownloadPath;
        }
        public override string ToString()
        {
            return $"DownloadId:{DownloadId} ;URI: {DownloadUri}; DownloadPath: {DownloadPath}; DownloadedLength: {DownloadedLength};DownloadProgress: {DownloadProgress} ; DownloadTimeSpan: {DownloadTimeSpan}";
        }
    }
}
