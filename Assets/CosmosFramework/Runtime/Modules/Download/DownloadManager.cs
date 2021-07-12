using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cosmos;
namespace Cosmos.Download
{
    //================================================
    //1、下载模块提供一键式资源下载、本地写入功能。通过监听开放的接口可
    //以检测下载进度；
    //2、下载模块支持localhost文件下载与http地址文件的下载。模块下载到本
    //地后，由多线程增量写入，以防下载错误导致的整体文件丢失。
    //3、下载模块支持断点续下。
    //4、支持动态添加、移除、暂停、恢复下载任务；
    //================================================
    [Module]
    internal class DownloadManager : Module, IDownloadManager
    {
        #region events
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { downloader.DownloadStart += value; }
            remove { downloader.DownloadStart -= value; }
        }
        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        public event Action<DownloadSuccessEventArgs> DownloadSuccess
        {
            add { downloader.DownloadSuccess += value; }
            remove { downloader.DownloadSuccess -= value; }
        }
        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        public event Action<DownloadFailureEventArgs> DownloadFailure
        {
            add { downloader.DownloadFailure += value; }
            remove { downloader.DownloadFailure -= value; }
        }
        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        public event Action<DonwloadOverallEventArgs> DownloadOverall
        {
            add { downloader.DownloadOverall += value; }
            remove { downloader.DownloadOverall -= value; }
        }
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        public event Action<DownloadAndWriteFinishEventArgs> DownloadAndWriteFinish
        {
            add { downloader.DownloadAndWriteFinish += value; }
            remove { downloader.DownloadAndWriteFinish -= value; }
        }
        #endregion
        /// <summary>
        /// 是否删除本地下载失败的文件；
        /// </summary>
        public bool DeleteFailureFile
        {
            get { return downloader.DeleteFailureFile; }
            set
            {
                downloader.DeleteFailureFile = value;
                deleteFailureFile = value;
            }
        }
        /// <summary>
        /// 任务过期时间，以秒为单位；
        /// </summary>
        public float DownloadTimeout
        {
            get { return downloader.DownloadTimeout; }
            set
            {
                if (value <= 0)
                    downloadTimeout = 0;
                downloader.DownloadTimeout = downloadTimeout;
            }
        }
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get { return downloader.Downloading; } }
        /// <summary>
        ///  下载中的资源总数；
        /// </summary>
        public int DownloadingCount { get { return downloader.DownloadingCount; } }
        /// <summary>
        /// 下载模式；
        /// </summary>
        public DownloaderMode DownloaderMode { get; private set; }
        /// <summary>
        /// 是否删除本地下载失败的文件；
        /// </summary>
        bool deleteFailureFile;
        /// <summary>
        ///任务过期时间，以秒为单位
        /// </summary>
        float downloadTimeout;
        /// <summary>
        /// 下载器；
        /// </summary>
        IDownloader downloader;
        /// <summary>
        /// 下载资源地址帮助体；用于解析URL中的文件列表；
        /// 支持localhost地址与http地址；
        /// </summary>
        IDownloadUrlHelper downloadUrlHelper;
        /// <summary>
        /// 下载器缓存；
        /// </summary>
        Dictionary<DownloaderMode, IDownloader> downloaderDict;

        /// <summary>
        /// 可以用，但是没必要；
        /// 切换下载模式，切换下载器后会保留先前的的下载配置；
        /// 此操作为异步处理，当有个下载器正在下载时，等到下载器下载停止再切换；
        /// <see cref="Cosmos.Download. DownloaderMode"/>
        /// </summary>
        /// <param name="downloaderMode">下载模式</param>
        public void SwitchDownloadMode(DownloaderMode downloaderMode)
        {
            if (DownloaderMode == downloaderMode)
                return;
            if (downloader != null)
            {
                FutureTask.Detection(() => downloader.Downloading, (ft) =>
                 {
                     downloader.Release();
                     downloader = downloaderDict[downloaderMode];
                     DownloaderMode = downloaderMode;
                     downloader.DeleteFailureFile = deleteFailureFile;
                     downloader.DownloadTimeout = downloadTimeout;
                 });
            }
            else
            {
                downloader = downloaderDict[downloaderMode];
                DownloaderMode = downloaderMode;
                downloader.DeleteFailureFile = deleteFailureFile;
                downloader.DownloadTimeout = downloadTimeout;
            }
        }
        /// <summary>
        /// 设置下载资源地址帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        public void SetIUrlHelper(IDownloadUrlHelper helper)
        {
            this.downloadUrlHelper = helper;
        }
        /// <summary>
        /// 添加URI下载；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        /// <param name="downloadPath">下载到地址的绝对路径</param>
        public void AddUriDownload(string uri, string downloadPath)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException("URI is invalid !");
            if (string.IsNullOrEmpty(downloadPath))
                throw new ArgumentNullException("DownloadPath is invalid !");
            downloader.AddUriDownload(uri, downloadPath);
        }
        /// <summary>
        /// 将URL添加到下载队列，并下载当前URL页面下的所有文件到本地；
        /// </summary>
        /// <param name="url">统一资源定位符</param>
        /// <param name="downloadRootPath">下载到地址的根目录</param>
        public void AddUrlDownload(string url, string downloadRootPath)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("URL is invalid !");
            if (string.IsNullOrEmpty(downloadRootPath))
                throw new ArgumentNullException("DownloadRootPath is invalid !");
            var relUris = downloadUrlHelper.ParseUrlToRelativeUris(url);
            var length = relUris.Length;
            for (int i = 0; i < length; i++)
            {
                var absUri = Path.Combine(url, relUris[i]);
                var absDownloadPath = Path.Combine(downloadRootPath, relUris[i]);
                AddUriDownload(absUri, absDownloadPath);
            }
        }
        /// <summary>
        /// 移除URI下载；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        public void RemoveUriDownload(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException("URI is invalid !");
            downloader.RemoveUriDownload(uri);
        }
        /// <summary>
        /// 移除多个URI下载；
        /// </summary>
        /// <param name="uris">统一资源名称数组</param>
        public void RemoveUrisDownload(string[] uris)
        {
            if (uris == null)
                throw new ArgumentNullException("URIs is invalid !");
            var length = uris.Length;
            for (int i = 0; i < length; i++)
                RemoveUriDownload(uris[i]);
        }
        /// <summary>
        /// 移除所有下载；
        /// </summary>
        public void RemoveAllDownload()
        {
            downloader.RemoveAllDownload();
        }
        /// <summary>
        /// 设置完成下载配置后启动下载；
        public void LaunchDownload()
        {
            downloader.LaunchDownload();
        }
        /// <summary>
        /// 取消下载
        /// </summary>
        public void CancelDownload()
        {
            downloader.CancelDownload();
        }
        protected override void OnPreparatory()
        {
            downloaderDict = new Dictionary<DownloaderMode, IDownloader>();
            var unityWebDownloader = new UnityWebDownloader();
            var webClientDownloader = new WebClientDownloader();
            downloaderDict.Add(DownloaderMode.UnityWebRequest, unityWebDownloader);
            downloaderDict.Add(DownloaderMode.WebClient, webClientDownloader);
            downloader = unityWebDownloader;
            downloader.DeleteFailureFile = deleteFailureFile;
            downloader.DownloadTimeout = downloadTimeout;
            downloadUrlHelper = new DefaultDownloadUrlHelper();
        }
    }
}
