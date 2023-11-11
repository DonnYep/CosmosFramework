using System;

namespace Cosmos.Resource
{
    public struct ResourceDownloadNode 
    {
        /// <summary>
        /// download id
        /// </summary>
        public int ResourceDownloadId { get; private set; }
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
        /// <summary>
        /// bundle size on local path
        /// </summary>
        public long LocalResourceSize { get; private set; }
        public ResourceDownloadNode(int resourceDownloadId, string resourceDownloadURL, string resourceDownloadPath, long recordedResourceSize, long localResourceSize)
        {
            ResourceDownloadId = resourceDownloadId;
            ResourceDownloadURL = resourceDownloadURL;
            ResourceDownloadPath = resourceDownloadPath;
            RecordedResourceSize = recordedResourceSize;
            LocalResourceSize = localResourceSize;
        }
    }
}