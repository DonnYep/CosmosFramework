using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    /// <summary>
    /// 下载模块；
    /// </summary>
    //[Module]
    public class WebRequestManager : Module, IWebRequestManager
    {

        WebRequestMode currentWebRequestMode;
        Dictionary<WebRequestMode, WebRequestChannel> builtInChannelDict;
        IWebRequestAgent currentWebRequestHelper;
        /// <summary>
        /// 网络状态是否可用；
        /// </summary>
        public bool NetworkReachable { get { return Application.internetReachability != NetworkReachability.NotReachable; } }
        /// <summary>
        /// 当前web请求模式；
        /// </summary>
        public WebRequestMode CurrentWebRequestMode { get { return currentWebRequestMode; } }
        public override void OnInitialization()
        {
            builtInChannelDict = new Dictionary<WebRequestMode, WebRequestChannel>();
            var unityChannel = new WebRequestChannel(WebRequestMode.UnityWebRequest.ToString(), new UnityWebRequestAgent());
            var webClientChannel = new WebRequestChannel(WebRequestMode.WebClient.ToString(), new WebClientAgent());
            builtInChannelDict.Add(WebRequestMode.UnityWebRequest,webClientChannel);
            builtInChannelDict.Add(WebRequestMode.UnityWebRequest, unityChannel);
            currentWebRequestHelper = unityChannel.WebRequestHelper;
        }
        /// <summary>
        ///切换网络请求模式；
        /// </summary>
        /// <see cref="WebRequestMode"></see>
        /// <param name="webRequestMode">Web请求模式</param>
        /// <returns>是否切换成功</returns>
        public bool ChangeWebRequestMode(WebRequestMode webRequestMode)
        {
            bool result = false;
            currentWebRequestMode = webRequestMode;
            if( builtInChannelDict.TryGetValue(webRequestMode,out var channel))
            {
                currentWebRequestHelper = channel.WebRequestHelper;
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 异步下载资源；
        /// 注意，返回值类型可以是Task与Coroutine任意一种表示异步的引用对象；
        /// </summary>
        /// <see cref="Task">Return vaalue</see>
        /// <see cref="UnityEngine. Coroutine">Return vaalue</see>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">传入的回调</param>
        /// <returns>表示异步的引用对象</returns>
        public object DownloadAsync(string uri,WebRequestCallback webRequestCallback)
        {
            return currentWebRequestHelper.DownloadAsync(uri, webRequestCallback);
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
            return currentWebRequestHelper.UploadAsync(uri, webRequestCallback);
        }
    }
}
