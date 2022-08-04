using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    internal class WebRequester
    {
        /// <summary>
        /// 任务队列；
        /// </summary>
        List<WebRequestTask> taskList = new List<WebRequestTask>();
        Dictionary<long, WebRequestTask> taskDict = new Dictionary<long, WebRequestTask>();
        public int TaskCount { get { return taskList.Count; } }
        public bool InExecution { get; private set; }
        /// <summary>
        /// 开始回调；
        /// </summary>
        public Action<WebRequestStartEventArgs> onStartCallback;
        /// <summary>
        /// 进度回调；
        /// </summary>
        public Action<WebRequestUpdateEventArgs> onUpdateCallback;
        /// <summary>
        /// 成功回调；
        /// </summary>
        public Action<WebRequestSuccessEventArgs> onSuccessCallback;
        /// <summary>
        /// 失败回调；
        /// </summary>
        public Action<WebRequestFailureEventArgs> onFailureCallback;
        /// <summary>
        /// 所有任务完成回调；
        /// </summary>
        public Action<WebRequestAllTaskCompleteEventArgs> onAllTaskCompleteCallback;
        public UnityWebRequest CurrentWebRequest { get; private set; }
        public Coroutine Coroutine { get; private set; }
        public WebRequestTask CurrentTask { get; private set; }
        public void AddTask(WebRequestTask webRequestTask)
        {
            if (!taskDict.ContainsKey(webRequestTask.TaskId))
            {
                taskList.Add(webRequestTask);
                taskDict.Add(webRequestTask.TaskId, webRequestTask);
            }
        }
        public bool RemoveTask(long taskId)
        {
            if (taskDict.Remove(taskId, out var webRequestTask))
            {
                taskList.Remove(webRequestTask);
                return true;
            }
            return false;
        }
        public bool StopTask(long taskId)
        {
            if (CurrentTask != null)
            {
                if (CurrentTask.TaskId == taskId)
                {
                    CurrentWebRequest.Abort();
                }
                else
                    return RemoveTask(taskId);
            }
            return RemoveTask(taskId);
        }
        public bool HashTask(long taskId)
        {
            return taskDict.ContainsKey(taskId);
        }
        /// <summary>
        /// 开启网络请求；
        /// </summary>
        public void StartRequestTasks()
        {
            if (!InExecution)
                Coroutine = Utility.Unity.StartCoroutine(MultipleRequest());
        }
        /// <summary>
        /// 结束网络请求
        /// </summary>
        public void StopRequestTasks()
        {
            if (Coroutine != null)
                Utility.Unity.StopCoroutine(Coroutine);
            CurrentWebRequest?.Abort();
            InExecution = false;
        }
        public void AbortRequestTasks()
        {
            StopRequestTasks();
            taskDict.Clear();
            taskList.Clear();
        }
        IEnumerator MultipleRequest()
        {
            InExecution = true;
            while (taskList.Count > 0)
            {
                var task = taskList[0];
                CurrentTask = task;
                switch (task.WebRequestType)
                {
                    case WebRequestType.DownLoad:
                        yield return DownloadRequest(task);
                        break;
                    case WebRequestType.Upload:
                        yield return UploadRequest(task);
                        break;
                }
                taskList.RemoveFirst();
                taskDict.Remove(task.TaskId);
                WebRequestTask.Release(task);

                var allTaskCompleteEventArgs = WebRequestAllTaskCompleteEventArgs.Create();
                onAllTaskCompleteCallback?.Invoke(allTaskCompleteEventArgs);
                WebRequestAllTaskCompleteEventArgs.Release(allTaskCompleteEventArgs);
            }
            InExecution = false;
        }
        IEnumerator DownloadRequest(WebRequestTask webRequestTask)
        {
            var unityWebRequest = webRequestTask.UnityWebRequest;
            var taskId = webRequestTask.TaskId;
            var url = unityWebRequest.url;
            using (UnityWebRequest request = unityWebRequest)
            {
                var startEventArgs = WebRequestStartEventArgs.Create(taskId, url, request);
                onStartCallback?.Invoke(startEventArgs);
                WebRequestStartEventArgs.Release(startEventArgs);
                CurrentWebRequest = request;
                request.SendWebRequest();
                while (!request.isDone)
                {
                    var updateEventArgs = WebRequestUpdateEventArgs.Create(taskId, url, request.downloadProgress, request);
                    onUpdateCallback?.Invoke(updateEventArgs);
                    WebRequestUpdateEventArgs.Release(updateEventArgs);
                    yield return null;
                }
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    if (request.isDone)
                    {
                        var updateEventArgs = WebRequestUpdateEventArgs.Create(taskId, url, 1, request);
                        onUpdateCallback?.Invoke(updateEventArgs);
                        WebRequestUpdateEventArgs.Release(updateEventArgs);
                        var successEventArgs = WebRequestSuccessEventArgs.Create(taskId, url, request.downloadHandler.data, request);
                        onSuccessCallback?.Invoke(successEventArgs);
                        WebRequestSuccessEventArgs.Release(successEventArgs);
                    }
                }
                else
                {
                    var failureEventArgs = WebRequestFailureEventArgs.Create(taskId, url, request.error, request);
                    onFailureCallback?.Invoke(failureEventArgs);
                    WebRequestFailureEventArgs.Release(failureEventArgs);
                }
                CurrentWebRequest = null;
            }
        }
        IEnumerator UploadRequest(WebRequestTask webRequestTask)
        {
            var unityWebRequest = webRequestTask.UnityWebRequest;
            var taskId = webRequestTask.TaskId;
            var url = unityWebRequest.url;
            using (UnityWebRequest request = unityWebRequest)
            {
                var startEventArgs = WebRequestStartEventArgs.Create(taskId, url, request);
                onStartCallback?.Invoke(startEventArgs);
                WebRequestStartEventArgs.Release(startEventArgs);
                CurrentWebRequest = request;
                request.SendWebRequest();
                while (!request.isDone)
                {
                    var updateEventArgs = WebRequestUpdateEventArgs.Create(taskId, url, request.uploadProgress, request);
                    onUpdateCallback?.Invoke(updateEventArgs);
                    WebRequestUpdateEventArgs.Release(updateEventArgs);
                    yield return null;
                }
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    if (request.isDone)
                    {
                        var updateEventArgs = WebRequestUpdateEventArgs.Create(taskId, url, 1, request);
                        onUpdateCallback?.Invoke(updateEventArgs);
                        WebRequestUpdateEventArgs.Release(updateEventArgs);
                        var successEventArgs = WebRequestSuccessEventArgs.Create(taskId, url, request.downloadHandler.data, request);
                        onSuccessCallback?.Invoke(successEventArgs);
                        WebRequestSuccessEventArgs.Release(successEventArgs);
                    }
                }
                else
                {
                    var failureEventArgs = WebRequestFailureEventArgs.Create(taskId, url, request.error, request);
                    onFailureCallback?.Invoke(failureEventArgs);
                    WebRequestFailureEventArgs.Release(failureEventArgs);
                }
                CurrentWebRequest = null;
            }
        }
    }
}
