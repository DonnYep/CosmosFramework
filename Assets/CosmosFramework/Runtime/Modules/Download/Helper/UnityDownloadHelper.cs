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
    public class UnityDownloadHelper// : IDownloadHelper
    {
        public bool Downloading { get; private set; }
        public DownloadInfo DownloadInfo { get; private set; }
        public async Task DownloadFileAsync(DownloadInfo downloadTask, Action<string> failureCallback, Action<byte[]> successCallback, Action<float> updateCalback)
        {
            DownloadInfo = downloadTask;
            await EnumDownloadWebRequest(downloadTask, failureCallback,successCallback,updateCalback);
        }
        /// <summary>
        /// 从断点开始继续下载；
        /// </summary>
        /// <param name="downloadTask">下载任务</param>
        /// <param name="startPosition">中断的位置</param>
        public async Task DownloadFileAsync(DownloadInfo downloadTask, long startPosition, Action<string> failureCallback, Action<byte[]> successCallback, Action<float> updateCalback)
        {
            DownloadInfo = downloadTask;
            await EnumDownloadWebRequest(downloadTask, failureCallback, successCallback, updateCalback);
        }
        IEnumerator EnumDownloadWebRequest(DownloadInfo downloadTask, Action<string> failureCallback, Action<byte[]> successCallback, Action<float> updateCalback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(downloadTask.Uri))
            {
                Downloading = true;
                request.timeout = downloadTask.Timeout;
                var startEventArgs = DownloadStartEventArgs.Create(request.url, downloadTask.DownloadPath, downloadTask.CustomeData);
                DownloadStartEventArgs.Release(startEventArgs);
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    var percentage = (int)request.downloadProgress * 100;
                    updateCalback?.Invoke(percentage);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        updateCalback?.Invoke(100);
                        successCallback.Invoke(request.downloadHandler.data);
                    }
                }
                else
                {
                    failureCallback.Invoke(request.error);
                    Downloading = false;
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
    }
}
