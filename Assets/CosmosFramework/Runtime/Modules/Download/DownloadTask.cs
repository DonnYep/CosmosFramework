using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    internal struct DownloadTask : IEquatable<DownloadTask>
    {
        /// <summary>
        /// URI绝对路径；
        /// </summary>
        public string URI { get; private set; }
        /// <summary>
        /// 本地资源的绝对路径；
        /// </summary>
        public string DownloadPath{ get; private set; }
        /// <summary>
        /// 下载任务的构造函数；
        /// </summary>
        /// <param name="uri">URI绝对路径</param>
        /// <param name="downloadPath">本地资源的绝对路径</param>
        public DownloadTask(string uri, string downloadPath)
        {
            URI = uri;
            DownloadPath = downloadPath;
        }
        public bool Equals(DownloadTask other)
        {
            bool result = false;
            if (this.GetType() == other.GetType())
            {
                result = this.URI==other.URI && this.DownloadPath==other.DownloadPath;
            }
            return result;
        }
    }
}
