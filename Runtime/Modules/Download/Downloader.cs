﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
namespace Cosmos.Download
{
    /// <summary>
    /// 文件下载器；
    /// </summary>
    public class Downloader : IDownloader
    {
        #region events
        Action<DownloadStartEventArgs> onDownloadStart;
        Action<DownloadSuccessEventArgs> onDownloadSuccess;
        Action<DownloadFailureEventArgs> onDownloadFailure;
        Action<DonwloadOverallProgressEventArgs> onDownloadOverall;
        Action<AllDownloadTasksCompletedEventArgs> onAllDownloadTaskCompleted;
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
        public event Action<DonwloadOverallProgressEventArgs> OnDownloadOverallProgress
        {
            add { onDownloadOverall += value; }
            remove { onDownloadOverall -= value; }
        }
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        public event Action<AllDownloadTasksCompletedEventArgs> OnAllDownloadTaskCompleted
        {
            add { onAllDownloadTaskCompleted += value; }
            remove { onAllDownloadTaskCompleted -= value; }
        }
        #endregion
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get; private set; }
        /// <summary>
        /// 下载中的资源总数；
        /// </summary>
        public int DownloadingCount { get { return pendingTasks.Count; } }
        /// <summary>
        /// 下载任务数量；
        /// </summary>
        int downloadTaskCount = 0;
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
        List<DownloadCompletedInfo> successedInfos = new List<DownloadCompletedInfo>();
        /// <summary>
        /// 下载失败的任务;
        /// </summary>
        List<DownloadCompletedInfo> failedInfos = new List<DownloadCompletedInfo>();
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
        UnityWebRequest unityWebRequest;
        /// <summary>
        /// 单位资源的百分比比率；
        /// </summary>
        float UnitResRatio { get { return 100f / downloadTaskCount; } }
        /// <summary>
        /// 当前下载的序号；
        /// </summary>
        int currentDownloadTaskIndex = 0;
        /// <summary>
        /// 当前是否可下载；
        /// </summary>
        bool canDownload;

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
            Utility.Unity.StartCoroutine(EnumDownloadMultipleFiles());
        }
        /// <summary>
        /// 移除所有下载；
        /// </summary>
        public void RemoveAllDownload()
        {
            OnCancelDownload();
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
            onAllDownloadTaskCompleted = null;
            downloadTaskCount = 0;
        }
        /// <summary>
        /// 多文件下载迭代器方法；
        /// </summary>
        /// <returns>迭代器接口</returns>
        IEnumerator EnumDownloadMultipleFiles()
        {
            while (pendingTasks.Count > 0)
            {
                var downloadTask = pendingTasks.RemoveFirst();
                currentDownloadTaskIndex = downloadTaskCount - pendingTasks.Count - 1;
                yield return EnumDownloadSingleFile(downloadTask);
                pendingTaskDict.Remove(downloadTask.URI);
            }
            OnDownloadedPendingFiles();
        }
        /// <summary>
        /// 单文件下载迭代器方法；
        /// </summary>
        /// <param name="downloadTask">下载任务对象</param>
        /// <returns>迭代器接口</returns>
        IEnumerator EnumDownloadSingleFile(DownloadTask downloadTask)
        {
            var fileDownloadStartTime = DateTime.Now;
            var uri = downloadTask.URI;
            var fileDownloadPath = downloadTask.DownloadPath;
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                Downloading = true;
                var append = DownloadDataProxy.DownloadAppend;
                var deleteFailureFile = DownloadDataProxy.DeleteFileOnAbort;
#if UNITY_2019_1_OR_NEWER
                var handler = new DownloadHandlerFile(downloadTask.DownloadPath, append) { removeFileOnAbort = deleteFailureFile };
#elif UNITY_2018_1_OR_NEWER
                var handler= new DownloadHandlerFile(downloadTask.DownloadPath) {  removeFileOnAbort=deleteFailureFile};
#endif
                request.downloadHandler = handler;
                unityWebRequest = request;
                var timeout = DownloadDataProxy.DownloadTimeout;
                if (timeout > 0)
                    request.timeout = timeout;
                var startEventArgs = DownloadStartEventArgs.Create(uri, fileDownloadPath);
                onDownloadStart?.Invoke(startEventArgs);
                DownloadStartEventArgs.Release(startEventArgs);
                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    OnFileDownloading(uri, fileDownloadPath, request.downloadProgress, request.downloadedBytes);
                    yield return null;
                }
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError&& canDownload)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError && canDownload)
#endif
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var fileDownloadEndTime = DateTime.Now;
                        var timeSpan = fileDownloadEndTime - fileDownloadStartTime;
                        var completedInfo = new DownloadCompletedInfo(uri, fileDownloadPath, request.downloadedBytes, timeSpan);
                        var successEventArgs = DownloadSuccessEventArgs.Create(completedInfo);
                        onDownloadSuccess?.Invoke(successEventArgs);
                        OnFileDownloading(uri, fileDownloadPath, 1, request.downloadedBytes);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        successedInfos.Add(completedInfo);
                    }
                }
                else
                {
                    Downloading = false;
                    var fileDownloadEndTime = DateTime.Now;
                    var timeSpan = fileDownloadEndTime - fileDownloadStartTime;
                    var completedInfo = new DownloadCompletedInfo(uri, fileDownloadPath, request.downloadedBytes, timeSpan);
                    var failureEventArgs = DownloadFailureEventArgs.Create(completedInfo, request.error);
                    onDownloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failedInfos.Add(completedInfo);
                    OnFileDownloading(uri, fileDownloadPath, 1, request.downloadedBytes);
                    unityWebRequest = null;
                }
            }
        }
        /// <summary>
        /// 处理整体进度；
        /// individualPercent 为0~1；
        /// </summary>
        /// <param name="uri">资源地址</param>
        /// <param name="downloadPath">下载到本地的目录</param>
        /// <param name="individualPercent">资源个体百分比0~1</param>
        /// <param name="downloadedBytes">下载的长度</param>
        void OnFileDownloading(string uri, string downloadPath, float individualPercent, ulong downloadedBytes)
        {
            var overallIndexPercent = 100 * ((float)currentDownloadTaskIndex / downloadTaskCount);
            var overallProgress = overallIndexPercent + (UnitResRatio * individualPercent);
            var eventArgs = DonwloadOverallProgressEventArgs.Create(uri, downloadPath, overallProgress, individualPercent, downloadedBytes);
            onDownloadOverall?.Invoke(eventArgs);
            DonwloadOverallProgressEventArgs.Release(eventArgs);
        }
        /// <summary>
        /// 当Pending uri的文件全部下载完成；
        /// </summary>
        void OnDownloadedPendingFiles()
        {
            canDownload = false;
            Downloading = false;
            downloadEndTime = DateTime.Now;
            var eventArgs = AllDownloadTasksCompletedEventArgs.Create(successedInfos.ToArray(), failedInfos.ToArray(), downloadEndTime - downloadStartTime);
            onAllDownloadTaskCompleted?.Invoke(eventArgs);
            AllDownloadTasksCompletedEventArgs.Release(eventArgs);
            //清理下载配置缓存；
            failedInfos.Clear();
            successedInfos.Clear();
            pendingTasks.Clear();
            downloadTaskCount = 0;
            pendingTaskDict.Clear();
        }
        void OnCancelDownload()
        {
            unityWebRequest?.Abort();
            //foreach (var task in pendingTasks)
            //{
            //    var completedInfo = new DownloadCompletedInfo(task.URI, task.DownloadPath, 0, TimeSpan.Zero);
            //    failedInfos.Add(completedInfo);
            //}
            //todo 这里需要将pending列表中的任务变更为下载失败
            pendingTasks.Clear();
            pendingTaskDict.Clear();
            downloadTaskCount = 0;
            failedInfos.Clear();
            successedInfos.Clear();
            canDownload = false;
        }
    }
}
