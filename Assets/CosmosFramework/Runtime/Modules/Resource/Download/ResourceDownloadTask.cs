using System;
namespace Cosmos.Resource
{
    /// <summary>
    /// 下载任务
    /// </summary>
    public struct ResourceDownloadTask : IEquatable<ResourceDownloadTask>
    {
        /// <summary>
        /// DownloadUrl = url/bundle key
        /// </summary>
        public string ResourceDownloadURL { get; private set; }
        /// <summary>
        /// DownloadPath =abs path/bundle key
        /// </summary>
        public string ResourceDownloadPath { get; private set; }
        /// <summary>
        /// bundle size in manifest
        /// </summary>
        public long RecordedResourceSize { get; private set; }
        public bool Equals(ResourceDownloadTask other)
        {
            return this.ResourceDownloadURL == other.ResourceDownloadURL &&
               this.ResourceDownloadPath == other.ResourceDownloadPath &&
              this.RecordedResourceSize == other.RecordedResourceSize;
        }
        /// <summary>
        /// 生成下载任务
        /// </summary>
        /// <param name="resourceDownloadURL">资源地址</param>
        /// <param name="resourceDownloadPath">下载地址</param>
        /// <param name="recordedResourceSize">完整的二进制文件长度</param>
        /// <returns></returns>
        public ResourceDownloadTask(string resourceDownloadURL, string resourceDownloadPath, long recordedResourceSize)
        {
            ResourceDownloadURL = resourceDownloadURL;
            ResourceDownloadPath = resourceDownloadPath;
            RecordedResourceSize = recordedResourceSize;
        }
    }
}
