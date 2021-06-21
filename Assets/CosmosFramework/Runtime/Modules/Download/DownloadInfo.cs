using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class DownloadInfo:IReference
    {
        /// <summary>
        /// Remote资源所在的地址；
        /// </summary>
        public string Uri { get; private set; }
        /// <summary>
        /// Local资源下载后存储的地址；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// 任务过期时间，以秒为单位；
        /// </summary>
        public int Timeout { get; private set; }
        public DownloadInfo(string uri, string downloadPath, int timeout)
        {
            Uri = uri;
            DownloadPath = downloadPath;
            if (timeout <= 0)
                Timeout = 0;
            else
                Timeout = timeout;
        }
        public void Release()
        {
            Uri = string.Empty;
            DownloadPath = string.Empty;
            Timeout = 0;
        }
    }
}
