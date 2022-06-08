using System;
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
    *
    *5、若不自定义设置下载器，(调用SetOrUpdateDownloadHelper方法)，则
    * 使用框架原生默认的下载器；
     */
    //================================================
    [Module]
    internal class DownloadManager : Module, IDownloadManager
    {
        #region events
        ///<inheritdoc/>
        public event Action<DownloadStartEventArgs> OnDownloadStart
        {
            add { downloader.OnDownloadStart += value; }
            remove { downloader.OnDownloadStart -= value; }
        }
        ///<inheritdoc/>
        public event Action<DownloadSuccessEventArgs> OnDownloadSuccess
        {
            add { downloader.OnDownloadSuccess += value; }
            remove { downloader.OnDownloadSuccess -= value; }
        }
        ///<inheritdoc/>
        public event Action<DownloadFailureEventArgs> OnDownloadFailure
        {
            add { downloader.OnDownloadFailure += value; }
            remove { downloader.OnDownloadFailure -= value; }
        }
        ///<inheritdoc/>
        public event Action<DonwloadOverallEventArgs> OnDownloadOverall
        {
            add { downloader.OnDownloadOverall += value; }
            remove { downloader.OnDownloadOverall -= value; }
        }
        ///<inheritdoc/>
        public event Action<DownloadAndWriteFinishEventArgs> OnDownloadAndWriteFinish
        {
            add { downloader.OnDownloadAndWriteFinish += value; }
            remove { downloader.OnDownloadAndWriteFinish -= value; }
        }
        #endregion
        ///<inheritdoc/>
        public bool DeleteFailureFile
        {
            get { return downloader.DeleteFailureFile; }
            set
            {
                downloader.DeleteFailureFile = value;
                deleteFailureFile = value;
            }
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public bool Downloading { get { return downloader.Downloading; } }
        ///<inheritdoc/>
        public int DownloadingCount { get { return downloader.DownloadingCount; } }

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

        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public void SetUrlHelper(IDownloadUrlHelper helper)
        {
            this.downloadUrlHelper = helper;
        }
        ///<inheritdoc/>
        public void SetRequesterHelper(IDownloadRequester helper)
        {
            this.downloadRequester = helper;
        }
        ///<inheritdoc/>
        public void AddUriDownload(string uri, string downloadPath)
        {
            Utility.Text.IsStringValid(uri, "URI is invalid !");
            Utility.Text.IsStringValid(downloadPath, "DownloadPath is invalid !");
            downloader.AddUriDownload(uri, downloadPath);
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public void RemoveUriDownload(string uri)
        {
            Utility.Text.IsStringValid(uri, "URI is invalid !");
            downloader.RemoveUriDownload(uri);
        }
        ///<inheritdoc/>
        public void RemoveUrisDownload(string[] uris)
        {
            if (uris == null)
                throw new ArgumentNullException("URIs is invalid !");
            var length = uris.Length;
            for (int i = 0; i < length; i++)
                RemoveUriDownload(uris[i]);
        }
        ///<inheritdoc/>
        public void RemoveAllDownload()
        {
            downloader.RemoveAllDownload();
        }
        ///<inheritdoc/>
        public void GetUriFileSizeAsync(string uri, Action<long> callback)
        {
            Utility.Text.IsStringValid(uri, "URI is invalid !");
            if (callback == null)
                throw new ArgumentNullException("Callback is invalid !");
            downloadRequester.GetUriFileSizeAsync(uri, callback);
        }
        ///<inheritdoc/>
        public void GetUrlFilesSizeAsync(string url, Action<long> callback)
        {
            Utility.Text.IsStringValid(url, "URL is invalid !");
            var relUris = downloadUrlHelper.ParseUrlToRelativeUris(url);
            downloadRequester.GetUriFilesSizeAsync(relUris, callback);
        }
        ///<inheritdoc/>
        public void LaunchDownload()
        {
            downloader.LaunchDownload();
        }
        ///<inheritdoc/>
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
        protected override void OnTermination()
        {
            downloader.CancelDownload();
        }
    }
}
