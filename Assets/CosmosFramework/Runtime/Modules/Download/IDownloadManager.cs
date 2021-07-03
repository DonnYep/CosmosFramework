using Cosmos.Download;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface IDownloadManager : IModuleManager
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
        float DownloadTimeout { get; }
        /// <summary>
        /// 下载模式；
        /// </summary>
        DownloaderMode DownloaderMode { get; }
        /// <summary>
        /// 取消下载；
        /// </summary>
        void CancelDownload();
        /// <summary>
        /// 初始化下载器；
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="downloadPath">本地持久化路径</param>
        /// <param name="timeout">过期时间</param>
        void InitDownloader(string url, string downloadPath, float timeout);
        /// <summary>
        /// 资源下载；
        /// </summary>
        /// <param name="downloadableList">资源文件列表</param>
        void Download(string[] downloadableList);
    }
}
