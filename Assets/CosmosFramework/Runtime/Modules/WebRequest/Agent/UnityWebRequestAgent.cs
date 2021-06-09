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
    public class UnityWebRequestAgent:IWebRequestAgent
    {
        /// <summary>
        /// 异步下载资源；
        /// 注意，返回值类型可以是Task与Coroutine任意一种表示异步的引用对象；
        /// </summary>
        /// <see cref="Task">Return vaalue</see>
        /// <see cref="UnityEngine. Coroutine">Return vaalue</see>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">传入的回调</param>
        /// <returns>表示异步的引用对象</returns>
        public object DownloadAsync(string uri, WebRequestCallback webRequestCallback)
        {
            return Utility.Unity.StartCoroutine(EnumDownloadWebRequest(UnityWebRequest.Get(uri), webRequestCallback));
        }
        /// <summary>
        /// 异步上传资源；
        /// 注意，返回值类型可以是Task与Coroutine任意一种表示异步的引用对象；
        /// </summary>
        /// <see cref="Task">Return vaalue</see>
        /// <see cref="UnityEngine. Coroutine">Return vaalue</see>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">传入的回调</param>
        /// <returns>表示异步的引用对象</returns>
        public object UploadAsync(string uri, WebRequestCallback webRequestCallback)
        {
            return null;
        }
        IEnumerator EnumDownloadWebRequest(UnityWebRequest unityWebRequest, WebRequestCallback webRequestCallback)
        {
            using (UnityWebRequest request = unityWebRequest)
            {
                webRequestCallback.StartCallback?.Invoke();
                request.SendWebRequest();
                while (!request.isDone)
                {
                    webRequestCallback.UpdateCallback?.Invoke(request.downloadProgress);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        webRequestCallback.UpdateCallback?.Invoke(1);
                        webRequestCallback.SuccessCallback?.Invoke(request.downloadHandler.data);
                    }
                }
                else
                {
                    webRequestCallback.FailureCallback?.Invoke(request.error);
                }
            }
        }
    }
}
