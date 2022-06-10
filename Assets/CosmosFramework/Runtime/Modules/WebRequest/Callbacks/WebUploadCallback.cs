using System;
namespace Cosmos.WebRequest
{
    public class WebUploadCallback : IReference
    {
        Action onStartCallback;
        Action<float> onUpdateCallback;
        Action onSuccessCallback;
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
        /// 上传成功回调；
        /// </summary>
        public Action OnSuccessCallback { get { return onSuccessCallback; } }
        /// <summary>
        /// 请求失败回调；
        /// ErrorMessage;
        /// </summary>
        public Action<string> OnFailureCallback { get { return onFailureCallback; } }
        public void Release()
        {
            onStartCallback = null;
            onUpdateCallback = null; ;
            onSuccessCallback = null; ;
            onFailureCallback = null; ;
        }
        public static WebUploadCallback Create(Action onStartCallback, Action<float> onUpdateCallback,
    Action onSuccessCallback, Action<string> onFailureCallback)
        {
            var webRequest = ReferencePool.Acquire<WebUploadCallback>();
            webRequest.onStartCallback = onStartCallback;
            webRequest.onUpdateCallback = onUpdateCallback;
            webRequest.onSuccessCallback = onSuccessCallback;
            webRequest.onFailureCallback = onFailureCallback;
            return webRequest;
        }
        public static void Release(WebUploadCallback webPostCallback)
        {
            ReferencePool.Release(webPostCallback);
        }
    }
}
