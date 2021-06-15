using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class DownloadTasks : IReference
    {
        /// <summary>
        /// 资源组；
        /// </summary>
        public string[] Uris { get; private set; }
        /// <summary>
        /// 资源下载后存储的地址；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// 任务过期时间；
        /// </summary>
        public float Timeout { get; private set; }
        public void Release()
        {
            Uris = null; 
            DownloadPath = string.Empty;
            Timeout = 0;
        }
        public static DownloadTasks Create(string[] uris, string downloadPath, float timeout)
        {
            var task = ReferencePool.Accquire<DownloadTasks>();
            task.Uris = uris;
            task.DownloadPath = downloadPath;
            task.Timeout = timeout;
            return task;
        }
        public static void Release(DownloadTasks task)
        {
            ReferencePool.Release(task);
        }
    }
}
