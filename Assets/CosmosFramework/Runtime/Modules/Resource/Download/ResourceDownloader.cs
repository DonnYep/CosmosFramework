using Cosmos.Download;
using System;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源下载
    /// </summary>
    public class ResourceDownloader
    {
        //UNDONE 资源下载器 
        readonly IDownloadManager downloadManager;
        /// <summary>
        /// taskId===downloadTask
        /// </summary>
        readonly Dictionary<long, ResourceDownloadTask> taskDict;
        /// <summary>
        /// taskId===downloadNode
        /// </summary>
        readonly Dictionary<long, ResourceDownloadNode> nodeDict;
        /// <summary>
        /// 已经下载的长度
        /// </summary>
        long currentDownloadedSize;
        /// <summary>
        /// 总共需要下载的长度
        /// </summary>
        long totalRequiredDownloadSize;
        /// <summary>
        /// 下载成功的任务列表
        /// </summary>
        readonly List<ResourceDownloadTask> downloadSuccessTaskList;
        /// <summary>
        /// 下载失败的任务列表
        /// </summary>
        readonly List<ResourceDownloadTask> downloadFailureTaskList;

        Action<ResourceDownloadSuccessEventArgs> onDownloadSuccess;
        Action<ResourceDownloadFailureEventArgs> onDownloadFailure;
        Action<ResourceDownloadUpdateEventArgs> onDownloadUpdate;
        Action<ResourceDownloadCompeleteEventArgs> onDownloadComplete;
        /// <summary>
        /// 任务下载成功事件
        /// </summary>
        public event Action<ResourceDownloadSuccessEventArgs> OnDownloadSuccess
        {
            add { onDownloadSuccess += value; }
            remove { onDownloadSuccess -= value; }
        }
        /// <summary>
        /// 任务下载失败事件
        /// </summary>
        public event Action<ResourceDownloadFailureEventArgs> OnDownloadFailure
        {
            add { onDownloadFailure += value; }
            remove { onDownloadFailure -= value; }
        }
        /// <summary>
        /// 整体任务下载更新事件
        /// </summary>
        public event Action<ResourceDownloadUpdateEventArgs> OnDownloadUpdate
        {
            add { onDownloadUpdate += value; }
            remove { onDownloadUpdate -= value; }
        }
        /// <summary>
        /// 所有任务完成事件
        /// </summary>
        public event Action<ResourceDownloadCompeleteEventArgs> OnDownloadComplete
        {
            add { onDownloadComplete += value; }
            remove { onDownloadComplete -= value; }
        }
        public ResourceDownloader(IDownloadManager downloadManager)
        {
            this.downloadManager = downloadManager;
            taskDict = new Dictionary<long, ResourceDownloadTask>();
            nodeDict = new Dictionary<long, ResourceDownloadNode>();
            downloadSuccessTaskList = new List<ResourceDownloadTask>();
            downloadFailureTaskList = new List<ResourceDownloadTask>();
        }
        public void OnInitialize()
        {
            downloadManager.OnDownloadSuccess += OnDownloadSuccessHandler;
            downloadManager.OnDownloadFailure += OnDownloadFailureHandler;
            downloadManager.OnDownloadOverallProgress += OnDownloadUpdateHandler;
        }
        public void OnTerminate()
        {
            downloadManager.OnDownloadSuccess -= OnDownloadSuccessHandler;
            downloadManager.OnDownloadFailure -= OnDownloadFailureHandler;
            downloadManager.OnDownloadOverallProgress -= OnDownloadUpdateHandler;
        }
        /// <summary>
        /// 添加下载任务
        /// </summary>
        /// <param name="task">下载任务</param>
        /// <returns>下载任务Id</returns>
        public long AddDownloadTask(ResourceDownloadTask task)
        {
            var url = task.ResourceDownloadURL;
            var downloadPath = task.ResourceDownloadPath;
            var recordedSize = task.RecordedResourceSize;
            long localSize = 0;
            long recordedResourceSize = 0;
#if UNITY_2019_1_OR_NEWER
            localSize = Utility.IO.GetFileSize(task.ResourceDownloadPath);
            recordedResourceSize = recordedSize - localSize;
#elif UNITY_2018_1_OR_NEWER
            recordedResourceSize = downloadNode.RecordedResourceSize 
#endif
            totalRequiredDownloadSize += recordedResourceSize;
            var taskId = downloadManager.AddDownload(url, downloadPath, localSize);
            var downloadNode = new ResourceDownloadNode(taskId, url, downloadPath, recordedSize, localSize);
            taskDict.Add(taskId, task);
            return taskId;
        }
        /// <summary>
        /// 移除下载任务
        /// </summary>
        /// <param name="taskId">下载任务</param>
        /// <returns>移除结果，失败或成功</returns>
        public void RemoveTask(long taskId)
        {
            nodeDict.Remove(taskId, out var downloadNode);
            taskDict.Remove(taskId);
            downloadManager.RemoveDownload(taskId);
            long recordedResourceSize = 0;
#if UNITY_2019_1_OR_NEWER
            recordedResourceSize = downloadNode.RecordedResourceSize - downloadNode.LocalResourceSize;
#elif UNITY_2018_1_OR_NEWER
            recordedResourceSize = downloadNode.RecordedResourceSize 
#endif
            totalRequiredDownloadSize -= recordedResourceSize;
        }
        public void StartDowload()
        {
            if (!downloadManager.Downloading)
                downloadManager.LaunchDownload();
        }
        public void StopDowload()
        {
            // TODO ResourceDownloader StopDowload
        }
        void OnDownloadSuccessHandler(DownloadSuccessEventArgs eventArgs)
        {
            var taskId = eventArgs.DownloadInfo.DownloadId;
            if (taskDict.Remove(taskId, out var task))
            {
                nodeDict.Remove(taskId, out var downloadNode);
                downloadSuccessTaskList.Add(task);
                //todo下载的成功逻辑

                var successEventArgs = ResourceDownloadSuccessEventArgs.Create(task);
                onDownloadSuccess?.Invoke(successEventArgs);
                ResourceDownloadSuccessEventArgs.Release(successEventArgs);
                currentDownloadedSize += (long)eventArgs.DownloadInfo.DownloadedLength;
                if (taskDict.Count == 0)
                {
                    OnDownloadCompleteHandler();
                }
            }
        }
        void OnDownloadFailureHandler(DownloadFailureEventArgs eventArgs)
        {
            var taskId = eventArgs.DownloadInfo.DownloadId;
            if (taskDict.Remove(taskId, out var task))
            {

                nodeDict.Remove(taskId, out var downloadNode);
                downloadFailureTaskList.Add(task);
                var failureEventArgs = ResourceDownloadFailureEventArgs.Create(task);
                onDownloadFailure?.Invoke(failureEventArgs);
                ResourceDownloadFailureEventArgs.Release(failureEventArgs);
            }
        }
        void OnDownloadUpdateHandler(DonwloadUpdateEventArgs eventArgs)
        {
            var taskId = eventArgs.DownloadInfo.DownloadId;
            if (taskDict.ContainsKey(taskId))
            {
                var downloadedSize = currentDownloadedSize + (long)eventArgs.DownloadInfo.DownloadedLength;
                var updateEventArgs = ResourceDownloadUpdateEventArgs.Create(downloadedSize, totalRequiredDownloadSize);
                onDownloadUpdate?.Invoke(updateEventArgs);
                ResourceDownloadUpdateEventArgs.Release(updateEventArgs);
            }
        }
        void OnDownloadCompleteHandler()
        {
            var successTasks = downloadSuccessTaskList.ToArray();
            var failureTasks = downloadFailureTaskList.ToArray();
            var eventArgs = ResourceDownloadCompeleteEventArgs.Create(successTasks, failureTasks);
            onDownloadComplete?.Invoke(eventArgs);
            ResourceDownloadCompeleteEventArgs.Release(eventArgs);
            downloadSuccessTaskList.Clear();
            downloadFailureTaskList.Clear();
            currentDownloadedSize = 0;
            totalRequiredDownloadSize = 0;
        }
    }
}
