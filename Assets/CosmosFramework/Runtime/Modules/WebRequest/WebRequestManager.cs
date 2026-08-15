using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    //================================================
    /*
     * 1、WebRequest用于加载AssetBundle资源。资源状态可以是Remote的，
    *  也可以是Local下persistentDataPath的。
     * 
     * 2、内置已经实现了一个默认的WebRequest帮助类对象。模块初始化时会
    * 自动加载并将默认的helper设置为此模块的默认加载helper。
     * 
     * 3、请求以队列形式存在，支持最大并发数量限制与优先级调度。
     * 
     * 4、新API（AddDownloadTextTaskAsync等）返回句柄，支持
    * 事件回调/协程等待/async-await/同步等待/取消/超时/重试。
     */
    //================================================
    [Module]
    internal class WebRequestManager : Module, IWebRequestManager
    {
        WebRequester webRequester = new WebRequester();
        Dictionary<long, WebUrlFileRequestTask> urlFileReqiestTaskDict = new Dictionary<long, WebUrlFileRequestTask>();
        Action<WebRequestGetHtmlFilesFailureEventArgs> onGetHtmlFilesFailureCallback;
        Action<WebRequestGetHtmlFilesSuccessEventArgs> onGetHtmlFilesSuccessCallback;
        ///<inheritdoc/>
        public bool Running { get { return webRequester.Running; } }
        ///<inheritdoc/>
        public int TaskCount { get { return webRequester.TaskCount; } }
        /// <inheritdoc/>
        public int MaxConcurrentRequests
        {
            get { return webRequester.Scheduler.MaxConcurrent; }
            set { webRequester.Scheduler.MaxConcurrent = Mathf.Max(1, value); }
        }
        /// <inheritdoc/>
        public int ActiveRequestCount { get { return webRequester.Scheduler.ActiveCount; } }
        /// <inheritdoc/>
        public int WaitingRequestCount { get { return webRequester.Scheduler.WaitingCount; } }
        ///<inheritdoc/>
        public event Action<WebRequestStartEventArgs> OnStartCallback
        {
            add { webRequester.onStartCallback += value; }
            remove { webRequester.onStartCallback -= value; }
        }
        ///<inheritdoc/>
        public event Action<WebRequestUpdateEventArgs> OnUpdateCallback
        {
            add { webRequester.onUpdateCallback += value; }
            remove { webRequester.onUpdateCallback -= value; }
        }
        ///<inheritdoc/>
        public event Action<WebRequestSuccessEventArgs> OnSuccessCallback
        {
            add { webRequester.onSuccessCallback += value; }
            remove { webRequester.onSuccessCallback -= value; }
        }
        ///<inheritdoc/>
        public event Action<WebRequestFailureEventArgs> OnFailureCallback
        {
            add { webRequester.onFailureCallback += value; }
            remove { webRequester.onFailureCallback -= value; }
        }
        ///<inheritdoc/>
        public event Action<WebRequestGetContentLengthFailureEventArgs> OnGetContentLengthFailureCallback
        {
            add { webRequester.onGetContentLengthFailureCallback += value; }
            remove { webRequester.onGetContentLengthFailureCallback -= value; }
        }
        ///<inheritdoc/>
        public event Action<WebRequestGetContentLengthSuccessEventArgs> OnGetContentLengthSuccessCallback
        {
            add { webRequester.onGetContentLengthSuccessCallback += value; }
            remove { webRequester.onGetContentLengthSuccessCallback -= value; }
        }
        ///<inheritdoc/>
        public event Action<WebRequestAllTaskCompleteEventArgs> OnAllTaskCompleteCallback
        {
            add { webRequester.onAllTaskCompleteCallback += value; }
            remove { webRequester.onAllTaskCompleteCallback -= value; }
        }
        /// <inheritdoc/>
        public event Action<WebRequestGetHtmlFilesFailureEventArgs> OnGetHtmlFilesFailureCallback
        {
            add { onGetHtmlFilesFailureCallback += value; }
            remove { onGetHtmlFilesFailureCallback -= value; }
        }
        /// <inheritdoc/>
        public event Action<WebRequestGetHtmlFilesSuccessEventArgs> OnGetHtmlFilesSuccessCallback
        {
            add { onGetHtmlFilesSuccessCallback += value; }
            remove { onGetHtmlFilesSuccessCallback -= value; }
        }

        #region 新API：句柄式异步请求
        /// <inheritdoc/>
        public WebRequestHandle<string> AddDownloadTextTaskAsync(string url, float timeoutSeconds = 30, int retryCount = 0, uint priority = 0)
        {
            return CreateHandle(url, timeoutSeconds, retryCount, priority,
                u => UnityWebRequest.Get(u),
                request => request.downloadHandler != null ? request.downloadHandler.text : null);
        }
        /// <inheritdoc/>
        public WebRequestHandle<byte[]> AddDownloadBytesTaskAsync(string url, float timeoutSeconds = 30, int retryCount = 0, uint priority = 0)
        {
            return CreateHandle(url, timeoutSeconds, retryCount, priority,
                u => UnityWebRequest.Get(u),
                request => request.downloadHandler != null ? request.downloadHandler.data : null);
        }
        /// <inheritdoc/>
        public WebRequestHandle<Texture2D> AddDownloadTextureTaskAsync(string url, float timeoutSeconds = 30, int retryCount = 0, uint priority = 0)
        {
            return CreateHandle(url, timeoutSeconds, retryCount, priority,
                UnityWebRequestTexture.GetTexture,
                request => (request.downloadHandler as DownloadHandlerTexture) != null ? ((DownloadHandlerTexture)request.downloadHandler).texture : null);
        }
        /// <inheritdoc/>
        public WebRequestHandle<AudioClip> AddDownloadAudioTaskAsync(string url, AudioType audioType, float timeoutSeconds = 30, int retryCount = 0, uint priority = 0)
        {
            return CreateHandle(url, timeoutSeconds, retryCount, priority,
                u => UnityWebRequestMultimedia.GetAudioClip(u, audioType),
                request => (request.downloadHandler as DownloadHandlerAudioClip) != null ? ((DownloadHandlerAudioClip)request.downloadHandler).audioClip : null);
        }
        /// <inheritdoc/>
        public WebRequestHandle<AssetBundle> AddDownloadAssetBundleTaskAsync(string url, float timeoutSeconds = 30, int retryCount = 0, uint priority = 0)
        {
            return CreateHandle(url, timeoutSeconds, retryCount, priority,
                UnityWebRequestAssetBundle.GetAssetBundle,
                request => (request.downloadHandler as DownloadHandlerAssetBundle) != null ? ((DownloadHandlerAssetBundle)request.downloadHandler).assetBundle : null);
        }
        /// <inheritdoc/>
        public WebRequestHandle<long> AddGetContentLengthTaskAsync(string url, float timeoutSeconds = 30, int retryCount = 0, uint priority = 0)
        {
            return CreateHandle(url, timeoutSeconds, retryCount, priority,
                UnityWebRequest.Head,
                request =>
                {
                    long length = 0;
                    var size = request.GetRequestHeader("Content-Length");
                    long.TryParse(size, out length);
                    return length;
                });
        }
        /// <inheritdoc/>
        public WebRequestHandle<byte[]> AddUploadTaskAsync(string url, byte[] data, WebRequestUploadType uploadType, float timeoutSeconds = 30, int retryCount = 0, uint priority = 0)
        {
            return CreateHandle(url, timeoutSeconds, retryCount, priority,
                u =>
                {
                    UnityWebRequest request = null;
                    switch (uploadType)
                    {
                        case WebRequestUploadType.POST:
                            request = UnityWebRequest.Post(u, Utility.Converter.ConvertToString(data));
                            break;
                        case WebRequestUploadType.PUT:
                            request = UnityWebRequest.Put(u, data);
                            break;
                    }
                    return request;
                },
                request => request.downloadHandler != null ? request.downloadHandler.data : null);
        }
        /// <inheritdoc/>
        public WebRequestHandle<bool> AddDownloadFileTaskAsync(string url, string savePath, float timeoutSeconds = 30, int retryCount = 0, uint priority = 0)
        {
            return CreateHandle(url, timeoutSeconds, retryCount, priority,
                u =>
                {
                    var request = new UnityWebRequest(u, UnityWebRequest.kHttpVerbGET);
                    request.downloadHandler = new DownloadHandlerFile(savePath);
                    return request;
                },
                request => request.downloadHandler != null && request.isDone);
        }
        /// <summary>
        /// 创建句柄并注册执行
        /// </summary>
        WebRequestHandle<T> CreateHandle<T>(string url, float timeoutSeconds, int retryCount, uint priority,
            Func<string, UnityWebRequest> requestFactory, Func<UnityWebRequest, T> resultConverter)
        {
            var handle = new WebRequestHandle<T>(url, requestFactory, resultConverter);
            handle.TaskId = WebRequestTask.GetTaskId();
            handle.TimeoutSeconds = timeoutSeconds;
            handle.RetryCount = retryCount;
            handle.Priority = priority;
            webRequester.RegisterHandle(handle);
            return handle;
        }
        /// <inheritdoc/>
        public void CancelAllRequests()
        {
            webRequester.AbortRequestTasks();
        }
        #endregion

        #region 旧版API：事件式请求（兼容保留）
        ///<inheritdoc/>
        public long AddDownloadAssetBundleTask(string url)
        {
            var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
            return AddDownloadRequestTask(webRequest);
        }
        ///<inheritdoc/>
        public long AddDownloadAudioTask(string url, AudioType audioType)
        {
            var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            return AddDownloadRequestTask(webRequest);
        }
        ///<inheritdoc/>
        public long AddDownloadTextTask(string url)
        {
            var webRequest = UnityWebRequest.Get(url);
            return AddDownloadRequestTask(webRequest);
        }
        ///<inheritdoc/>
        public long AddDownloadTextureTask(string url)
        {
            var webRequest = UnityWebRequestTexture.GetTexture(url);
            return AddDownloadRequestTask(webRequest);
        }
        ///<inheritdoc/>
        public long AddDownloadRequestTask(string url)
        {
            var webRequest = UnityWebRequest.Get(url);
            return AddDownloadRequestTask(webRequest);
        }
        ///<inheritdoc/>
        public long AddUploadRequestTask(string url, byte[] data, WebRequestUploadType uploadType)
        {
            UnityWebRequest webRequest = null;
            switch (uploadType)
            {
                case WebRequestUploadType.POST:
                    webRequest = UnityWebRequest.Post(url, Utility.Converter.ConvertToString(data));
                    break;
                case WebRequestUploadType.PUT:
                    webRequest = UnityWebRequest.Put(url, data);
                    break;
            }
            return AddUploadRequestTask(webRequest);
        }
        ///<inheritdoc/>
        public long AddUploadRequestTask(UnityWebRequest webRequest)
        {
            var task = WebRequestTask.Create(webRequest.url, webRequest, WebRequestType.Upload);
            webRequester.AddTask(task);
            StartRequestTasks();
            return task.TaskId;
        }
        ///<inheritdoc/>
        public long AddDownloadRequestTask(UnityWebRequest webRequest)
        {
            var task = WebRequestTask.Create(webRequest.url, webRequest, WebRequestType.DownLoad);
            webRequester.AddTask(task);
            StartRequestTasks();
            return task.TaskId;
        }
        ///<inheritdoc/>
        public long AddGetContentLengthTask(string url)
        {
            var task = WebRequestTask.Create(url, null, WebRequestType.ContentLength);
            webRequester.AddTask(task);
            StartRequestTasks();
            return task.TaskId;
        }
        ///<inheritdoc/>
        public long AddUrlFileRequestTask(string url)
        {
            var urlFileRequestTask = ReferencePool.Acquire<WebUrlFileRequestTask>();
            var task = WebRequestTask.Create(url, null, WebRequestType.None);
            urlFileRequestTask.StartRequestUrl(task, OnUrlFileRequestTaskSuccess, OnUrlFileRequestTaskFailure);
            urlFileReqiestTaskDict.Add(task.TaskId, urlFileRequestTask);
            return task.TaskId;
        }
        ///<inheritdoc/>
        public bool RemoveTask(long taskId)
        {
            return webRequester.RemoveTask(taskId);
        }
        ///<inheritdoc/>
        public bool RemoveUrlFileRequestTask(long taskId)
        {
            if (urlFileReqiestTaskDict.Remove(taskId, out var urlFileRequestTask))
            {
                ReferencePool.Release(urlFileRequestTask);
                return true;
            }
            return false;
        }
        ///<inheritdoc/>
        public bool HasTask(long taskId)
        {
            return webRequester.HasTask(taskId);
        }
        ///<inheritdoc/>
        public void StartRequestTasks()
        {
            webRequester.StartRequestTasks();
        }
        ///<inheritdoc/>
        public void StopRequestTasks()
        {
            webRequester.StopRequestTasks();
        }
        ///<inheritdoc/>
        public void AbortRequestTasks()
        {
            webRequester.AbortRequestTasks();
        }
        protected override void OnTermination()
        {
            webRequester.AbortRequestTasks();
        }
        #endregion

        /// <summary>
        /// 请求url地址下的文件信息成功回调
        /// </summary>
        /// <param name="taskId">任务id</param>
        void OnUrlFileRequestTaskSuccess(long taskId)
        {
            if (urlFileReqiestTaskDict.Remove(taskId, out var urlFileRequestTask))
            {
                var taskUrl = urlFileRequestTask.WebRequestTask.URL;
                var urlFileInfos = urlFileRequestTask.UrlFileInfoList.ToArray();
                var timeSpan = urlFileRequestTask.TimeSpan;
                var eventArgs = WebRequestGetHtmlFilesSuccessEventArgs.Create(taskId, taskUrl, urlFileInfos, timeSpan);
                onGetHtmlFilesSuccessCallback?.Invoke(eventArgs);
                WebRequestGetHtmlFilesSuccessEventArgs.Release(eventArgs);
                ReferencePool.Release(urlFileRequestTask);
            }
        }
        /// <summary>
        /// 请求url地址下的文件信息失败回调
        /// </summary>
        /// <param name="taskId">任务id</param>
        void OnUrlFileRequestTaskFailure(long taskId)
        {
            if (urlFileReqiestTaskDict.Remove(taskId, out var urlFileRequestTask))
            {
                var taskUrl = urlFileRequestTask.WebRequestTask.URL;
                var urlFileInfos = urlFileRequestTask.UrlFileInfoList.ToArray();
                var timeSpan = urlFileRequestTask.TimeSpan;
                var errorMessages = urlFileRequestTask.ErrorMessageList.ToArray();
                var eventArgs = WebRequestGetHtmlFilesFailureEventArgs.Create(taskId, taskUrl, urlFileInfos, errorMessages, timeSpan);
                onGetHtmlFilesFailureCallback?.Invoke(eventArgs);
                WebRequestGetHtmlFilesFailureEventArgs.Release(eventArgs);
                ReferencePool.Release(urlFileRequestTask);
            }
        }
    }
}
