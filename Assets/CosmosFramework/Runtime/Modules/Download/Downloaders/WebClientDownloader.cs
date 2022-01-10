using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public class WebClientDownloader : IDownloader
    {
        #region events
        Action<DownloadStartEventArgs> onDownloadStart;
        Action<DownloadSuccessEventArgs> onDownloadSuccess;
        Action<DownloadFailureEventArgs> onDownloadFailure;
        Action<DonwloadOverallEventArgs> onDownloadOverall;
        Action<DownloadAndWriteFinishEventArgs> onDownloadAndWriteFinish;
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        public event Action<DownloadStartEventArgs> OnDownloadStart
        {
            add { onDownloadStart += value; }
            remove { onDownloadStart -= value; }
        }
        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        public event Action<DownloadSuccessEventArgs> OnDownloadSuccess
        {
            add { onDownloadSuccess += value; }
            remove { onDownloadSuccess -= value; }
        }
        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        public event Action<DownloadFailureEventArgs> OnDownloadFailure
        {
            add { onDownloadFailure += value; }
            remove { onDownloadFailure -= value; }
        }
        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        public event Action<DonwloadOverallEventArgs> OnDownloadOverall
        {
            add { onDownloadOverall += value; }
            remove { onDownloadOverall -= value; }
        }
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        public event Action<DownloadAndWriteFinishEventArgs> OnDownloadAndWriteFinish
        {
            add { onDownloadAndWriteFinish += value; }
            remove { onDownloadAndWriteFinish -= value; }
        }
        #endregion
        /// <summary>
        /// 是否删除本地下载失败的文件；
        /// </summary>
        public bool DeleteFailureFile { get; set; }
        public float DownloadTimeout
        {
            get { return downloadTimeout; }
            set
            {
                if (value <= 0) downloadTimeout = 0;
                else
                    downloadTimeout = value;
            }
        }
        float downloadTimeout;
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get; private set; }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        public int DownloadingCount { get { return pendingTasks.Count; } }
        /// <summary>
        /// 挂起的下载任务；
        /// </summary>
        List<DownloadTask> pendingTasks = new List<DownloadTask>();
        /// <summary>
        /// 挂起的下载任务字典缓存；
        /// </summary>
        Dictionary<string, DownloadTask> pendingTaskDict = new Dictionary<string, DownloadTask>();
        /// <summary>
        /// 下载成功的任务;
        /// </summary>
        List<DownloadTask> successTasks = new List<DownloadTask>();
        /// <summary>
        /// 下载失败的任务;
        /// </summary>
        List<DownloadTask> failureTasks = new List<DownloadTask>();
        /// <summary>
        /// 下载开始时间；
        /// </summary>
        DateTime downloadStartTime;
        /// <summary>
        /// 下载结束时间；
        /// </summary>
        DateTime downloadEndTime;
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
        int currentDownloadTaskIndex = 0;
        /// <summary>
        /// 当前是否可下载；
        /// </summary>
        bool canDownload;
        /// <summary>
        /// URI===[[缓存的长度===写入本地的长度]]；
        /// 数据写入记录；
        /// </summary>
        Dictionary<string, DownloadWrittenInfo> dataWrittenDict = new Dictionary<string, DownloadWrittenInfo>();
        /// <summary>
        /// 下载任务数量；
        /// </summary>
        int downloadTaskCount = 0;
        /// <summary>
        /// 添加URI下载；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        /// <param name="downloadPath">下载到地址的绝对路径</param>
        public void AddUriDownload(string uri, string downloadPath)
        {
            if (!pendingTaskDict.ContainsKey(uri))
            {
                var dt = new DownloadTask(uri, downloadPath);
                pendingTaskDict.Add(uri, dt);
                pendingTasks.Add(dt);
                downloadTaskCount++;
            }
        }
        /// <summary>
        /// 移除URI下载；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        public void RemoveUriDownload(string uri)
        {
            if (pendingTaskDict.Remove(uri, out var downloadTask))
            {
                pendingTasks.Remove(downloadTask);
                downloadTaskCount--;
            }
        }
        /// <summary>
        /// 移除所有下载；
        /// </summary>
        public void RemoveAllDownload()
        {
            OnCancelDownload();
        }
        /// <summary>
        /// 启动下载；
        /// </summary>
        public void LaunchDownload()
        {
            canDownload = true;
            if (pendingTasks.Count == 0 || !canDownload)
            {
                canDownload = false;
                return;
            }
            Downloading = true;
            downloadStartTime = DateTime.Now;
            DownloadMultipleFiles();
        }
        /// <summary>
        /// 取消下载；
        /// </summary>
        public void CancelDownload()
        {
            OnCancelDownload();
        }
        /// <summary>
        /// 释放下载器；
        /// </summary>
        public void Release()
        {
            onDownloadStart = null;
            onDownloadSuccess = null;
            onDownloadFailure = null;
            onDownloadOverall = null;
            onDownloadAndWriteFinish = null;
            downloadTaskCount = 0;
        }
        /// <summary>
        /// 多文件下载迭代器方法；
        /// </summary>
        /// <returns>迭代器接口</returns>
        async void DownloadMultipleFiles()
        {
            while (pendingTasks.Count > 0)
            {
                var downloadTask = pendingTasks.RemoveFirst();
                currentDownloadTaskIndex = downloadTaskCount - pendingTasks.Count - 1;
                await DownloadSingleFile(downloadTask);
            }
            OnDownloadedPendingFiles();
        }
        /// <summary>
        /// 单文件下载迭代器方法；
        /// </summary>
        /// <param name="uri">文件地址</param>
        /// <param name="fileDownloadPath">本地写入的地址</param>
        /// <returns>迭代器接口</returns>
        async Task DownloadSingleFile(DownloadTask downloadTask)
        {
            var uri = downloadTask.URI;
            var fileDownloadPath = downloadTask.DownloadPath;
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    Downloading = true;
                    this.webClient = webClient;
                    var startEventArgs = DownloadStartEventArgs.Create(uri, fileDownloadPath);
                    onDownloadStart?.Invoke(startEventArgs);
                    DownloadStartEventArgs.Release(startEventArgs);
                    webClient.DownloadProgressChanged += (sender, eventArgs) =>
                    {
                        OnFileDownloading(uri, fileDownloadPath, (float)eventArgs.ProgressPercentage / 100);
                    };
                    webClient.DownloadDataCompleted += (sender, eventArgs) =>
                    {
                        Downloading = false;
                        var resultData = eventArgs.Result;
                        var successEventArgs = DownloadSuccessEventArgs.Create(uri, fileDownloadPath, resultData);
                        onDownloadSuccess?.Invoke(successEventArgs);
                        OnFileDownloading(uri, fileDownloadPath, 1);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        successTasks.Add(downloadTask);
                        var downloadedData = new DownloadedData(uri, resultData, fileDownloadPath);
                        OnDownloadedData(downloadedData);
                    };
                    await webClient.DownloadDataTaskAsync(uri);
                }
                catch (Exception exception)
                {
                    Downloading = false;
                    var failureEventArgs = DownloadFailureEventArgs.Create(uri, fileDownloadPath, exception.Message);
                    onDownloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failureTasks.Add(downloadTask);
                    OnFileDownloading(uri, fileDownloadPath, 1);
                    if (DeleteFailureFile)
                    {
                        Utility.IO.DeleteFile(fileDownloadPath);
                    }
                }
            }
        }
        /// <summary>
        /// 当下载到一个文件；
        /// </summary>
        /// <param name="downloadedData">下载到的文件信息</param>
        /// <returns>异步Task任务</returns>
        void OnDownloadedData(DownloadedData downloadedData)
        {
            var cachedLenth = downloadedData.Data.Length;
            if (dataWrittenDict.TryGetValue(downloadedData.URI, out var writeInfo))
            {
                //旧缓存的长度小于新缓存的长度，则更新长度；
                if (writeInfo.CachedLength >= cachedLenth)
                    return;
                //缓存新数据的长度，保留原来写入的长度；
                dataWrittenDict.AddOrUpdate(downloadedData.URI, new DownloadWrittenInfo(cachedLenth, writeInfo.WrittenLength));

                Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
                dataWrittenDict.AddOrUpdate(downloadedData.URI, new DownloadWrittenInfo(cachedLenth, cachedLenth));
            }
            else
            {
                //缓存新数据长度，原始写入长度设置为0，表示为写入过；
                dataWrittenDict.Add(downloadedData.URI, new DownloadWrittenInfo(cachedLenth, 0));
                Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
                dataWrittenDict.AddOrUpdate(downloadedData.URI, new DownloadWrittenInfo(cachedLenth, cachedLenth));
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
            var overallIndexPercent = 100 * ((float)currentDownloadTaskIndex / DownloadingCount);
            var overallProgress = overallIndexPercent + (unitResRatio * (individualPercent));
            var eventArgs = DonwloadOverallEventArgs.Create(uri, downloadPath, overallProgress, individualPercent);
            onDownloadOverall.Invoke(eventArgs);
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
            var successUris = successTasks.Select(t => t.URI).ToArray();
            var failureUris = failureTasks.Select(t => t.URI).ToArray();
            var eventArgs = DownloadAndWriteFinishEventArgs.Create(successUris, failureUris, downloadEndTime - downloadStartTime);
            onDownloadAndWriteFinish?.Invoke(eventArgs);
            DownloadAndWriteFinishEventArgs.Release(eventArgs);
            //清理下载配置缓存；
            failureTasks.Clear();
            successTasks.Clear();
            pendingTasks.Clear();
            dataWrittenDict.Clear();
            downloadTaskCount = 0;
            pendingTaskDict.Clear();
        }
        void OnCancelDownload()
        {
            webClient.CancelAsync();
            pendingTasks.Clear();
            pendingTaskDict.Clear();
            downloadTaskCount = 0;
            pendingTasks.Clear();
            failureTasks.Clear();
            successTasks.Clear();
            canDownload = false;
        }
    }
}
