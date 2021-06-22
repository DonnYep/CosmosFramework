using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using UnityEngine.Networking;
using System.Collections;
namespace Cosmos.Download
{
    /// <summary>
    /// 使用UnityWebRequest下载的帮助体对象；
    /// 帮助体对象只实现单文件下载，多文件下载需由downloader调度完成；
    /// </summary>
    public class UnityDownloadHelper : IDownloadHelper
    {
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadUpdateEventArgs> downloadUpdate;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;

        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        public event Action<DownloadUpdateEventArgs> DownloadUpdate
        {
            add { downloadUpdate += value; }
            remove { downloadUpdate -= value; }
        }
        public event Action<DownloadSuccessEventArgs> DownloadSuccess
        {
            add { downloadSuccess += value; }
            remove { downloadSuccess -= value; }
        }
        public event Action<DownloadFailureEventArgs> DownloadFailure
        {
            add { downloadFailure += value; }
            remove { downloadFailure -= value; }
        }
        public bool Downloading { get; private set; }
        public DownloadInfo DownloadInfo { get; private set; }
        /// <summary>
        /// 开始异步下载文件；
        /// </summary>
        /// <param name="downloadTask">下载任务</param>
        /// <param name="customeData">自定义的数据</param>
        public Task DownloadFileAsync(DownloadInfo downloadTask, object customeData)
        {
            DownloadInfo = downloadTask;
            return EnumDownloadWebRequest(UnityWebRequest.Get(downloadTask.Uri), downloadTask.DownloadPath, downloadTask.Timeout, customeData);
        }
        /// <summary>
        /// 从断点开始继续下载；
        /// </summary>
        /// <param name="downloadTask">下载任务</param>
        /// <param name="startPosition">中断的位置</param>
        /// <param name="customeData">自定义数据类型</param>
        public Task DownloadFileAsync(DownloadInfo downloadTask, long startPosition, object customeData)
        {
            DownloadInfo = downloadTask;
            return EnumDownloadWebRequest(UnityWebRequest.Get(downloadTask.Uri), downloadTask.DownloadPath, downloadTask.Timeout, customeData);
        }
        async Task EnumDownloadWebRequest(UnityWebRequest unityWebRequest, string downloadPath, int timeout, object customeData)
        {
            using (UnityWebRequest request = unityWebRequest)
            {
                Downloading = true;
                request.timeout = timeout;
                var startEventArgs = DownloadStartEventArgs.Create(request.url, downloadPath, customeData);
                downloadStart?.Invoke(startEventArgs);
                DownloadStartEventArgs.Release(startEventArgs);
                await request.SendWebRequest();
                while (!request.isDone)
                {
                    var percentage = (int)request.downloadProgress * 100;
                    var updateEventArgs = DownloadUpdateEventArgs.Create(request.url, downloadPath, percentage, customeData);
                    downloadUpdate?.Invoke(updateEventArgs);
                    DownloadUpdateEventArgs.Release(updateEventArgs);
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var updateEventArgs = DownloadUpdateEventArgs.Create(request.url, downloadPath, 100, customeData);
                        downloadUpdate?.Invoke(updateEventArgs);
                        var successEventArgs = DownloadSuccessEventArgs.Create(request.url, downloadPath, request.downloadHandler.data, customeData);
                        downloadSuccess?.Invoke(successEventArgs);
                        DownloadUpdateEventArgs.Release(updateEventArgs);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                    }
                }
                else
                {
                    Downloading = false;
                    var failureEventArgs = DownloadFailureEventArgs.Create(request.url, downloadPath, request.error, customeData);
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                }
            }
        }
        /// <summary>
        /// 取消下载；
        /// 默认会缓存已经下载到的进度，便于下次断点续下；
        /// </summary>
        /// <param name="clearDownloadedFile">是否清理已经下载的的文件</param>
        public void CancelDownload(bool clearDownloadedFile = false)
        {
            if (!Downloading)
                return;
            if (DownloadInfo != null)
            {
                Downloading = false;
                var failureEventArgs = DownloadFailureEventArgs.Create(DownloadInfo.Uri, DownloadInfo.DownloadPath, "CancelDownload", null);
                downloadFailure?.Invoke(failureEventArgs);
                DownloadFailureEventArgs.Release(failureEventArgs);
                if (clearDownloadedFile)
                {
                    try
                    {
                        Utility.IO.DeleteFile(DownloadInfo.DownloadPath);
                    }
                    catch { }
                }
            }
        }

        public void ClearEvents()
        {
            downloadStart = null;
            downloadUpdate = null;
            downloadSuccess = null;
            downloadFailure = null;
        }
    }
}
