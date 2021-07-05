using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using System.Net;

namespace Cosmos.Download
{
    /// <summary>
    /// 文件下载器；
    /// </summary>
    public abstract class Downloader
    {
        #region events
        protected Action<DownloadStartEventArgs> downloadStart;
        protected Action<DownloadSuccessEventArgs> downloadSuccess;
        protected Action<DownloadFailureEventArgs> downloadFailure;
        protected Action<DonwloadOverallEventArgs> downloadOverall;
        protected Action<DownloadFinishEventArgs> downloadFinish;
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
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get; protected set; }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        public int DownloadableCount { get; protected set; }

        protected List<string> pendingURIs = new List<string>();
        protected List<string> successURIs = new List<string>();
        protected List<string> failureURIs = new List<string>();

        protected DateTime downloadStartTime;
        protected DateTime downloadEndTime;

        protected DownloadConfig downloadConfig;
        /// <summary>
        /// 单位资源的百分比比率；
        /// </summary>
        protected float unitResRatio;
        /// <summary>
        /// 当前下载的序号；
        /// </summary>
        protected int currentDownloadIndex = 0;
        /// <summary>
        /// 当前是否可下载；
        /// </summary>
        protected bool canDownload;
        /// <summary>
        /// 是否可写入本地；
        /// </summary>
        protected bool canWrite;
        /// <summary>
        /// 下载进度的缓存；
        /// </summary>
        Dictionary<string, DownloadedData> dataCacheDict = new Dictionary<string, DownloadedData>();

        public void SetDownloadConfig(DownloadConfig downloadConfig)
        {
            this.downloadConfig = downloadConfig;
            pendingURIs.AddRange(downloadConfig.FileList);
            DownloadableCount = downloadConfig.FileList.Length;
            unitResRatio = 100f / DownloadableCount;
        }
        /// <summary>
        /// 启动下载；
        /// </summary>
        public void LaunchDownload()
        {
            canDownload = true;
            if (pendingURIs.Count == 0 || !canDownload)
                return;
            Downloading = true;
            downloadStartTime = DateTime.Now;
            RecursiveDownload();
            canWrite = true;
        }
        /// <summary>
        /// 下载轮询，需要由外部调用；
        /// </summary>
        public async void TickRefresh()
        {
            if (!canWrite && !canDownload)
                return;
            if (dataCacheDict.Count > 0)
            {
                var data = dataCacheDict.First().Value;
                dataCacheDict.Remove(data.URI);
                await Task.Run(() =>
                {
                    try
                    {
                        Utility.IO.WriteFile(data.Data, data.DownloadPath);
                    }
                    catch { }
                });
                canWrite = dataCacheDict.Count > 0 ? true : false;
            }
        }
        /// <summary>
        /// 终止下载，谨慎使用；
        /// </summary>
        public void CancelDownload()
        {
            failureURIs.AddRange(pendingURIs);
            pendingURIs.Clear();
            var eventArgs = DownloadFinishEventArgs.Create(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
            downloadFinish?.Invoke(eventArgs);
            DownloadFinishEventArgs.Release(eventArgs);
            canDownload = false;
            CancelWebAsync();
        }
        public virtual void Reset()
        {
            downloadStart = null;
            downloadSuccess = null;
            downloadFailure = null;
            downloadOverall = null;
            downloadFinish = null;
            downloadConfig.Reset();
            DownloadableCount = 0;
            canWrite = false;
        }
        /// <summary>
        /// 处理整体进度；
        /// individualPercent 为0~1；
        /// </summary>
        /// <param name="uri">资源地址</param>
        /// <param name="downloadPath">下载到本地的目录</param>
        /// <param name="individualPercent">资源个体百分比0~1</param>
        protected void ProcessOverallProgress(string uri, string downloadPath, float individualPercent)
        {
            var overallIndexPercent = 100 * ((float)currentDownloadIndex / DownloadableCount);
            var overallProgress = overallIndexPercent + (unitResRatio * (individualPercent));
            var eventArgs = DonwloadOverallEventArgs.Create(uri, downloadPath, overallProgress, individualPercent);
            downloadOverall.Invoke(eventArgs);
            DonwloadOverallEventArgs.Release(eventArgs);
        }
        protected async void RecursiveDownload()
        {
            if (pendingURIs.Count == 0)
            {
                downloadEndTime = DateTime.Now;
                var eventArgs = DownloadFinishEventArgs.Create(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
                downloadFinish?.Invoke(eventArgs);
                DownloadFinishEventArgs.Release(eventArgs);
                canDownload = false;
                Downloading = false;
                return;
            }
            string downloadableUri = pendingURIs[0];
            currentDownloadIndex = DownloadableCount - pendingURIs.Count;
            var fileDownloadPath = Path.Combine(downloadConfig.DownloadPath, downloadableUri);
            pendingURIs.RemoveAt(0);
            if (canDownload)
            {
                var remoteUri = Utility.IO.WebPathCombine(downloadConfig.URL, downloadableUri);
                await WebDownload(remoteUri, fileDownloadPath);
                RecursiveDownload();
            }
        }
        protected abstract void CancelWebAsync();
        protected abstract IEnumerator WebDownload(string uri, string fileDownloadPath);
        protected void CacheDownloadedData(DownloadedData downloadedData)
        {
            if (dataCacheDict.TryGetValue(downloadedData.URI, out var data))
            {
                if (data.Data.Length < downloadedData.Data.Length)
                    dataCacheDict[downloadedData.URI] = downloadedData;
            }
            else
                dataCacheDict.Add(downloadedData.URI, downloadedData);
        }
    }
}
