using System;
using UnityEngine;
namespace Cosmos.WebRequest
{
    //================================================
    /*
     * 1、WebRequest用于加载AssetBundle资源。资源状态可以是Remote的，
    *  也可以是Local下persistentDataPath的；
     * 
     * 2、内置已经实现了一个默认的WebRequest帮助类对象；模块初始化时会
    * 自动加载并将默认的helper设置为此模块的默认加载helper；
    * 
    */
    //================================================
    [Module]
    internal class WebRequestManager : Module, IWebRequestManager
    {
        IWebRequestHelper webRequestHelper;
        /// <inheritdoc/>
        public bool NetworkReachable { get { return Application.internetReachability != NetworkReachability.NotReachable; } }
        /// <inheritdoc/>
        public async void SetHelperAsync(IWebRequestHelper webRequestHelper)
        {
            if (webRequestHelper != null)
                await new WaitUntil(() => { return webRequestHelper.IsLoading == false; });
            this.webRequestHelper = webRequestHelper;
        }
        /// <inheritdoc/>
        public Coroutine RequestAssetBundleAsync(string uri, WebRequestCallback webRequestCallback, Action<AssetBundle> resultCallback)
        {
            return webRequestHelper.RequestAssetBundleAsync(uri, webRequestCallback, resultCallback);
        }
        /// <inheritdoc/>
        public Coroutine RequestAssetBundleAsync(Uri uri, WebRequestCallback webRequestCallback, Action<AssetBundle> resultCallback)
        {
            return webRequestHelper.RequestAssetBundleAsync(uri, webRequestCallback, resultCallback);
        }
        /// <inheritdoc/>
        public Coroutine RequestAudioAsync(string uri, AudioType audioType, WebRequestCallback webRequestCallback, Action<AudioClip> resultCallback)
        {
            return webRequestHelper.RequestAudioAsync(uri, audioType, webRequestCallback, resultCallback);
        }
        /// <inheritdoc/>
        public Coroutine RequestAudioAsync(Uri uri, AudioType audioType, WebRequestCallback webRequestCallback, Action<AudioClip> resultCallback)
        {
            return webRequestHelper.RequestAudioAsync(uri, audioType, webRequestCallback, resultCallback);
        }
        /// <inheritdoc/>
        public Coroutine RequestFileBytesAsync(string uri, WebRequestCallback webRequestCallback)
        {
            return webRequestHelper.RequestFileBytesAsync(uri, webRequestCallback);
        }
        /// <inheritdoc/>
        public Coroutine RequestFileBytesAsync(Uri uri, WebRequestCallback webRequestCallback)
        {
            return webRequestHelper.RequestFileBytesAsync(uri, webRequestCallback);
        }
        /// <inheritdoc/>
        public Coroutine RequestTextAsync(string uri, WebRequestCallback webRequestCallback, Action<string> resultCallback)
        {
            return webRequestHelper.RequestTextAsync(uri, webRequestCallback, resultCallback);
        }
        /// <inheritdoc/>
        public Coroutine RequestTextAsync(Uri uri, WebRequestCallback webRequestCallback, Action<string> resultCallback)
        {
            return webRequestHelper.RequestTextAsync(uri, webRequestCallback, resultCallback);
        }
        /// <inheritdoc/>
        public Coroutine RequestTextureAsync(string uri, WebRequestCallback webRequestCallback, Action<Texture2D> resultCallback)
        {
            return webRequestHelper.RequestTextureAsync(uri, webRequestCallback, resultCallback);
        }
        /// <inheritdoc/>
        public Coroutine RequestTextureAsync(Uri uri, WebRequestCallback webRequestCallback, Action<Texture2D> resultCallback)
        {
            return webRequestHelper.RequestTextureAsync(uri, webRequestCallback, resultCallback);
        }
        /// <inheritdoc/>
        public Coroutine PostAsync(Uri uri, byte[] bytes, WebUploadCallback webUploadCallback)
        {
            return webRequestHelper.PostAsync(uri, bytes, webUploadCallback);
        }
        /// <inheritdoc/>
        public Coroutine PostAsync(string uri, byte[] bytes, WebUploadCallback webUploadCallback)
        {
            return webRequestHelper.PostAsync(uri, bytes, webUploadCallback);
        }
        /// <inheritdoc/>
        public Coroutine PutAsync(Uri uri, byte[] bytes, WebUploadCallback webUploadCallback)
        {
            return webRequestHelper.PutAsync(uri, bytes, webUploadCallback);
        }
        /// <inheritdoc/>
        public Coroutine PutAsync(string uri, byte[] bytes, WebUploadCallback webUploadCallback)
        {
            return webRequestHelper.PutAsync(uri, bytes, webUploadCallback);
        }
        /// <inheritdoc/>
        public void AbortAllRequest()
        {
            webRequestHelper.AbortAllRequest();
        }
        protected override void OnInitialization()
        {
            //初始化时会加载默认helper
            webRequestHelper = new DefaultWebRequestHelper();
        }
        protected override void OnTermination()
        {
            webRequestHelper.AbortAllRequest();
        }
    }
}
