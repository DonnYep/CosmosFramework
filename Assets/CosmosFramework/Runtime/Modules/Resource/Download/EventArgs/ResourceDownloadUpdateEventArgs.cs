using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.Resource
{
    public class ResourceDownloadUpdateEventArgs : GameEventArgs
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
        /// 已经下载完成的大小
        /// </summary>
        public long CompletedDownloadSize { get; private set; }
        /// <summary>
        /// 总共需要下载的大小
        /// </summary>
        public long TotalRequiredDownloadSize { get; private set; }
        public override void Release()
        {
        }
    }
}
