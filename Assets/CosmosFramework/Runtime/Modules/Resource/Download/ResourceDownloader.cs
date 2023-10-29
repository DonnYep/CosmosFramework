using Cosmos.WebRequest;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源下载，WIP
    /// </summary>
    public class ResourceDownloader
    {
        readonly IWebRequestManager webRequestManager;
        readonly Action<string, ResourceManifest> onSuccess;
        readonly Action<string, string> onFailure;
        /// <summary>
        /// taskId===downloadTask
        /// </summary>
        readonly Dictionary<long, ResourceDownloadTask> taskDict;
        /// <summary>
        /// downloadTask===taskId
        /// </summary>
        readonly Dictionary<ResourceDownloadTask, long> reserveTaskDict;
        /// <summary>
        /// 已经下载的长度
        /// </summary>
        long currentDownloadedSize;
        /// <summary>
        /// 总共需要下载的长度
        /// </summary>
        long totalRequiredDownloadSize;
        public ResourceDownloader(IWebRequestManager webRequestManager)
        {
            this.webRequestManager = webRequestManager;
            taskDict = new Dictionary<long, ResourceDownloadTask>();
            reserveTaskDict = new Dictionary<ResourceDownloadTask, long>();
        }
        public void OnInitialize()
        {
            webRequestManager.OnSuccessCallback += OnSuccessCallback;
            webRequestManager.OnFailureCallback += OnFailureCallback;
            webRequestManager.OnUpdateCallback += OnUpdateCallback;
        }
        public void OnTerminate()
        {
            webRequestManager.OnSuccessCallback -= OnSuccessCallback;
            webRequestManager.OnFailureCallback -= OnFailureCallback;
            webRequestManager.OnUpdateCallback -= OnUpdateCallback;
        }
        /// <summary>
        /// 添加下载任务
        /// </summary>
        /// <param name="task">下载任务</param>
        /// <returns>添加结果，失败或成功</returns>
        public bool AddDownloadTask(ResourceDownloadTask task)
        {
            var url = task.ResourceDownloadURL;
            var downloadPath = task.ResourceDownloadPath;
            if (!reserveTaskDict.ContainsKey(task))
            {
                var request = GetWebRequest(url, downloadPath);
                var taskId = webRequestManager.AddDownloadRequestTask(request);
                reserveTaskDict.Add(task, taskId);
                taskDict.Add(taskId, task);
                long recordedResourceSize = 0;
#if UNITY_2019_1_OR_NEWER
                recordedResourceSize = task.RecordedResourceSize - task.LocalResourceSize;
#elif UNITY_2018_1_OR_NEWER
                recordedResourceSize = task.RecordedResourceSize 
#endif
                totalRequiredDownloadSize += recordedResourceSize;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 移除下载任务
        /// </summary>
        /// <param name="task">下载任务</param>
        /// <returns>移除结果，失败或成功</returns>
        public bool RemoveTask(ResourceDownloadTask task)
        {
            if (reserveTaskDict.Remove(task, out var taskId))
            {
                taskDict.Remove(taskId);
                webRequestManager.RemoveTask(taskId);
                long recordedResourceSize = 0;
#if UNITY_2019_1_OR_NEWER
                recordedResourceSize = task.RecordedResourceSize - task.LocalResourceSize;
#elif UNITY_2018_1_OR_NEWER
                recordedResourceSize = task.RecordedResourceSize 
#endif
                totalRequiredDownloadSize -= recordedResourceSize;
                return true;
            }
            return false;
        }
        void OnSuccessCallback(WebRequestSuccessEventArgs eventArgs)
        {
            var taskId = eventArgs.TaskId;
            if (taskDict.Remove(taskId, out var task))
            {
                reserveTaskDict.Remove(task);
                //todo下载的成功逻辑
            }
        }
        void OnFailureCallback(WebRequestFailureEventArgs eventArgs)
        {
            taskDict.Remove(eventArgs.TaskId);
            onFailure?.Invoke(eventArgs.URL, eventArgs.ErrorMessage);
        }
        void OnUpdateCallback(WebRequestUpdateEventArgs eventArgs)
        {
            var taskId = eventArgs.TaskId;
            if (taskDict.ContainsKey(eventArgs.TaskId))
            {
                var downloadedSize = currentDownloadedSize + (long)eventArgs.WebRequest.downloadedBytes;

            }
        }
        UnityWebRequest GetWebRequest(string url, string downloadPath)
        {
            var request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerFile(downloadPath, true);
            return request;
        }
    }
}
