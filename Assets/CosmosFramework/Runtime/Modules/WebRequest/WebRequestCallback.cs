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
        Action<object> startCallback;
        Action<float> updateCallback;
        Action<object> successCallback;
        Action<object> failureCallback;
        /// <summary>
        /// 请求开始回调；
        /// </summary>
        public Action<object> StartCallback { get { return startCallback; } }
        /// <summary>
        /// 请求执行中回调；
        /// </summary>
        public Action<float> UpdateCallback { get { return updateCallback; } }
        /// <summary>
        /// 请求成功回调；
        /// </summary>
        public Action<object> SuccessCallback { get { return successCallback; } }
        /// <summary>
        /// 请求失败回调；
        /// </summary>
        public Action<object> FailureCallback { get { return failureCallback; } }
        public WebRequestCallback () { }
        public void Release()
        {
            startCallback=null;
            updateCallback = null;;
            successCallback = null; ;
            failureCallback = null; ;
        }
        public static WebRequestCallback Create(Action<object> startCallback, Action<float> updateCallback, Action<object> successCallback, Action<object> failureCallback)
        {
            var webRequest = ReferencePool.Accquire<WebRequestCallback>();
            webRequest.startCallback = startCallback;
            webRequest.updateCallback = updateCallback;
            webRequest.successCallback = successCallback;
            webRequest.failureCallback = failureCallback;
            return webRequest;
        }
    }
}
