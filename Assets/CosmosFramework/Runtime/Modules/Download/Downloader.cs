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
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        protected Action<DownloadStartEventArgs> downloadStart;
        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        protected Action<DownloadSuccessEventArgs> downloadSuccess;
        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        protected Action<DownloadFailureEventArgs> downloadFailure;
        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        protected Action<DonwloadOverallEventArgs> downloadOverall;
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        protected Action<DownloadAndWriteFinishEventArgs> downloadAndWriteFinish;
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
        public event Action<DownloadAndWriteFinishEventArgs> DownloadAndWriteFinish
        {
            add { downloadAndWriteFinish += value; }
            remove { downloadAndWriteFinish -= value; }
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

        protected DateTime writeStartTime;
        protected DateTime writeEndTime;

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
        /// 下载进度的缓存；
        /// </summary>
        Dictionary<string, DownloadedData> dataCacheDict = new Dictionary<string, DownloadedData>();
        /// <summary>
        /// URI===[[缓存的长度===写入本地的长度]]；
        /// 数据写入记录；
        /// </summary>
        Dictionary<string, DownloadWriteInfo> dataWriteDict = new Dictionary<string, DownloadWriteInfo>();
        /// <summary>
        /// 遍历用的DownloadWriteInfo缓存；
        /// </summary>
        List<DownloadWriteInfo> writeInfoCache = new List<DownloadWriteInfo>();
        /// <summary>
        /// 未写入DownloadWriteInfo的缓存；
        /// </summary>
        List<DownloadWriteInfo> unwrittenCache = new List<DownloadWriteInfo>();
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
        }
        /// <summary>
        /// 终止下载，谨慎使用；
        /// </summary>
        public void CancelDownload()
        {
            failureURIs.AddRange(pendingURIs);
            pendingURIs.Clear();
            var eventArgs = DownloadAndWriteFinishEventArgs.Create(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
            downloadAndWriteFinish?.Invoke(eventArgs);
            DownloadAndWriteFinishEventArgs.Release(eventArgs);
            canDownload = false;
            CancelWebAsync();
            failureURIs.Clear();
            successURIs.Clear();
            pendingURIs.Clear();
        }
        public virtual void Release()
        {
            downloadStart = null;
            downloadSuccess = null;
            downloadFailure = null;
            downloadOverall = null;
            downloadAndWriteFinish = null;
            downloadConfig.Reset();
            DownloadableCount = 0;
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
                canDownload = false;
                Downloading = false;
                //异步检测完成状态；
                FutureTask.Detection(() =>
                {
                    unwrittenCache.Clear();
                    writeInfoCache.Clear();
                    writeInfoCache.AddRange(dataWriteDict.Values);
                    var length = writeInfoCache.Count;
                    for (int i = 0; i < length; i++)
                    {
                        if (writeInfoCache[i].CachedLength != writeInfoCache[i].WrittenLength)
                        {
                            unwrittenCache.Add(writeInfoCache[i]);
                        }
                    }
                    return unwrittenCache.Count <= 0;
                },
                (ft) =>
                {
                    dataCacheDict.Clear();
                    dataWriteDict.Clear();
                    downloadEndTime = DateTime.Now;
                    var eventArgs = DownloadAndWriteFinishEventArgs.Create(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
                    downloadAndWriteFinish?.Invoke(eventArgs);
                    DownloadAndWriteFinishEventArgs.Release(eventArgs);
                    //清理下载配置缓存；
                    failureURIs.Clear();
                    successURIs.Clear();
                    pendingURIs.Clear();
                });
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
        protected Task WriteDownloadedData(DownloadedData downloadedData)
        {
            var cachedLenth = downloadedData.Data.Length;
            if (dataWriteDict.TryGetValue(downloadedData.URI, out var writeInfo))
            {
                //旧缓存的长度小于新缓存的长度，则更新长度；
                if (writeInfo.CachedLength >= cachedLenth)
                    return null;
                dataCacheDict.AddOrUpdate(downloadedData.URI, downloadedData);
                //缓存新数据的长度，保留原来写入的长度；
                dataWriteDict.AddOrUpdate(downloadedData.URI, new DownloadWriteInfo(cachedLenth, writeInfo.WrittenLength));
                return Task.Run(() =>
                {
                    Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
                    dataWriteDict.AddOrUpdate(downloadedData.URI, new DownloadWriteInfo(cachedLenth, cachedLenth));
                });
            }
            else
            {
                dataCacheDict.Add(downloadedData.URI, downloadedData);
                //缓存新数据长度，原始写入长度设置为0，表示为写入过；
                dataWriteDict.Add(downloadedData.URI, new DownloadWriteInfo(cachedLenth, 0));
                return Task.Run(() =>
                {
                    Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
                    dataWriteDict.AddOrUpdate(downloadedData.URI, new DownloadWriteInfo(cachedLenth, cachedLenth));
                });
            }
        }
    }
}
