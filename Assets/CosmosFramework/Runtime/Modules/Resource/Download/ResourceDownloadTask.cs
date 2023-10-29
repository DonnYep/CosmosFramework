using System;
namespace Cosmos.Resource
{
    /// <summary>
    /// <para>生成下载任务请使用：<see cref="ResourceDownloadTask.Create(string, string, long, long)"/></para>
    ///<para> 释放下载任务请使用：<see cref="ResourceDownloadTask.Release"/></para>
    /// </summary>
    public class ResourceDownloadTask : IEquatable<ResourceDownloadTask>, IReference
    {
        /// <summary>
        /// DownloadUri = url/bundle key
        /// </summary>
        public string ResourceDownloadURL { get; private set; }
        /// <summary>
        /// DownloadPath =abs path/bundle key
        /// </summary>
        public string ResourceDownloadPath { get; private set; }
        /// <summary>
        /// bundle size on local path
        /// </summary>
        public long LocalResourceSize { get; private set; }
        /// <summary>
        /// bundle size in manifest
        /// </summary>
        public long RecordedResourceSize { get; private set; }
        public bool Equals(ResourceDownloadTask other)
        {
            return this.ResourceDownloadURL == other.ResourceDownloadURL &&
               this.ResourceDownloadPath == other.ResourceDownloadPath &&
               this.LocalResourceSize == other.LocalResourceSize &&
              this.RecordedResourceSize == other.RecordedResourceSize;
        }
        public void Release()
        {
            ResourceDownloadURL = string.Empty;
            ResourceDownloadPath = string.Empty;
            LocalResourceSize = 0;
            RecordedResourceSize = 0;
        }
        /// <summary>
        /// 生成下载任务
        /// </summary>
        /// <param name="resourceDownloadURL">资源地址</param>
        /// <param name="resourceDownloadPath">下载地址</param>
        /// <param name="localResourceSize">本地已经下载的文件二进制长度</param>
        /// <param name="recordedResourceSize">完整的二进制文件长度</param>
        /// <returns></returns>
        public static ResourceDownloadTask Create(string resourceDownloadURL, string resourceDownloadPath, long localResourceSize, long recordedResourceSize)
        {
            var task = ReferencePool.Acquire<ResourceDownloadTask>();
            task.ResourceDownloadURL = resourceDownloadURL;
            task.ResourceDownloadPath = resourceDownloadPath;
            task.LocalResourceSize = localResourceSize;
            task.RecordedResourceSize = recordedResourceSize;
            return task;
        }
        public static void Release(ResourceDownloadTask task)
        {
            ReferencePool.Release(task);
        }
    }
}
