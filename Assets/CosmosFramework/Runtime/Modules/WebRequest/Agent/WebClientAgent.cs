using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.WebRequest
{
    public class WebClientAgent : IWebRequestAgent
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
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webRequestCallback.StartCallback?.Invoke();
                    var task = webClient.DownloadDataTaskAsync(uri);
                    webClient.DownloadProgressChanged += (sender, eventArgs) =>
                    {
                        var progress = eventArgs.ProgressPercentage;
                        var percentage = (float)progress / 100f;
                        webRequestCallback.UpdateCallback.Invoke(percentage);
                    };
                    webClient.DownloadDataCompleted += (sender, eventArgs) =>
                    { 
                        webRequestCallback.SuccessCallback.Invoke(eventArgs.Result);
                    };
                    return task;
                }
                catch (Exception exception)
                {
                    webRequestCallback.FailureCallback(exception.ToString());
                    throw exception;
                }
            }
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
    }
}