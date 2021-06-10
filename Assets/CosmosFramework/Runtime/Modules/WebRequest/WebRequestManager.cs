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
        IWebRequestHelper webRequestHelper;
        string _url;
        /// <summary>
        /// 网络状态是否可用；
        /// </summary>
        public bool NetworkReachable { get { return Application.internetReachability != NetworkReachability.NotReachable; } }
        /// <summary>
        /// Uniform Resource Locator；
        /// </summary>
        public string URL { get { return _url; } set { _url = value; } }
        public override void OnPreparatory()
        {
            //初始化时会加载默认helper
            webRequestHelper = new DefaultWebRequestHelper();
        }
        /// <summary>
        /// 设置WebRequestHelper；
        /// </summary>
        /// <param name="webRequestHelper">自定义实现的WebRequestHelper</param>
        public void SetWebRequestHelper(IWebRequestHelper webRequestHelper)
        {
            if (webRequestHelper != null)
            {
                if (!webRequestHelper.IsLoading)
                    this.webRequestHelper = webRequestHelper;
                else
                {
                    //Utility.Unity.PredicateCoroutine(() => webRequestHelper.IsLoading, () =>
                    //{
                    //    this.webRequestHelper = webRequestHelper;
                    //});

                    //这里需要异步等待一个任务；协程等待协程会阻塞，因此使用自己实现的任务池进行监控；
                    //error
                }
              
            }
            else
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
            return webRequestHelper.RequestAudioAsync(uri, audioType, webRequestCallback,resultCallback);
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
            return webRequestHelper.RequestTextAsync(uri, webRequestCallback,resultCallback);
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
