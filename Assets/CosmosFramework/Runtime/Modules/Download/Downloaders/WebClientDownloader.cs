using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public class WebClientDownloader : IDownloader
    {
        #region events
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;
        Action<DonwloadOverallEventArgs> downloadOverall;
        Action<DownloadAndWriteFinishEventArgs> downloadAndWriteFinish;
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        public event Action<DownloadSuccessEventArgs> DownloadSuccess
        {
            add { downloadSuccess += value; }
            remove { downloadSuccess -= value; }
        }
        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        public event Action<DownloadFailureEventArgs> DownloadFailure
        {
            add { downloadFailure += value; }
            remove { downloadFailure -= value; }
        }
        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        public event Action<DonwloadOverallEventArgs> DownloadOverall
        {
            add { downloadOverall += value; }
            remove { downloadOverall -= value; }
        }
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
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
        /// <summary>
        /// 挂起的待下载URI；
        /// </summary>
        List<string> pendingURIs = new List<string>();
        /// <summary>
        /// 下载成功的URI；
        /// </summary>
        List<string> successURIs = new List<string>();
        /// <summary>
        /// 下载失败的URI;
        /// </summary>
        List<string> failureURIs = new List<string>();
        /// <summary>
        /// 下载开始时间；
        /// </summary>
        DateTime downloadStartTime;
        /// <summary>
        /// 下载结束时间；
        /// </summary>
        DateTime downloadEndTime;
        /// <summary>
        /// 下载配置；
        /// </summary>
        DownloadConfig downloadConfig;
        /// <summary>
        /// web下载client;
        /// </summary>
        WebClient webClient;
        /// <summary>
        /// 单位资源的百分比比率；
        /// </summary>
        float unitResRatio;
        /// <summary>
        /// 当前下载的序号；
        /// </summary>
        int currentDownloadIndex = 0;
        /// <summary>
        /// 当前是否可下载；
        /// </summary>
        bool canDownload;
        /// <summary>
        /// URI===[[缓存的长度===写入本地的长度]]；
        /// 数据写入记录；
        /// </summary>
        Dictionary<string, DownloadWriteInfo> dataWriteDict = new Dictionary<string, DownloadWriteInfo>();
        /// <summary>
        /// 为下载器进行配置；
        /// </summary>
        /// <param name="downloadConfig">下载配置数据</param>
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
            DownloadMultipleFiles();
        }
        /// <summary>
        /// 取消下载；
        /// </summary>
        public void CancelDownload()
        {
            failureURIs.AddRange(pendingURIs);
            pendingURIs.Clear();
            var eventArgs = DownloadAndWriteFinishEventArgs.Create(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
            downloadAndWriteFinish?.Invoke(eventArgs);
            DownloadAndWriteFinishEventArgs.Release(eventArgs);
            canDownload = false;
            webClient?.CancelAsync();
            failureURIs.Clear();
            successURIs.Clear();
            pendingURIs.Clear();
        }
        /// <summary>
        /// 释放下载器；
        /// </summary>
        public void Release()
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
        /// 多文件下载迭代器方法；
        /// </summary>
        /// <returns>迭代器接口</returns>
        async void DownloadMultipleFiles()
        {
            var length = pendingURIs.Count;
            for (int i = 0; i < length; i++)
            {
                currentDownloadIndex = i;
                var currentUri = pendingURIs[i];
                var fileDownloadPath = Path.Combine(downloadConfig.DownloadPath, currentUri);
                var remoteUri = Utility.IO.WebPathCombine(downloadConfig.URL, currentUri);
                await DownloadSingleFile(remoteUri, fileDownloadPath);
            }
            OnDownloadedPendingFiles();
        }
        /// <summary>
        /// 单文件下载迭代器方法；
        /// </summary>
        /// <param name="uri">文件地址</param>
        /// <param name="fileDownloadPath">本地写入的地址</param>
        /// <returns>迭代器接口</returns>
        async Task DownloadSingleFile(string uri, string fileDownloadPath)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    Downloading = true;
                    this.webClient = webClient;
                    var startEventArgs = DownloadStartEventArgs.Create(uri, fileDownloadPath);
                    downloadStart?.Invoke(startEventArgs);
                    DownloadStartEventArgs.Release(startEventArgs);
                    webClient.DownloadProgressChanged += (sender, eventArgs) =>
                    {
                        OnFileDownloading(uri, downloadConfig.DownloadPath, (float)eventArgs.ProgressPercentage / 100);
                    };
                    webClient.DownloadDataCompleted += (sender, eventArgs) =>
                    {
                        Downloading = false;
                        var resultData = eventArgs.Result;
                        var successEventArgs = DownloadSuccessEventArgs.Create(uri, fileDownloadPath, resultData);
                        downloadSuccess?.Invoke(successEventArgs);
                        OnFileDownloading(uri, downloadConfig.DownloadPath, 1);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        successURIs.Add(uri);
                        var downloadedData = new DownloadedData(uri, resultData, fileDownloadPath);
                        OnDownloadedData(downloadedData);
                    };
                    await webClient.DownloadDataTaskAsync(uri);
                }
                catch (Exception exception)
                {
                    Downloading = false;
                    var failureEventArgs = DownloadFailureEventArgs.Create(uri, fileDownloadPath, exception.Message);
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failureURIs.Add(uri);
                    OnFileDownloading(uri, downloadConfig.DownloadPath, 1);
                    if (downloadConfig.DeleteFailureFile)
                    {
                        Utility.IO.DeleteFile(fileDownloadPath);
                    }
                }
            }

            //using (UnityWebRequest request = UnityWebRequest.Get(uri))
            //{
            //    Downloading = true;
            //    unityWebRequest = request;
            //    var timeout = Convert.ToInt32(downloadConfig.DownloadTimeout);
            //    if (timeout > 0)
            //        request.timeout = timeout;
            //    var startEventArgs = DownloadStartEventArgs.Create(uri, fileDownloadPath);
            //    downloadStart?.Invoke(startEventArgs);
            //    DownloadStartEventArgs.Release(startEventArgs);
            //    var operation = request.SendWebRequest();
            //    while (!operation.isDone && canDownload)
            //    {
            //        OnFileDownloading(uri, downloadConfig.DownloadPath, request.downloadProgress);
            //        var downloadedData = new DownloadedData(uri, request.downloadHandler.data, fileDownloadPath);
            //        yield return OnDownloadedData(downloadedData);
            //    }
            //    if (!request.isNetworkError && !request.isHttpError && canDownload)
            //    {
            //        if (request.isDone)
            //        {
            //            Downloading = false;
            //            var downloadData = request.downloadHandler.data;
            //            var downloadedData = new DownloadedData(uri, request.downloadHandler.data, fileDownloadPath);
            //            yield return OnDownloadedData(downloadedData);
            //            var successEventArgs = DownloadSuccessEventArgs.Create(uri, fileDownloadPath, downloadData);
            //            downloadSuccess?.Invoke(successEventArgs);
            //            OnFileDownloading(uri, downloadConfig.DownloadPath, 1);
            //            DownloadSuccessEventArgs.Release(successEventArgs);
            //            successURIs.Add(uri);
            //        }
            //    }
            //    else
            //    {
            //        Downloading = false;
            //        var failureEventArgs = DownloadFailureEventArgs.Create(uri, fileDownloadPath, request.error);
            //        downloadFailure?.Invoke(failureEventArgs);
            //        DownloadFailureEventArgs.Release(failureEventArgs);
            //        failureURIs.Add(uri);
            //        OnFileDownloading(uri, downloadConfig.DownloadPath, 1);
            //        if (downloadConfig.DeleteFailureFile)
            //        {
            //            Utility.IO.DeleteFile(fileDownloadPath);
            //        }
            //    }
            //}
        }
        /// <summary>
        /// 当下载到一个文件；
        /// </summary>
        /// <param name="downloadedData">下载到的文件信息</param>
        /// <returns>异步Task任务</returns>
        void OnDownloadedData(DownloadedData downloadedData)
        {
            var cachedLenth = downloadedData.Data.Length;
            if (dataWriteDict.TryGetValue(downloadedData.URI, out var writeInfo))
            {
                //旧缓存的长度小于新缓存的长度，则更新长度；
                if (writeInfo.CachedLength >= cachedLenth)
                    return ;
                //缓存新数据的长度，保留原来写入的长度；
                dataWriteDict.AddOrUpdate(downloadedData.URI, new DownloadWriteInfo(cachedLenth, writeInfo.WrittenLength));

                Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
                dataWriteDict.AddOrUpdate(downloadedData.URI, new DownloadWriteInfo(cachedLenth, cachedLenth));
            }
            else
            {
                //缓存新数据长度，原始写入长度设置为0，表示为写入过；
                dataWriteDict.Add(downloadedData.URI, new DownloadWriteInfo(cachedLenth, 0));
                Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
                dataWriteDict.AddOrUpdate(downloadedData.URI, new DownloadWriteInfo(cachedLenth, cachedLenth));
            }
        }
        /// <summary>
        /// 处理整体进度；
        /// individualPercent 为0~1；
        /// </summary>
        /// <param name="uri">资源地址</param>
        /// <param name="downloadPath">下载到本地的目录</param>
        /// <param name="individualPercent">资源个体百分比0~1</param>
        void OnFileDownloading(string uri, string downloadPath, float individualPercent)
        {
            var overallIndexPercent = 100 * ((float)currentDownloadIndex / DownloadableCount);
            var overallProgress = overallIndexPercent + (unitResRatio * (individualPercent));
            var eventArgs = DonwloadOverallEventArgs.Create(uri, downloadPath, overallProgress, individualPercent);
            downloadOverall.Invoke(eventArgs);
            DonwloadOverallEventArgs.Release(eventArgs);
        }
        /// <summary>
        /// 当Pending uri的文件全部下载完成；
        /// </summary>
        void OnDownloadedPendingFiles()
        {
            canDownload = false;
            Downloading = false;
            downloadEndTime = DateTime.Now;
            var eventArgs = DownloadAndWriteFinishEventArgs.Create(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
            downloadAndWriteFinish?.Invoke(eventArgs);
            DownloadAndWriteFinishEventArgs.Release(eventArgs);
            //清理下载配置缓存；
            failureURIs.Clear();
            successURIs.Clear();
            pendingURIs.Clear();

            dataWriteDict.Clear();
        }
    }
}
