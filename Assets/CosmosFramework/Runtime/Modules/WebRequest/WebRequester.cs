using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    /// <summary>
    /// 网络请求执行器（兼容旧版事件式API）。
    /// <para>内部基于 <see cref="WebRequestScheduler"/> 与 <see cref="WebRequestHandle{T}"/> 并发执行，</para>
    /// <para>对外保留旧版的队列语义与事件回调，同时新增句柄式API由 <see cref="WebRequestManager"/> 暴露。</para>
    /// </summary>
    internal class WebRequester
    {
        /// <summary>
        /// 任务表：taskId === handle
        /// </summary>
        readonly Dictionary<long, WebRequestHandleBase> taskDict = new Dictionary<long, WebRequestHandleBase>();
        /// <summary>
        /// 并发调度器
        /// </summary>
        internal WebRequestScheduler Scheduler { get; } = new WebRequestScheduler();

        /// <summary>
        /// 当前任务数量
        /// </summary>
        public int TaskCount { get { return taskDict.Count; } }
        /// <summary>
        /// 是否存在执行中的任务
        /// </summary>
        public bool Running { get { return taskDict.Count > 0; } }
        /// <summary>
        /// 当前正在执行的请求对象（兼容旧版API）
        /// </summary>
        public UnityWebRequest CurrentWebRequest { get; private set; }
        /// <summary>
        /// 当前正在执行的任务（兼容旧版API）
        /// </summary>
        public WebRequestTask CurrentTask { get; private set; }

        /// <summary>
        /// 开始回调
        /// </summary>
        public Action<WebRequestStartEventArgs> onStartCallback;
        /// <summary>
        /// 进度回调
        /// </summary>
        public Action<WebRequestUpdateEventArgs> onUpdateCallback;
        /// <summary>
        /// 成功回调
        /// </summary>
        public Action<WebRequestSuccessEventArgs> onSuccessCallback;
        /// <summary>
        /// 失败回调
        /// </summary>
        public Action<WebRequestFailureEventArgs> onFailureCallback;
        /// <summary>
        /// 获取文件长度失败回调
        /// </summary>
        public Action<WebRequestGetContentLengthFailureEventArgs> onGetContentLengthFailureCallback;
        /// <summary>
        /// 获取文件长度成功回调
        /// </summary>
        public Action<WebRequestGetContentLengthSuccessEventArgs> onGetContentLengthSuccessCallback;
        /// <summary>
        /// 所有任务完成回调
        /// </summary>
        public Action<WebRequestAllTaskCompleteEventArgs> onAllTaskCompleteCallback;

        DateTime allTaskStartTime;
        bool allTaskStarted;

        /// <summary>
        /// 添加旧版任务
        /// </summary>
        public void AddTask(WebRequestTask webRequestTask)
        {
            if (webRequestTask == null)
                return;
            if (taskDict.ContainsKey(webRequestTask.TaskId))
                return;
            WebRequestHandleBase handle = null;
            switch (webRequestTask.WebRequestType)
            {
                case WebRequestType.ContentLength:
                    handle = CreateContentLengthHandle(webRequestTask);
                    break;
                case WebRequestType.Upload:
                    handle = CreateUploadHandle(webRequestTask);
                    break;
                case WebRequestType.DownLoad:
                default:
                    handle = CreateDownloadHandle(webRequestTask);
                    break;
            }
            RegisterHandle(handle);
        }

        /// <summary>
        /// 注册句柄（新API与旧版任务共用入口）
        /// </summary>
        public void RegisterHandle(WebRequestHandleBase handle)
        {
            if (handle == null)
                return;
            if (taskDict.ContainsKey(handle.TaskId))
                return;
            taskDict.Add(handle.TaskId, handle);
            if (!allTaskStarted)
            {
                allTaskStarted = true;
                allTaskStartTime = DateTime.Now;
            }
            SubscribeHandleEvents(handle);
            Scheduler.Enqueue(handle);
        }

        /// <summary>
        /// 移除任务。进行中或等待中的任务会被取消。
        /// </summary>
        public bool RemoveTask(long taskId)
        {
            if (taskDict.Remove(taskId, out var handle))
            {
                if (!handle.IsDone)
                    handle.Cancel();
                else
                    handle.DisposeRequest();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否存在任务
        /// </summary>
        public bool HasTask(long taskId)
        {
            return taskDict.ContainsKey(taskId);
        }

        /// <summary>
        /// 开始请求（调度器自动执行，保留方法以兼容旧版API）
        /// </summary>
        public void StartRequestTasks()
        {
            Scheduler.Paused = false;
        }

        /// <summary>
        /// 停止调度，效果与暂停相似：进行中的请求继续，不再派发新请求
        /// </summary>
        public void StopRequestTasks()
        {
            Scheduler.Paused = true;
        }

        /// <summary>
        /// 完全停止并清空任务
        /// </summary>
        public void AbortRequestTasks()
        {
            var handles = new List<WebRequestHandleBase>(taskDict.Values);
            taskDict.Clear();
            for (int i = 0; i < handles.Count; i++)
            {
                var handle = handles[i];
                if (!handle.IsDone)
                    handle.Cancel();
                else
                    handle.DisposeRequest();
            }
            Scheduler.CancelWaiting();
            Scheduler.Paused = false;
            allTaskStarted = false;
            CurrentTask = null;
            CurrentWebRequest = null;
        }

        void SubscribeHandleEvents(WebRequestHandleBase handle)
        {
            handle.OnStartedInternal += OnHandleStarted;
            handle.OnProgressInternal += OnHandleProgress;
            handle.OnCompletedInternal += OnHandleCompleted;
        }

        void OnHandleStarted(WebRequestHandleBase handle)
        {
            CurrentWebRequest = handle.UnityWebRequest;
            CurrentTask = CreateLegacyTask(handle);
            var eventArgs = WebRequestStartEventArgs.Create(handle.TaskId, handle.URL, handle.UnityWebRequest);
            onStartCallback?.Invoke(eventArgs);
            WebRequestStartEventArgs.Release(eventArgs);
        }

        void OnHandleProgress(WebRequestHandleBase handle, float progress)
        {
            if (onUpdateCallback == null)
                return;
            var timeSpan = DateTime.Now - handle.StartTime;
            var eventArgs = WebRequestUpdateEventArgs.Create(handle.TaskId, handle.URL, progress, handle.UnityWebRequest, timeSpan);
            onUpdateCallback?.Invoke(eventArgs);
            WebRequestUpdateEventArgs.Release(eventArgs);
        }

        void OnHandleCompleted(WebRequestHandleBase handle)
        {
            var timeSpan = DateTime.Now - handle.StartTime;
            var request = handle.UnityWebRequest;
            if (handle.IsSucceeded)
            {
                if (handle is WebRequestHandle<long>)
                {
                    var length = (long)((WebRequestHandle<long>)handle).Result;
                    var successArgs = WebRequestGetContentLengthSuccessEventArgs.Create(handle.TaskId, handle.URL, length, timeSpan);
                    onGetContentLengthSuccessCallback?.Invoke(successArgs);
                    WebRequestGetContentLengthSuccessEventArgs.Release(successArgs);
                }
                else
                {
                    var data = request != null && request.downloadHandler != null ? request.downloadHandler.data : null;
                    var successArgs = WebRequestSuccessEventArgs.Create(handle.TaskId, handle.URL, data, request, timeSpan);
                    onSuccessCallback?.Invoke(successArgs);
                    WebRequestSuccessEventArgs.Release(successArgs);
                }
            }
            else
            {
                if (handle is WebRequestHandle<long>)
                {
                    var failureArgs = WebRequestGetContentLengthFailureEventArgs.Create(handle.TaskId, handle.URL, handle.Error ?? "Request failed !", timeSpan);
                    onGetContentLengthFailureCallback?.Invoke(failureArgs);
                    WebRequestGetContentLengthFailureEventArgs.Release(failureArgs);
                }
                else
                {
                    var failureArgs = WebRequestFailureEventArgs.Create(handle.TaskId, handle.URL, handle.Error ?? "Request failed !", request, timeSpan);
                    onFailureCallback?.Invoke(failureArgs);
                    WebRequestFailureEventArgs.Release(failureArgs);
                }
            }
            taskDict.Remove(handle.TaskId);
            if (CurrentTask != null && CurrentTask.TaskId == handle.TaskId)
            {
                CurrentTask = null;
                CurrentWebRequest = null;
            }
            if (taskDict.Count == 0 && allTaskStarted)
            {
                allTaskStarted = false;
                var allCompleteArgs = WebRequestAllTaskCompleteEventArgs.Create(DateTime.Now - allTaskStartTime);
                onAllTaskCompleteCallback?.Invoke(allCompleteArgs);
                WebRequestAllTaskCompleteEventArgs.Release(allCompleteArgs);
            }
        }

        WebRequestTask CreateLegacyTask(WebRequestHandleBase handle)
        {
            return WebRequestTask.Create(handle.URL, handle.UnityWebRequest, WebRequestType.DownLoad);
        }

        #region Handle工厂（旧版任务转换）
        WebRequestHandleBase CreateDownloadHandle(WebRequestTask task)
        {
            var handle = new WebRequestHandle<byte[]>(task.URL,
                url => task.UnityWebRequest != null ? task.UnityWebRequest : UnityWebRequest.Get(url),
                request => request.downloadHandler != null ? request.downloadHandler.data : null);
            handle.TaskId = task.TaskId;
            return handle;
        }
        WebRequestHandleBase CreateUploadHandle(WebRequestTask task)
        {
            var handle = new WebRequestHandle<byte[]>(task.URL,
                url => task.UnityWebRequest,
                request => request.downloadHandler != null ? request.downloadHandler.data : null);
            handle.TaskId = task.TaskId;
            return handle;
        }
        WebRequestHandleBase CreateContentLengthHandle(WebRequestTask task)
        {
            var handle = new WebRequestHandle<long>(task.URL,
                url => UnityWebRequest.Head(url),
                request =>
                {
                    var size = request.GetRequestHeader("Content-Length");
                    long length = 0;
                    long.TryParse(size, out length);
                    return length;
                });
            handle.TaskId = task.TaskId;
            return handle;
        }
        #endregion
    }
}
