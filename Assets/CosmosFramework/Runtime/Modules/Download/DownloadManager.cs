using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cosmos;
namespace Cosmos.Download
{
    [Module]
    internal class DownloadManager : Module, IDownloadManager
    {
        #region events
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { currentDownloader.DownloadStart += value; }
            remove { currentDownloader.DownloadStart -= value; }
        }
        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        public event Action<DownloadSuccessEventArgs> DownloadSuccess
        {
            add { currentDownloader.DownloadSuccess += value; }
            remove { currentDownloader.DownloadSuccess -= value; }
        }
        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        public event Action<DownloadFailureEventArgs> DownloadFailure
        {
            add { currentDownloader.DownloadFailure += value; }
            remove { currentDownloader.DownloadFailure -= value; }
        }
        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        public event Action<DonwloadOverallEventArgs> DownloadOverall
        {
            add { currentDownloader.DownloadOverall += value; }
            remove { currentDownloader.DownloadOverall -= value; }
        }
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        public event Action<DownloadAndWriteFinishEventArgs> DownloadAndWriteFinish
        {
            add { currentDownloader.DownloadAndWriteFinish += value; }
            remove { currentDownloader.DownloadAndWriteFinish -= value; }
        }
        #endregion
        /// <summary>
        /// 下载模块配置；
        /// </summary>
        public DownloadConfig DownloadConfig { get; private set; }
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get { return currentDownloader.Downloading; } }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        public int DownloadableCount { get { return currentDownloader.DownloadableCount; } }
        /// <summary>
        /// 下载模式；
        /// </summary>
        public DownloaderMode DownloaderMode { get; private set; }
        /// <summary>
        /// 下载器；
        /// </summary>
        Downloader currentDownloader;
        /// <summary>
        /// 下载器缓存；
        /// </summary>
        Dictionary<DownloaderMode, Downloader> downloaderDict;
        public override void OnPreparatory()
        {
            downloaderDict = new Dictionary<DownloaderMode, Downloader>();
            var unityWebDownloader = new UnityWebDownloader();
            var webClientDownloader = new WebClientDownloader();
            downloaderDict.Add(DownloaderMode.UnityWebRequest, unityWebDownloader);
            downloaderDict.Add(DownloaderMode.WebClient, webClientDownloader);
            currentDownloader = unityWebDownloader;
        }
        /// <summary>
        /// 切换下载模式；
        /// 此操作为异步处理，当有个下载器正在下载时，等到下载器下载停止再切换；
        /// <see cref="Cosmos.Download. DownloaderMode"/>
        /// </summary>
        /// <param name="downloaderMode">下载模式</param>
        public void SwitchDownloadMode(DownloaderMode downloaderMode)
        {
            if (DownloaderMode == downloaderMode)
                return;
            if (currentDownloader != null)
            {
                FutureTask.Detection(() => currentDownloader.Downloading, (ft) =>
                 {
                     currentDownloader.Release();
                     currentDownloader = downloaderDict[downloaderMode];
                     DownloaderMode = downloaderMode;
                 });
            }
            else
            {
                currentDownloader = downloaderDict[downloaderMode];
                DownloaderMode = downloaderMode;
            }
        }
        /// <summary>
        /// 设置下载配置；
        /// <see cref="Cosmos.Download. DownloadConfig"/>
        /// </summary>
        /// <param name="downloadConfig">下载配置</param>
        public void SetDownloadConfig(DownloadConfig downloadConfig)
        {
            DownloadConfig = downloadConfig;
            currentDownloader.SetDownloadConfig(downloadConfig);
        }
        /// <summary>
        /// 设置完成下载配置后启动下载；
        public void LaunchDownload()
        {
            if (DownloadConfig == null)
                throw new ArgumentNullException("DownloadConfig is invalid !");
            currentDownloader.LaunchDownload();
        }
        /// <summary>
        /// 取消下载
        /// </summary>
        public void CancelDownload()
        {
            currentDownloader.CancelDownload();
        }
        /// <summary>
        /// 重置下载配置‘
        /// </summary>
        public void ResetDownloadConfig()
        {
            DownloadConfig?.Reset();
        }
    }
}
