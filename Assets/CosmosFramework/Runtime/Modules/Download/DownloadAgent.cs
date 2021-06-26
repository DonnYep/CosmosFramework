using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    /// <summary>
    /// 下载代理类；
    /// 支持多文件异步下载、多线程本地写入；
    /// </summary>
    public class DownloadAgent
    {
        /// <summary>
        /// key=> uri ; value=>fileName ;
        /// </summary>
        Dictionary<string, string> uriNameDict;

        readonly string url;
        readonly string downloadPath;
        int TargetCount;
        bool isDone;
        public string URL { get { return url; } }
        public string DownloadPath { get { return downloadPath; } }
        /// <summary>
        /// 可下载资源的数量；
        /// </summary>
        public int DownloadableCount { get; private set; }
        public bool Downloading { get; set; }
        public DownloadAgent(string url,string downloadPath)
        {
            this.url = url;
            this.downloadPath= downloadPath;
        }
        public bool Download(Dictionary<string, string> uriNameDict)
        {
            return true;
        }
    }
}
