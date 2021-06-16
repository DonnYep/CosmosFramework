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
    //================================================
    //1、WebRequest用于加载AssetBundle资源。资源状态可以是Remote的，
    // 也可以是Local下persistentDataPath的；
    //2、内置已经实现了一个默认的WebRequest帮助类对象；模块初始化时会
    // 自动加载并将默认的helper设置为此模块的默认加载helper；
    //3、helper可以自行实现并且切换，切换模块的状态是异步的，内部由
    // FutureTask进行异步状态的检测。
    //================================================
    [Module]
    internal class WebRequestManager : Module, IWebRequestManager
    {
        IWebRequestHelper webRequestHelper;
        string url;
        /// <summary>
        /// 网络状态是否可用；
        /// </summary>
        public bool NetworkReachable { get { return Application.internetReachability != NetworkReachability.NotReachable; } }
        /// <summary>
        /// Uniform Resource Locator；
        /// 资源所在的地址，需要手动赋值；
        /// 此地址可以是本地持久化地址，亦可是Remote Web地址；
        /// </summary>
        public string URL { get { return url; } set { url = value; } }
        public override void OnPreparatory()
        {
            //初始化时会加载默认helper
            webRequestHelper = new DefaultWebRequestHelper();
        }
        /// <summary>
        /// 设置WebRequestHelper；
        /// </summary>
        /// <param name="webRequestHelper">自定义实现的WebRequestHelper</param>
        public async void SetHelperAsync(IWebRequestHelper webRequestHelper)
        {
            if (webRequestHelper != null)
                await new WaitUntil(() => { return webRequestHelper.IsLoading == false; });
            this.webRequestHelper = webRequestHelper;
        }
        /// <summary>
        /// 异步请求AssetBundle；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestAssetBundleAsync(string uri, WebRequestCallback webRequestCallback, Action<AssetBundle> resultCallback)
        {
            return webRequestHelper.RequestAssetBundleAsync(uri, webRequestCallback, resultCallback);
        }
        /// <summary>
        /// 异步请求AssetBundle；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestAssetBundleAsync(Uri uri, WebRequestCallback webRequestCallback, Action<AssetBundle> resultCallback)
        {
            return webRequestHelper.RequestAssetBundleAsync(uri, webRequestCallback, resultCallback);
        }
        /// <summary>
        /// 异步请求Audio；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="audioType">声音类型</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestAudioAsync(string uri, AudioType audioType, WebRequestCallback webRequestCallback, Action<AudioClip> resultCallback)
        {
            return webRequestHelper.RequestAudioAsync(uri, audioType, webRequestCallback, resultCallback);
        }
        /// <summary>
        /// 异步请求Audio；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="audioType">声音类型</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestAudioAsync(Uri uri, AudioType audioType, WebRequestCallback webRequestCallback, Action<AudioClip> resultCallback)
        {
            return webRequestHelper.RequestAudioAsync(uri, audioType, webRequestCallback, resultCallback);
        }
        /// <summary>
        /// 异步请求文件流；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestFileBytesAsync(string uri, WebRequestCallback webRequestCallback)
        {
            return webRequestHelper.RequestFileBytesAsync(uri, webRequestCallback);
        }
        /// <summary>
        /// 异步请求文件流；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestFileBytesAsync(Uri uri, WebRequestCallback webRequestCallback)
        {
            return webRequestHelper.RequestFileBytesAsync(uri, webRequestCallback);
        }
        /// <summary>
        /// 异步请求Text；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestTextAsync(string uri, WebRequestCallback webRequestCallback, Action<string> resultCallback)
        {
            return webRequestHelper.RequestTextAsync(uri, webRequestCallback, resultCallback);
        }
        /// <summary>
        /// 异步请求Text；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestTextAsync(Uri uri, WebRequestCallback webRequestCallback, Action<string> resultCallback)
        {
            return webRequestHelper.RequestTextAsync(uri, webRequestCallback, resultCallback);
        }
        /// <summary>
        /// 异步请求Texture；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestTextureAsync(string uri, WebRequestCallback webRequestCallback, Action<Texture2D> resultCallback)
        {
            return webRequestHelper.RequestTextureAsync(uri, webRequestCallback, resultCallback);
        }
        /// <summary>
        /// 异步请求Texture；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestTextureAsync(Uri uri, WebRequestCallback webRequestCallback, Action<Texture2D> resultCallback)
        {
            return webRequestHelper.RequestTextureAsync(uri, webRequestCallback, resultCallback);
        }
    }
}
