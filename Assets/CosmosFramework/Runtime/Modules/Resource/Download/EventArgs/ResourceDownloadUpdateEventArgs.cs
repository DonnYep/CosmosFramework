﻿namespace Cosmos.Resource
{
    public class ResourceDownloadUpdateEventArgs : GameEventArgs
    {
        /// <summary>
        /// 已经下载完成的大小
        /// </summary>
        public long DownloadedSize { get; private set; }
        /// <summary>
        /// 总共需要下载的大小
        /// </summary>
        public long TotalRequirementDownloadSize { get; private set; }
        public override void Release()
        {
            DownloadedSize = 0;
            TotalRequirementDownloadSize = 0;
        }
        public static ResourceDownloadUpdateEventArgs Create(long downloadedSize, long totalRequirementDownloadSize)
        {
            var eventArgs = ReferencePool.Acquire<ResourceDownloadUpdateEventArgs>();
            eventArgs.DownloadedSize = downloadedSize;
            eventArgs.TotalRequirementDownloadSize = totalRequirementDownloadSize;
            return eventArgs;
        }
        public static void Release(ResourceDownloadUpdateEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}