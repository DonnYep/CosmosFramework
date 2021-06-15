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
    public class UnityDownloadAgentHelper : IDownloadAgentHelper
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
        public IEnumerator DownloadFileAsync(DownloadTask downloadTask, object customeData)
        {
            return EnumDownloadWebRequest(UnityWebRequest.Get(downloadTask.Uri), downloadTask.DownloadPath, customeData);
        }
        public IEnumerator DownloadFileAsync(DownloadTask downloadTask, long startPosition, object customeData)
        {
            return EnumDownloadWebRequest(UnityWebRequest.Get(downloadTask.Uri), downloadTask.DownloadPath, customeData);
        }
        IEnumerator EnumDownloadWebRequest(UnityWebRequest unityWebRequest, string downloadPath, object customeData)
        {
            using (UnityWebRequest request = unityWebRequest)
            {
                var startEventArgs = DownloadStartEventArgs.Create(request.url, downloadPath, customeData);
                downloadStart?.Invoke(startEventArgs);
                DownloadStartEventArgs.Release(startEventArgs);
                request.SendWebRequest();
                while (!request.isDone)
                {
                    var percentage = (int)request.downloadProgress * 100;
                    var updateEventArgs = DownloadUpdateEventArgs.Create(request.url, downloadPath, percentage, customeData);
                    downloadUpdate?.Invoke(updateEventArgs);
                    DownloadUpdateEventArgs.Release(updateEventArgs);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
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
                    var failureEventArgs = DownloadFailureEventArgs.Create(request.url, downloadPath, request.error, customeData);
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                }
            }
        }
        //IEnumerator EnumBytesUnityWebRequests(UnityWebRequest[] unityWebRequests)
        //{
        //    var length = unityWebRequests.Length;
        //    var count = length - 1;
        //    var requestBytesList = new List<byte[]>();
        //    for (int i = 0; i < length; i++)
        //    {
        //        downloadCallback.UpdateCallback?.Invoke((float)i / (float)count);
        //        yield return EnumDownloadWebRequest(unityWebRequests[i], downloadCallback);
        //    }
        //    downloadCallback.SuccessCallback.Invoke(requestBytesList);
        //}


    }
}
