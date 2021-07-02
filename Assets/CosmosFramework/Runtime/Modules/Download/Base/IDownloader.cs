using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    //================================================
    //1、下载帮助体只实现单文件下载，多文件下载需由downloader调度完成；
    //================================================
    public interface IDownloader
    {
        event Action<DownloadStartEventArgs> DownloadStart;
        event Action<DownloadSuccessEventArgs> DownloadSuccess;
        event Action<DownloadFailureEventArgs> DownloadFailure;
        event Action<DonwloadOverallEventArgs> DownloadOverall;
        event Action<DownloadFinishEventArgs> DownloadFinish;
        /// <summary>
        /// 下载到本地的路径；
        /// </summary>
        string DownloadPath { get; }
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        bool Downloading { get; }
        /// <summary>
        /// 是否删除本地下载失败的文件；
        /// </summary>
        bool DeleteFailureFile { get; set; }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        int DownloadableCount { get; }
        /// <summary>
        /// 资源的地址；
        /// </summary>
        string URL { get; }
        /// <summary>
        /// 下载过期时间；
        /// </summary>
        int DownloadTimeout { get; }
        /// <summary>
        /// 终止下载，谨慎使用；
        /// </summary>
        void AbortDownload();
        /// <summary>
        /// 异步下载；
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="downloadPath">下载到本地的地址</param>
        /// <param name="downloadableList">可下载的文件列表</param>
        /// <param name="timeout">文件下载过期时间</param>
        void Download(string url, string downloadPath, string[] downloadableList, int timeout = 0);
        /// <summary>
        /// 下载轮询，需要由外部调用；
        /// </summary>
        void TickRefresh();
    }
}
