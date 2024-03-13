using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    //================================================
    /*
     * 1、WebRequest用于加载AssetBundle资源。资源状态可以是Remote的，
    *  也可以是Local下persistentDataPath的；
     * 
     * 2、内置已经实现了一个默认的WebRequest帮助类对象；模块初始化时会
    * 自动加载并将默认的helper设置为此模块的默认加载helper；
    * 
    * 3、请求以队列形式存在。
    * 
    * 4、当前未支持请求优先级。
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
        /// <inheritdoc/>
        public long AddDownloadAssetBundleTask(string url)
        {
            var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
            return AddDownloadRequestTask(webRequest);
        }
        /// <inheritdoc/>
        public long AddDownloadAudioTask(string url, AudioType audioType)
        {
            var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            return AddDownloadRequestTask(webRequest);
        }
        /// <inheritdoc/>
        public long AddDownloadTextTask(string url)
        {
            var webRequest = UnityWebRequest.Get(url);
            return AddDownloadRequestTask(webRequest);
        }
        /// <inheritdoc/>
        public long AddDownloadTextureTask(string url)
        {
            var webRequest = UnityWebRequestTexture.GetTexture(url);
            return AddDownloadRequestTask(webRequest);
        }
        /// <inheritdoc/>
        public long AddDownloadRequestTask(string url)
        {
            var webRequest = UnityWebRequest.Get(url);
            return AddDownloadRequestTask(webRequest);
        }
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public long AddUploadRequestTask(UnityWebRequest webRequest)
        {
            var task = WebRequestTask.Create(webRequest.url, webRequest, WebRequestType.Upload);
            webRequester.AddTask(task);
            StartRequestTasks();
            return task.TaskId;
        }
        /// <inheritdoc/>
        public long AddDownloadRequestTask(UnityWebRequest webRequest)
        {
            var task = WebRequestTask.Create(webRequest.url, webRequest, WebRequestType.DownLoad);
            webRequester.AddTask(task);
            StartRequestTasks();
            return task.TaskId;
        }
        /// <inheritdoc/>
        public long AddGetContentLengthTask(string url)
        {
            var task = WebRequestTask.Create(url, null, WebRequestType.ContentLength);
            webRequester.AddTask(task);
            StartRequestTasks();
            return task.TaskId;
        }
        /// <inheritdoc/>
        public long AddUrlFileRequestTask(string url)
        {
            var urlFileRequestTask = ReferencePool.Acquire<WebUrlFileRequestTask>();
            var task = WebRequestTask.Create(url, null, WebRequestType.None);
            urlFileRequestTask.StartRequestUrl(task, OnUrlFileRequestTaskSuccess, OnUrlFileRequestTaskFailure);
            urlFileReqiestTaskDict.Add(task.TaskId, urlFileRequestTask);
            return task.TaskId;
        }
        /// <inheritdoc/>
        public bool RemoveTask(long taskId)
        {
            return webRequester.RemoveTask(taskId);
        }
        /// <inheritdoc/>
        public bool RemoveUrlFileRequestTask(long taskId)
        {
            if (urlFileReqiestTaskDict.Remove(taskId, out var urlFileRequestTask))
            {
                ReferencePool.Release(urlFileRequestTask);
                return true;
            }
            return false;
        }
        /// <inheritdoc/>
        public bool HasTask(long taskId)
        {
            return webRequester.HasTask(taskId);
        }
        /// <inheritdoc/>
        public void StartRequestTasks()
        {
            webRequester.StartRequestTasks();
        }
        /// <inheritdoc/>
        public void StopRequestTasks()
        {
            webRequester.StopRequestTasks();
        }
        /// <inheritdoc/>
        public void AbortRequestTasks()
        {
            webRequester.AbortRequestTasks();
        }
        protected override void OnTermination()
        {
            webRequester.StopRequestTasks();
        }
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
                var errorMessages= urlFileRequestTask.ErrorMessageList.ToArray();
                var eventArgs = WebRequestGetHtmlFilesFailureEventArgs.Create(taskId, taskUrl, urlFileInfos, errorMessages,timeSpan);
                onGetHtmlFilesFailureCallback?.Invoke(eventArgs);
                WebRequestGetHtmlFilesFailureEventArgs.Release(eventArgs);
                ReferencePool.Release(urlFileRequestTask);
            }
        }
    }
}
