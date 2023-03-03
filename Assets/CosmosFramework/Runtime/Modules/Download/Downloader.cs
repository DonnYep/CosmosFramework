using System;
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
        Action<DonwloadUpdateEventArgs> onDownloadOverall;
        Action<DownloadTasksCompletedEventArgs> onAllDownloadTaskCompleted;
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
        public event Action<DonwloadUpdateEventArgs> OnDownloadOverallProgress
        {
            add { onDownloadOverall += value; }
            remove { onDownloadOverall -= value; }
        }
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        public event Action<DownloadTasksCompletedEventArgs> OnAllDownloadTaskCompleted
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
        List<DownloadInfo> successedInfos = new List<DownloadInfo>();
        /// <summary>
        /// 下载失败的任务;
        /// </summary>
        List<DownloadInfo> failedInfos = new List<DownloadInfo>();
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
        /// <param name="downloadUri">统一资源名称</param>
        /// <param name="downloadPath">下载到地址的绝对路径</param>
        public void AddDownload(string downloadUri, string downloadPath)
        {
            if (!pendingTaskDict.ContainsKey(downloadUri))
            {
                var downloadTask = new DownloadTask(downloadUri, downloadPath);
                pendingTaskDict.Add(downloadUri, downloadTask);
                pendingTasks.Add(downloadTask);
                downloadTaskCount++;
            }
        }
        /// <summary>
        /// 移除URI下载；
        /// </summary>
        /// <param name="downloadUri">统一资源名称</param>
        public void RemoveDownload(string downloadUri)
        {
            if (pendingTaskDict.Remove(downloadUri, out var downloadTask))
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
            if (Downloading)
                return;
            canDownload = true;
            if (pendingTasks.Count == 0 || !canDownload)
            {
                canDownload = false;
                return;
            }
            downloadStartTime = DateTime.Now;
            Utility.Unity.StartCoroutine(RunDownloadMultipleFiles());
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
        IEnumerator RunDownloadMultipleFiles()
        {
            Downloading = true;
            while (pendingTasks.Count > 0)
            {
                var downloadTask = pendingTasks.RemoveFirst();
                currentDownloadTaskIndex = downloadTaskCount - pendingTasks.Count - 1;
                yield return RunDownloadSingleFile(downloadTask);
                pendingTaskDict.Remove(downloadTask.DownloadUri);
            }
            OnPendingTasksCompleted();
            Downloading = false;
        }
        IEnumerator RunGetContentLength(DownloadTask downloadTask)
        {
            var startTime = DateTime.Now;
            var downloadUri = downloadTask.DownloadUri;
            var downloadPath = downloadTask.DownloadPath;
            using (UnityWebRequest request = UnityWebRequest.Head(downloadUri))
            {
                //这部分通过header获取需要下载的文件大小
                unityWebRequest = request;
                request.timeout = DownloadDataProxy.DownloadTimeout;
                yield return request.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError&& canDownload)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    var fileLength = long.Parse(request.GetRequestHeader("Content-Length"));
                    var timeSpan = DateTime.Now - startTime;
                    var info = new DownloadInfo(downloadUri, downloadPath, 0, fileLength, timeSpan);

                }
                else
                {
                    var timeSpan = DateTime.Now - startTime;
                    var info = new DownloadInfo(downloadUri, downloadPath, 0, 0, timeSpan);

                }
            }
        }
        /// <summary>
        /// 单文件下载迭代器方法；
        /// </summary>
        /// <param name="downloadTask">下载任务对象</param>
        /// <returns>迭代器接口</returns>
        IEnumerator RunDownloadSingleFile(DownloadTask downloadTask)
        {
            var fileDownloadStartTime = DateTime.Now;
            var downloadUri = downloadTask.DownloadUri;
            var downloadPath = downloadTask.DownloadPath;
            using (UnityWebRequest request = UnityWebRequest.Get(downloadUri))
            {
                var append = DownloadDataProxy.DownloadAppend;
                var deleteFailureFile = DownloadDataProxy.DeleteFileOnAbort;
#if UNITY_2019_1_OR_NEWER
                var handler = new DownloadHandlerFile(downloadTask.DownloadPath, append) { removeFileOnAbort = deleteFailureFile };
#elif UNITY_2018_1_OR_NEWER
                var handler= new DownloadHandlerFile(downloadTask.DownloadPath) {  removeFileOnAbort=deleteFailureFile};
#endif
                request.downloadHandler = handler;
                unityWebRequest = request;
                request.timeout = DownloadDataProxy.DownloadTimeout;

                {
                    var timeSpan = DateTime.Now - fileDownloadStartTime;
                    var downloadInfo = new DownloadInfo(downloadUri, downloadPath, 0, 0, timeSpan);
                    var startEventArgs = DownloadStartEventArgs.Create(downloadInfo, currentDownloadTaskIndex, downloadTaskCount);
                    onDownloadStart?.Invoke(startEventArgs);
                    DownloadStartEventArgs.Release(startEventArgs);
                }

                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    var timeSpan = DateTime.Now - fileDownloadStartTime;
                    var downloadInfo = new DownloadInfo(downloadUri, downloadPath, request.downloadedBytes, operation.progress, timeSpan);
                    OnFileDownloading(downloadInfo);
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
                        var timeSpan = DateTime.Now - fileDownloadStartTime;
                        var downloadInfo = new DownloadInfo(downloadUri, downloadPath, request.downloadedBytes, 1, timeSpan);
                        var successEventArgs = DownloadSuccessEventArgs.Create(downloadInfo, currentDownloadTaskIndex, downloadTaskCount);
                        onDownloadSuccess?.Invoke(successEventArgs);
                        OnFileDownloading(downloadInfo);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        successedInfos.Add(downloadInfo);
                    }
                }
                else
                {
                    var timeSpan = DateTime.Now - fileDownloadStartTime;
                    var downloadInfo = new DownloadInfo(downloadUri, downloadPath, request.downloadedBytes, 0, timeSpan);
                    var failureEventArgs = DownloadFailureEventArgs.Create(downloadInfo, currentDownloadTaskIndex, downloadTaskCount, request.error);
                    onDownloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failedInfos.Add(downloadInfo);
                    OnFileDownloading(downloadInfo);
                    unityWebRequest = null;
                }
            }
        }
        /// <summary>
        /// 处理整体进度；
        /// </summary>
        void OnFileDownloading(DownloadInfo info)
        {
            var eventArgs = DonwloadUpdateEventArgs.Create(info, currentDownloadTaskIndex, downloadTaskCount);
            onDownloadOverall?.Invoke(eventArgs);
            DonwloadUpdateEventArgs.Release(eventArgs);
        }
        /// <summary>
        /// 当Pending uri的文件全部下载完成；
        /// </summary>
        void OnPendingTasksCompleted()
        {
            canDownload = false;
            downloadEndTime = DateTime.Now;
            var eventArgs = DownloadTasksCompletedEventArgs.Create(successedInfos.ToArray(), failedInfos.ToArray(), downloadEndTime - downloadStartTime, downloadTaskCount);
            onAllDownloadTaskCompleted?.Invoke(eventArgs);
            DownloadTasksCompletedEventArgs.Release(eventArgs);
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
            Downloading = false;
        }
    }
}
