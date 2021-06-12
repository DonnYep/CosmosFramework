using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 下载任务；
    /// </summary>
    public class DownloadTask:IReference
    {
        /// <summary>
        /// 资源所在的地址；
        /// </summary>
        public string Uri { get; private set; }
        /// <summary>
        /// 资源下载后存储的地址；
        /// </summary>
        public string DownloadPath { get; private  set; }
        /// <summary>
        /// 任务过期时间；
        /// </summary>
        public float Timeout { get; private set; }
        public void Release()
        {
            Uri = string.Empty;
            DownloadPath = string.Empty;
            Timeout = 0;
        }
        public static DownloadTask Create(string uri,string downloadPath,float timeout)
        {
            var downloadTask = ReferencePool.Accquire<DownloadTask>();
            downloadTask.Uri = uri;
            downloadTask.DownloadPath = downloadPath;
            downloadTask.Timeout = timeout;
            return downloadTask;
        }
        public static void Release(DownloadTask downloadTask)
        {
            ReferencePool.Release(downloadTask);
        }
    }
}
