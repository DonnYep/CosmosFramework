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
        IDownloadFileListHelper downloadFileListHelper;

        string url;

        string downloadPath;

        bool isDone;

        public string URL { get { return url; } }

        public string DownloadPath { get { return downloadPath; } }
        /// <summary>
        /// 可下载资源的数量；
        /// </summary>
        public int DownloadableCount { get; private set; }
        public bool Downloading { get; private set; }

        public void SetHelper(IDownloadFileListHelper helper)
        {
            downloadFileListHelper = helper;
        }
        public void SetDownloadInfo(string url,string fileListContext,string downloadPath)
        {
            this.url = url;
            var fileList = downloadFileListHelper.ParseDownloadFileList(fileListContext);
            this.downloadPath = downloadPath;
        }
        public void StartDownload()
        {

        }
    }
}
