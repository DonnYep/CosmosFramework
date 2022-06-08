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
    * 3、helper可以自行实现并且切换，切换模块的状态是异步的，内部由
    * FutureTask进行异步状态的检测。
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
