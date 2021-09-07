using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.WebRequest
{
    public interface IWebRequestHelper
    {
        /// <summary>
        /// 是否正在加载；
        /// </summary>
        bool IsLoading { get; }
        /// <summary>
        /// 异步请求文件流；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestFileBytesAsync(string uri, WebRequestCallback webRequestCallback);
        /// <summary>
        /// 异步请求文件流；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestFileBytesAsync(Uri uri, WebRequestCallback webRequestCallback);
        /// <summary>
        /// 异步请求Text；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestTextAsync(string uri, WebRequestCallback webRequestCallback, Action<string> resultCallback);
        /// <summary>
        /// 异步请求Text；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestTextAsync(Uri uri, WebRequestCallback webRequestCallback, Action<string> resultCallback);
        /// <summary>
        /// 异步请求Texture；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestTextureAsync(string uri, WebRequestCallback webRequestCallback, Action<Texture2D> resultCallback);
        /// <summary>
        /// 异步请求Texture；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestTextureAsync(Uri uri, WebRequestCallback webRequestCallback, Action<Texture2D> resultCallback);
        /// <summary>
        /// 异步请求AssetBundle；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestAssetBundleAsync(string uri, WebRequestCallback webRequestCallback, Action<AssetBundle> resultCallback);
        /// <summary>
        /// 异步请求AssetBundle；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestAssetBundleAsync(Uri uri, WebRequestCallback webRequestCallback, Action<AssetBundle> resultCallback);
        /// <summary>
        /// 异步请求Audio；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="audioType">声音类型</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestAudioAsync(string uri, AudioType audioType, WebRequestCallback webRequestCallback, Action<AudioClip> resultCallback);
        /// <summary>
        /// 异步请求Audio；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="audioType">声音类型</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestAudioAsync(Uri uri, AudioType audioType, WebRequestCallback webRequestCallback, Action<AudioClip> resultCallback);
    }
}
