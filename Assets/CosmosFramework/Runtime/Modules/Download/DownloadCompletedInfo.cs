using System;
using System.Runtime.InteropServices;

namespace Cosmos.Download
{
    [StructLayout(LayoutKind.Auto)]
    public struct DownloadCompletedInfo : IEquatable<DownloadCompletedInfo>
    {
        /// <summary>
        /// 下载数据的构造；
        /// </summary>
        /// <param name="uri"> 资源定位地址</param>
        /// <param name="downloadPath">下载后存储的路径</param>c
        /// <param name="downloadedLength">length of downloaded file</param>
        /// <param name="downloadedLength">length of time spent downloading</param>
        public DownloadCompletedInfo(string uri, string downloadPath, ulong downloadedLength, TimeSpan timeSpan)
        {
            URI = uri;
            DownloadPath = downloadPath;
            DownloadedLength = downloadedLength;
            DownloadTimeSpan = timeSpan;
        }
        /// <summary>
        /// 资源定位地址；
        /// </summary>
        public string URI { get; private set; }
        /// <summary>
        /// 下载后存储的路径；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// length of downloaded file
        /// </summary>
        public ulong DownloadedLength { get; private set; }
        /// <summary>
        /// Length of time spent downloading
        /// </summary>
        public TimeSpan DownloadTimeSpan { get; private set; }
        public bool Equals(DownloadCompletedInfo other)
        {
            return this.URI == other.URI &&
                this.DownloadPath == other.DownloadPath;
        }
        public override string ToString()
        {
            return $"URI: {URI}; DownloadPath: {DownloadPath}; DownloadedLength: {DownloadedLength}; DownloadTimeSpan: {DownloadTimeSpan}";
        }
    }
}
