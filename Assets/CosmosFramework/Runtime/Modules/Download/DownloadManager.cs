using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Cosmos.Download
{
    //================================================
    /*
     * 1、下载模块提供一键式资源下载、本地写入功能。通过监听开放的接口可
    * 以检测下载进度；
    * 
    * 2、载模块支持localhost文件下载与http地址文件的下载。模块下载到本
    *后，增量写入，以防下载错误导致的整体文件丢失。
    *
    *3、载模块支持断点续下。
    *
    *4、支持动态添加、移除、暂停、恢复下载任务；
     */
    //================================================
    [Module]
    internal class DownloadManager : Module, IDownloadManager
    {
        #region events
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        public event Action<DownloadStartEventArgs> OnDownloadStart
        {
            add { downloader.OnDownloadStart += value; }
            remove { downloader.OnDownloadStart -= value; }
        }
        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        public event Action<DownloadSuccessEventArgs> OnDownloadSuccess
        {
            add { downloader.OnDownloadSuccess += value; }
            remove { downloader.OnDownloadSuccess -= value; }
        }
        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        public event Action<DownloadFailureEventArgs> OnDownloadFailure
        {
            add { downloader.OnDownloadFailure += value; }
            remove { downloader.OnDownloadFailure -= value; }
        }
        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        public event Action<DonwloadOverallEventArgs> OnDownloadOverall
        {
            add { downloader.OnDownloadOverall += value; }
            remove { downloader.OnDownloadOverall -= value; }
        }
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        public event Action<DownloadAndWriteFinishEventArgs> OnDownloadAndWriteFinish
        {
            add { downloader.OnDownloadAndWriteFinish += value; }
            remove { downloader.OnDownloadAndWriteFinish -= value; }
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
        /// 下载请求器，专门用于获取文件大小；
        /// </summary>
        IDownloadRequester downloadRequester;
        /// <summary>
        /// 下载资源地址帮助体；用于解析URL中的文件列表；
        /// 支持localhost地址与http地址；
        /// </summary>
        IDownloadUrlHelper downloadUrlHelper;

        /// <summary>
        /// 设置或更新downloader;
        /// </summary>
        /// <param name="newDownloader">下载器</param>
        public void SetOrUpdateDownloadHelper(IDownloader newDownloader)
        {
            if (this.downloader != null)
            {
                this.downloader.CancelDownload();
                FutureTask.Detection(() => this.downloader.Downloading, (ft) =>
                {
                    this.downloader.Release();
                    this.downloader = newDownloader;
                    this.downloader.DeleteFailureFile = deleteFailureFile;
                    this.downloader.DownloadTimeout = downloadTimeout;
                });
            }
        }
        /// <summary>
        /// 设置下载资源地址帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        public void SetUrlHelper(IDownloadUrlHelper helper)
        {
            this.downloadUrlHelper = helper;
        }
        /// <summary>
        /// 设置资源请求帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        public void SetRequesterHelper(IDownloadRequester helper)
        {
            this.downloadRequester = helper;
        }
        /// <summary>
        /// 添加URI下载；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        /// <param name="downloadPath">下载到地址的绝对路径</param>
        public void AddUriDownload(string uri, string downloadPath)
        {
            Utility.Text.IsStringValid(uri, "URI is invalid !");
            Utility.Text.IsStringValid(downloadPath, "DownloadPath is invalid !");
            downloader.AddUriDownload(uri, downloadPath);
        }
        /// <summary>
        /// 将URL添加到下载队列，并下载当前URL页面下的所有文件到本地；
        /// </summary>
        /// <param name="url">统一资源定位符</param>
        /// <param name="downloadRootPath">下载到地址的根目录</param>
        public void AddUrlDownload(string url, string downloadRootPath)
        {
            Utility.Text.IsStringValid(url, "DownloadPath is invalid !");
            Utility.Text.IsStringValid(downloadRootPath, "DownloadRootPath is invalid !");
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
            Utility.Text.IsStringValid(uri, "URI is invalid !");
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
        /// 获取URI单个文件的大小；
        /// 若获取到，则回调传入正确的数值，否则就传入-1；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        /// <param name="callback">回调</param>
        public void GetUriFileSizeAsync(string uri, Action<long> callback)
        {
            Utility.Text.IsStringValid(uri, "URI is invalid !");
            if (callback == null)
                throw new ArgumentNullException("Callback is invalid !");
            downloadRequester.GetUriFileSizeAsync(uri, callback);
        }
        /// <summary>
        /// 获取一个URL地址下的所有文件的总和大小；
        /// 若获取到，则回调传入正确的数值，否则就传入-1；
        /// </summary>
        /// <param name="url">统一资源定位符</param>
        /// <param name="callback">回调</param>
        public void GetUrlFilesSizeAsync(string url, Action<long> callback)
        {
            Utility.Text.IsStringValid(url, "URL is invalid !");
            var relUris = downloadUrlHelper.ParseUrlToRelativeUris(url);
            downloadRequester.GetUriFilesSizeAsync(relUris, callback);
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
        protected override void OnInitialization()
        {
            var unityWebDownloader = new UnityWebDownloader();
            downloader = unityWebDownloader;
            downloader.DeleteFailureFile = deleteFailureFile;
            downloader.DownloadTimeout = downloadTimeout;
            downloadUrlHelper = new DefaultDownloadUrlHelper();
            downloadRequester = new DefaultDownloadRequester();
        }
    }
}
