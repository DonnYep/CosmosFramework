using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// WebRequest 回调对象；
    /// </summary>
    public class WebRequestCallback : IReference
    {
        EventHandler<WebRequestStartEventArgs> startCallback;
        EventHandler<WebRequestUpdateEventArgs> updateCallback;
        EventHandler<WebRequestSuccessEventArgs> successCallback;
        EventHandler<WebRequestFailureEventArgs> failureCallback;
        /// <summary>
        /// 请求开始回调；
        /// </summary>
        public EventHandler<WebRequestStartEventArgs> StartCallback { get { return startCallback; } }
        /// <summary>
        /// 请求执行中回调；
        /// </summary>
        public EventHandler<WebRequestUpdateEventArgs> UpdateCallback { get { return updateCallback; } }
        /// <summary>
        /// 请求成功回调；
        /// </summary>
        public EventHandler<WebRequestSuccessEventArgs> SuccessCallback { get { return successCallback; } }
        /// <summary>
        /// 请求失败回调；
        /// </summary>
        public EventHandler<WebRequestFailureEventArgs> FailureCallback { get { return failureCallback; } }
        public WebRequestCallback () { }
        public void Release()
        {
            startCallback=null;
            updateCallback = null;;
            successCallback = null; ;
            failureCallback = null; ;
        }
        public static WebRequestCallback Create(EventHandler<WebRequestStartEventArgs> startCallback, EventHandler<WebRequestUpdateEventArgs> updateCallback,
            EventHandler<WebRequestSuccessEventArgs> successCallback, EventHandler<WebRequestFailureEventArgs> failureCallback)
        {
            var webRequest = ReferencePool.Accquire<WebRequestCallback>();
            webRequest.startCallback = startCallback;
            webRequest.updateCallback = updateCallback;
            webRequest.successCallback = successCallback;
            webRequest.failureCallback = failureCallback;
            return webRequest;
        }
        public static void Release(WebRequestCallback webRequestCallback)
        {
            ReferencePool.Release(webRequestCallback);
        }
    }
}
