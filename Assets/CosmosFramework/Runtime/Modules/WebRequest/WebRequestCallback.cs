using System;
namespace Cosmos.WebRequest
{
    /// <summary>
    /// WebRequest 回调对象；
    /// </summary>
    public class WebRequestCallback : IReference
    {
        Action onStartCallback;
        Action<float> onUpdateCallback;
        Action<byte[]> onSuccessCallback;
        Action<string> onFailureCallback;
        /// <summary>
        /// 请求开始回调；
        /// </summary>
        public Action OnStartCallback { get { return onStartCallback; } }
        /// <summary>
        /// 请求执行中回调；
        /// Progress value 0~1;
        /// </summary>
        public Action<float> OnUpdateCallback { get { return onUpdateCallback; } }
        /// <summary>
        /// 请求成功回调；
        /// Response bytes array;
        /// </summary>
        public Action<byte[]> OnSuccessCallback { get { return onSuccessCallback; } }
        /// <summary>
        /// 请求失败回调；
        /// ErrorMessage;
        /// </summary>
        public Action<string> OnFailureCallback { get { return onFailureCallback; } }
        public WebRequestCallback () { }
        public void Release()
        {
            onStartCallback=null;
            onUpdateCallback = null;;
            onSuccessCallback = null; ;
            onFailureCallback = null; ;
        }
        public static WebRequestCallback Create(Action onStartCallback, Action<float> onUpdateCallback,
            Action<byte[]> onSuccessCallback, Action<string> onFailureCallback)
        {
            var webRequest = ReferencePool.Acquire<WebRequestCallback>();
            webRequest.onStartCallback = onStartCallback;
            webRequest.onUpdateCallback = onUpdateCallback;
            webRequest.onSuccessCallback = onSuccessCallback;
            webRequest.onFailureCallback = onFailureCallback;
            return webRequest;
        }
        public static void Release(WebRequestCallback webRequestCallback)
        {
            ReferencePool.Release(webRequestCallback);
        }
    }
}
