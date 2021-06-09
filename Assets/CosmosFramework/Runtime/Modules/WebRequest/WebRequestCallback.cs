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
        Action startCallback;
        Action<float> updateCallback;
        Action<byte[]> successCallback;
        Action<string> failureCallback;
        /// <summary>
        /// 请求开始回调；
        /// </summary>
        public Action StartCallback { get { return startCallback; } }
        /// <summary>
        /// 请求执行中回调；
        /// Progress value 0~1;
        /// </summary>
        public Action<float> UpdateCallback { get { return updateCallback; } }
        /// <summary>
        /// 请求成功回调；
        /// Response bytes array;
        /// </summary>
        public Action<byte[]> SuccessCallback { get { return successCallback; } }
        /// <summary>
        /// 请求失败回调；
        /// ErrorMessage;
        /// </summary>
        public Action<string> FailureCallback { get { return failureCallback; } }
        public WebRequestCallback () { }
        public void Release()
        {
            startCallback=null;
            updateCallback = null;;
            successCallback = null; ;
            failureCallback = null; ;
        }
        public static WebRequestCallback Create(Action startCallback, Action<float> updateCallback,
            Action<byte[]> successCallback, Action<string> failureCallback)
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
