using Cosmos.WebRequest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos
{
    public class UnityWebRequestHelper: IWebRequestHelper
    {
        IWebRequestAgent webRequestAgent;
        public object DownloadTextAsync(string url, WebRequestCallback webRequestCallback)
        {
            var obj= webRequestAgent.DownloadAsync(url, webRequestCallback);

            return obj;
        }
        IEnumerator EnumUnityWebRequest(UnityWebRequest unityWebRequest, Action<float> progress, Action<UnityWebRequest> downloadedCallback)
        {
            using (UnityWebRequest request = unityWebRequest)
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    progress?.Invoke(request.downloadProgress);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        progress?.Invoke(1);
                        downloadedCallback(request);
                    }
                }
                else
                {
                    //throw new ArgumentException($"UnityWebRequest：{request.url } : {request.error } ！");
                }
            }
        }
    }
}
