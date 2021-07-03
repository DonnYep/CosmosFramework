using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cosmos;
namespace Cosmos.Download
{
    //[Module]
    internal class DownloadManager : Module, IDownloadManager
    {
        #region events
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;
        Action<DonwloadOverallEventArgs> downloadOverall;
        Action<DownloadFinishEventArgs> downloadFinish;
        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        public event Action<DownloadSuccessEventArgs> DownloadSuccess
        {
            add { downloadSuccess += value; }
            remove { downloadSuccess -= value; }
        }
        public event Action<DownloadFailureEventArgs> DownloadFailure
        {
            add { downloadFailure += value; }
            remove { downloadFailure -= value; }
        }
        public event Action<DonwloadOverallEventArgs> DownloadOverall
        {
            add { downloadOverall += value; }
            remove { downloadOverall -= value; }
        }
        public event Action<DownloadFinishEventArgs> DownloadFinish
        {
            add { downloadFinish += value; }
            remove { downloadFinish -= value; }
        }
        #endregion
        /// <summary>
        /// 下载到本地的路径；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get; private set; }
        /// <summary>
        /// 是否删除本地下载失败的文件；
        /// </summary>
        public bool DeleteFailureFile { get; set; }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        public int DownloadableCount { get; private set; }
        /// <summary>
        /// 资源的地址；
        /// </summary>
        public string URL { get; private set; }
        /// <summary>
        /// 下载过期时间；
        /// </summary>
        public float DownloadTimeout { get; private set; }

        /// <summary>
        /// 下载模式；
        /// </summary>
        public DownloaderMode DownloaderMode { get; private set; }
        /// <summary>
        /// 下载器；
        /// </summary>
        Downloader downloader;
        public override void OnPreparatory()
        {
            downloader = new UnityWebDownloader();
        }
        public void SetDownloader(Downloader downloader)
        {
            this.downloader = downloader;
        }
        public void SetDownloaderAsync(DownloaderMode downloaderMode)
        {
            DownloaderMode = downloaderMode;
            FutureTask.Detection(() => downloader.Downloading,(ft)=> 
            {
                switch (downloaderMode)
                {
                    case DownloaderMode.UnityWebRequest:
                        downloader = new UnityWebDownloader();
                        break;
                    case DownloaderMode.WebClient:
                        downloader = new WebClientDownloader();
                        break;
                }
            });
        }
        public void CancelDownload()
        {
            downloader.CancelDownload();
        }
        public void InitDownloader(string url, string downloadPath, float timeout)
        {
            downloader.InitDownloader(url, downloadPath, timeout);
        }
        public void Download(string[] downloadableList)
        {
            downloader.Download(downloadableList);
        }
        public void Clear()
        {
            DownloadPath = string.Empty;
        }
        [TickRefresh]
        void TickRefresh()
        {
            downloader?.TickRefresh();
        }
    }
}
