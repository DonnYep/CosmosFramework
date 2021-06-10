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
    public class UnityDownloadAgentHelper //: IDownloadAgentHelper
    {
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadSuccessEventArgs> downloadUpdate;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;
        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        public event Action<DownloadSuccessEventArgs> DownloadUpdate
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


        //public object DownloadFileAsync(string uri, object customeData)
        //{
        //    return Utility.Unity.StartCoroutine(EnumDownloadWebRequest(UnityWebRequest.Get(uri)));
            
        //}

        //public object DownloadFileAsync(string uri, long startPosition, object customeData)
        //{
        //    return Utility.Unity.StartCoroutine(EnumDownloadWebRequest(UnityWebRequest.Get(uri)));
        //}
        //IEnumerator EnumDownloadWebRequest(UnityWebRequest unityWebRequest)
        //{
        //    using (UnityWebRequest request = unityWebRequest)
        //    {
        //        downloadStart?.Invoke(null);
        //        request.SendWebRequest();
        //        while (!request.isDone)
        //        {
        //            downloadUpdate?.Invoke(request.downloadProgress);
        //            yield return null;
        //        }
        //        if (!request.isNetworkError && !request.isHttpError)
        //        {
        //            if (request.isDone)
        //            {
        //                downloadUpdate?.Invoke(1);
        //                downloadSuccess?.Invoke(request.downloadHandler.data);
        //            }
        //        }
        //        else
        //        {
        //            downloadFailure?.Invoke(request.error);
        //        }
        //    }
        //}
        //IEnumerator EnumBytesUnityWebRequests(UnityWebRequest[] unityWebRequests, DownloadMultipleCallback downloadCallback)
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
