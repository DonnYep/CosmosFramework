using Cosmos.WebRequest;
using System;
using System.Collections;
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

        UnityWebRequest currentRequest;

        /// <inheritdoc/>
        public Coroutine RequestAssetBundleAsync(Uri uri, WebRequestCallback webDownloadCallback, Action<AssetBundle> resultCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequestAssetBundle.GetAssetBundle(uri), webDownloadCallback, req =>
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
                resultCallback?.Invoke(bundle);
            }));
        }
        /// <inheritdoc/>
        public Coroutine RequestAssetBundleAsync(string uri, WebRequestCallback webDownloadCallback, Action<AssetBundle> resultCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequestAssetBundle.GetAssetBundle(uri), webDownloadCallback, req =>
           {
               AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
               resultCallback?.Invoke(bundle);
           }));
        }
        /// <inheritdoc/>
        public Coroutine RequestAudioAsync(Uri uri, AudioType audioType, WebRequestCallback webDownloadCallback, Action<AudioClip> resultCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequestMultimedia.GetAudioClip(uri, audioType), webDownloadCallback, (UnityWebRequest req) =>
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                resultCallback?.Invoke(clip);
            }));
        }
        /// <inheritdoc/>
        public Coroutine RequestAudioAsync(string uri, AudioType audioType, WebRequestCallback webDownloadCallback, Action<AudioClip> resultCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequestMultimedia.GetAudioClip(uri, audioType), webDownloadCallback, (UnityWebRequest req) =>
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                resultCallback?.Invoke(clip);
            }));
        }
        /// <inheritdoc/>
        public Coroutine RequestFileBytesAsync(string uri, WebRequestCallback webDownloadCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequest.Get(uri), webDownloadCallback, null));
        }
        /// <inheritdoc/>
        public Coroutine RequestFileBytesAsync(Uri uri, WebRequestCallback webDownloadCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequest.Get(uri), webDownloadCallback, null));
        }
        /// <inheritdoc/>
        public Coroutine RequestTextAsync(string uri, WebRequestCallback webDownloadCallback, Action<string> resultCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequest.Get(uri), webDownloadCallback, req =>
            {
                resultCallback?.Invoke(req.downloadHandler.text);
            }));
        }
        /// <inheritdoc/>
        public Coroutine RequestTextAsync(Uri uri, WebRequestCallback webDownloadCallback, Action<string> resultCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequest.Get(uri), webDownloadCallback, req =>
            {
                resultCallback?.Invoke(req.downloadHandler.text);
            }));
        }
        /// <inheritdoc/>
        public Coroutine RequestTextureAsync(string uri, WebRequestCallback webDownloadCallback, Action<Texture2D> resultCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequestTexture.GetTexture(uri), webDownloadCallback, (UnityWebRequest req) =>
              {
                  Texture2D texture = DownloadHandlerTexture.GetContent(req);
                  resultCallback?.Invoke(texture);
              }));
        }
        /// <inheritdoc/>
        public Coroutine RequestTextureAsync(Uri uri, WebRequestCallback webDownloadCallback, Action<Texture2D> resultCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebRequest(UnityWebRequestTexture.GetTexture(uri), webDownloadCallback, (UnityWebRequest req) =>
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(req);
                resultCallback?.Invoke(texture);
            }));
        }
        /// <inheritdoc/>
        public Coroutine PostAsync(Uri uri, byte[] bytes, WebUploadCallback webUploadCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebUpload(UnityWebRequest.Post(uri, Utility.Converter.Convert2String(bytes)), webUploadCallback));
        }
        /// <inheritdoc/>
        public Coroutine PostAsync(string uri, byte[] bytes, WebUploadCallback webUploadCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebUpload(UnityWebRequest.Post(uri, Utility.Converter.Convert2String(bytes)), webUploadCallback));
        }
        /// <inheritdoc/>
        public Coroutine PutAsync(Uri uri, byte[] bytes, WebUploadCallback webUploadCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebUpload(UnityWebRequest.Put(uri, bytes), webUploadCallback));
        }
        /// <inheritdoc/>
        public Coroutine PutAsync(string uri, byte[] bytes, WebUploadCallback webUploadCallback)
        {
            return Utility.Unity.StartCoroutine(EnumWebUpload(UnityWebRequest.Put(uri, bytes), webUploadCallback));
        }
        /// <inheritdoc/>
        public void AbortAllRequest()
        {
            currentRequest?.Abort();
        }
        IEnumerator EnumWebRequest(UnityWebRequest unityWebRequest, WebRequestCallback webDownloadCallback, Action<UnityWebRequest> doneCallback)
        {
            using (UnityWebRequest request = unityWebRequest)
            {
                webDownloadCallback.OnStartCallback?.Invoke();
                currentRequest = request;
                IsLoading = true;
                request.SendWebRequest();
                while (!request.isDone)
                {
                    webDownloadCallback.OnUpdateCallback?.Invoke(request.downloadProgress);
                    yield return null;
                }
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    if (request.isDone)
                    {
                        webDownloadCallback.OnUpdateCallback?.Invoke(1);
                        webDownloadCallback.OnSuccessCallback?.Invoke(request.downloadHandler.data);
                        doneCallback?.Invoke(request);
                    }
                }
                else
                {
                    webDownloadCallback.OnFailureCallback?.Invoke(request.error);
                }
                IsLoading = false;
                currentRequest = null;
            }
        }
        IEnumerator EnumWebUpload(UnityWebRequest unityWebRequest, WebUploadCallback webUploadCallback)
        {
            using (UnityWebRequest request = unityWebRequest)
            {
                webUploadCallback.OnStartCallback?.Invoke();
                currentRequest = request;
                IsLoading = true;
                request.SendWebRequest();
                while (!request.isDone)
                {
                    webUploadCallback.OnUpdateCallback?.Invoke(request.uploadProgress);
                    yield return null;
                }
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    if (request.isDone)
                    {
                        webUploadCallback.OnUpdateCallback?.Invoke(1);
                        webUploadCallback.OnSuccessCallback?.Invoke();
                    }
                }
                else
                {
                    webUploadCallback.OnFailureCallback?.Invoke(request.error);
                }
                IsLoading = false;
                currentRequest = null;
            }
        }
    }
}
