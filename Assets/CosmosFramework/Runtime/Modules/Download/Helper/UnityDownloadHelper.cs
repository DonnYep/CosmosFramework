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
        Action downloadStart;
        Action<string> downloadFailure;
        Action<byte[]> downloadSuccess;
        Action<float> downloadUpdate;

        public event Action DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        public event Action<string> DownloadFailure
        {
            add { downloadFailure += value; }
            remove { downloadFailure -= value; }
        }
        public event Action<byte[]> DownloadSuccess
        {
            add { downloadSuccess += value; }
            remove { downloadSuccess -= value; }
        }
        public event Action<float> DownloadUpdate
        {
            add { downloadUpdate += value; }
            remove { downloadUpdate -= value; }
        }

        public bool Downloading { get; private set; }
        public DownloadInfo DownloadInfo { get; private set; }
        public async Task DownloadFileAsync(DownloadInfo downloadTask)
        {
            DownloadInfo = downloadTask;
            await EnumDownloadWebRequest(downloadTask);
        }
        /// <summary>
        /// 从断点开始继续下载；
        /// </summary>
        /// <param name="downloadTask">下载任务</param>
        /// <param name="startPosition">中断的位置</param>
        public async Task DownloadFileAsync(DownloadInfo downloadTask, long startPosition)
        {
            DownloadInfo = downloadTask;
            await EnumDownloadWebRequest(downloadTask);
        }
        IEnumerator EnumDownloadWebRequest(DownloadInfo downloadTask)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(downloadTask.Uri))
            {
                downloadStart?.Invoke();
                Downloading = true;
                request.timeout = downloadTask.Timeout;
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    var percentage = (int)request.downloadProgress * 100;
                    downloadUpdate?.Invoke(percentage);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        downloadUpdate?.Invoke(100);
                        downloadSuccess?.Invoke(request.downloadHandler.data);
                    }
                }
                else
                {
                    downloadFailure?.Invoke(request.error);
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
                var failureEventArgs = DownloadFailureEventArgs.Create(DownloadInfo.Uri, DownloadInfo.DownloadPath, "CancelDownload");
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
