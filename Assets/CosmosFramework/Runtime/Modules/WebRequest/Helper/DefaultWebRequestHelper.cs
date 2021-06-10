using Cosmos.WebRequest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos
{
    public class DefaultWebRequestHelper : IWebRequestHelper
    {
        /// <summary>
        /// 是否正在加载；
        /// </summary>
        public bool IsLoading { get; private set; }
        /// <summary>
        /// 异步请求AssetBundle；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestAssetBundleAsync(Uri uri, WebRequestCallback webRequestCallback, Action<AssetBundle> resultCallback)
        {
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequestAssetBundle.GetAssetBundle(uri), webRequestCallback, req =>
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
                resultCallback?.Invoke(bundle);
            }));
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
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequestAssetBundle.GetAssetBundle(uri), webRequestCallback,  req =>
            {
                AssetBundle bundle= DownloadHandlerAssetBundle.GetContent(req);
                resultCallback?.Invoke(bundle);
            }));
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
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequestMultimedia.GetAudioClip(uri, audioType), webRequestCallback, (UnityWebRequest req) =>
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                resultCallback?.Invoke(clip);
            }));
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
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequestMultimedia.GetAudioClip(uri, audioType), webRequestCallback, (UnityWebRequest req) =>
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                resultCallback?.Invoke(clip);
            }));
        }
        /// <summary>
        /// 异步请求文件流；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestFileBytesAsync(string uri, WebRequestCallback webRequestCallback)
        {
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequest.Get(uri), webRequestCallback, null));
        }
        /// <summary>
        /// 异步请求文件流；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RequestFileBytesAsync(Uri uri, WebRequestCallback webRequestCallback)
        {
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequest.Get(uri), webRequestCallback, null));
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
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequest.Get(uri), webRequestCallback, req=> 
            {
                resultCallback?.Invoke(req.downloadHandler.text);
            }));
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
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequest.Get(uri), webRequestCallback, req =>
            {
                resultCallback?.Invoke(req.downloadHandler.text);
            }));
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
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequestTexture.GetTexture(uri), webRequestCallback, (UnityWebRequest req) =>
              {
                  Texture2D texture = DownloadHandlerTexture.GetContent(req);
                  resultCallback?.Invoke(texture);
              }));
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
            return Utility.Unity.StartCoroutine(EnumRequestWebRequest(UnityWebRequestTexture.GetTexture(uri), webRequestCallback, (UnityWebRequest req) =>
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(req);
                resultCallback?.Invoke(texture);
            }));
        }
        IEnumerator EnumRequestWebRequest(UnityWebRequest unityWebRequest, WebRequestCallback webRequestCallback, Action<UnityWebRequest> doneCallback)
        {
            using (UnityWebRequest request = unityWebRequest)
            {
                webRequestCallback.StartCallback?.Invoke();
                IsLoading = true ;
                request.SendWebRequest();
                while (!request.isDone)
                {
                    webRequestCallback.UpdateCallback?.Invoke(request.downloadProgress);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        webRequestCallback.UpdateCallback?.Invoke(1);
                        webRequestCallback.SuccessCallback?.Invoke(request.downloadHandler.data);
                        doneCallback?.Invoke(request);
                    }
                }
                else
                {
                    webRequestCallback.FailureCallback?.Invoke(request.error);
                }
                IsLoading = false;
            }
        }
    }
}
