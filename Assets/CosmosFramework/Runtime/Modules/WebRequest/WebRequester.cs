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
        /// <summary>
        /// 是否开始任务
        /// </summary>
        public bool Running { get; private set; }
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
        /// 获取文件长度失败回调；
        /// </summary>
        public Action<WebRequestGetContentLengthFailureEventArgs> onGetContentLengthFailureCallback;
        /// <summary>
        /// 获取文件长度成功回调；
        /// </summary>
        public Action<WebRequestGetContentLengthSuccessEventArgs> onGetContentLengthSuccessCallback;
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
            WebRequestTask webRequestTask = default;
            if (CurrentTask != null)
            {
                if (CurrentTask.TaskId == taskId)
                {
                    CurrentWebRequest.Abort();
                }
                else
                {
                    if (taskDict.Remove(taskId, out webRequestTask))
                    {
                        taskList.Remove(webRequestTask);
                        return true;
                    }
                    return false;
                }
            }
            if (taskDict.Remove(taskId, out webRequestTask))
            {
                taskList.Remove(webRequestTask);
                return true;
            }
            return false;
        }
        public bool HasTask(long taskId)
        {
            return taskDict.ContainsKey(taskId);
        }
        /// <summary>
        /// 开启网络请求；
        /// </summary>
        public void StartRequestTasks()
        {
            if (!Running)
                Coroutine = Utility.Unity.StartCoroutine(MultipleRequest());
        }
        /// <summary>
        /// 停止网络请求，相当于暂停
        /// </summary>
        public void StopRequestTasks()
        {
            try
            {
                if (Coroutine != null)
                    Utility.Unity.StopCoroutine(Coroutine);
            }
            catch { }
            CurrentWebRequest?.Abort();
            Running = false;
        }
        /// <summary>
        /// 完全停止并清空任务
        /// </summary>
        public void AbortRequestTasks()
        {
            StopRequestTasks();
            taskDict.Clear();
            taskList.Clear();
        }
        IEnumerator MultipleRequest()
        {
            Running = true;
            while (taskList.Count > 0)
            {
                var task = taskList[0];
                CurrentTask = task;
                if (task.WebRequestType == WebRequestType.ContentLength)
                    yield return GetContentLengthRequest(task);
                else
                    yield return WebRequest(task);
                taskList.RemoveFirst();
                taskDict.Remove(task.TaskId);
                WebRequestTask.Release(task);

                var allTaskCompleteEventArgs = WebRequestAllTaskCompleteEventArgs.Create();
                onAllTaskCompleteCallback?.Invoke(allTaskCompleteEventArgs);
                WebRequestAllTaskCompleteEventArgs.Release(allTaskCompleteEventArgs);
            }
            Running = false;
        }
        IEnumerator WebRequest(WebRequestTask webRequestTask)
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
                    WebRequestUpdateEventArgs updateEventArgs = null;
                    switch (webRequestTask.WebRequestType)
                    {
                        case WebRequestType.DownLoad:
                            updateEventArgs = WebRequestUpdateEventArgs.Create(taskId, url, request.downloadProgress, request);
                            break;
                        case WebRequestType.Upload:
                            updateEventArgs = WebRequestUpdateEventArgs.Create(taskId, url, request.uploadProgress, request);
                            break;
                    }
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
        IEnumerator GetContentLengthRequest(WebRequestTask webRequestTask)
        {
            using (UnityWebRequest request = UnityWebRequest.Head(webRequestTask.URL))
            {
                yield return request.SendWebRequest();
                var size = request.GetRequestHeader("Content-Length");
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    var eventArgs = WebRequestGetContentLengthSuccessEventArgs.Create(webRequestTask.TaskId, webRequestTask.URL, Convert.ToInt64(size));
                    onGetContentLengthSuccessCallback?.Invoke(eventArgs);
                    WebRequestGetContentLengthSuccessEventArgs.Release(eventArgs);
                }
                else
                {
                    var eventArgs = WebRequestGetContentLengthFailureEventArgs.Create(webRequestTask.TaskId, webRequestTask.URL, request.error);
                    onGetContentLengthFailureCallback?.Invoke(eventArgs);
                    WebRequestGetContentLengthFailureEventArgs.Release(eventArgs);
                }
            }
        }
    }
}
