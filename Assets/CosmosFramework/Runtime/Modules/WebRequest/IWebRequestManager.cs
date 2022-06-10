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
    public interface IWebRequestManager : IModuleManager
    {
        /// <summary>
        /// 网络状态是否可用；
        /// </summary>
        bool NetworkReachable { get ; }
        /// <summary>
        /// 设置WebRequestHelper；
        /// </summary>
        /// <param name="webRequestHelper">自定义实现的WebRequestHelper</param>
        void SetHelperAsync(IWebRequestHelper webRequestHelper);
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
        /// <summary>
        /// 异步提交新建资源；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="bytes">数据流</param>
        /// <param name="webUploadCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine PostAsync(Uri uri, byte[] bytes, WebUploadCallback webUploadCallback);
        /// <summary>
        /// 异步提交新建资源；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="bytes">数据流</param>
        /// <param name="webUploadCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine PostAsync(string uri, byte[] bytes, WebUploadCallback webUploadCallback);
        /// <summary>
        /// 异步提交覆盖资源；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="bytes">数据流</param>
        /// <param name="webUploadCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine PutAsync(Uri uri, byte[] bytes, WebUploadCallback webUploadCallback);
        /// <summary>
        /// 异步提交覆盖资源；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="bytes">数据流</param>
        /// <param name="webUploadCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine PutAsync(string uri, byte[] bytes, WebUploadCallback webUploadCallback);
        /// <summary>
        /// 结束所有网络请求
        /// </summary>
        void AbortAllRequest();

    }
}
