using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
namespace Quark
{
    public partial class QuarkUtility
    {
        public static class Unity
        {
            #region UnityWebRequest
            public static Coroutine DownloadBytesAsync(string url, Action<float> progress, Action<byte[]> downloadedCallback, Action<string> failureCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
                {
                    downloadedCallback?.Invoke(req.downloadHandler.data);
                }, failureCallback));
            }
            public static Coroutine DownloadTextAsync(string url, Action<float> progress, Action<string> downloadedCallback, Action<string> failureCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
                {
                    downloadedCallback?.Invoke(req.downloadHandler.text);
                },failureCallback));
            }
            public static Coroutine DownloadTextureAsync(string url, Action<float> progress, Action<Texture2D> downloadedCallback, Action<string> failureCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequestTexture.GetTexture(url), progress, (UnityWebRequest req) =>
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(req);
                    downloadedCallback?.Invoke(texture);
                },failureCallback));
            }
            public static Coroutine DownloadAudioAsync(string url, AudioType audioType, Action<float> progress, Action<AudioClip> downloadedCallback, Action<string> failureCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequestMultimedia.GetAudioClip(url, audioType), progress, (UnityWebRequest req) =>
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                    downloadedCallback?.Invoke(clip);
                },failureCallback));
            }
            public static Coroutine DownloadAssetBundleAsync(string url, Action<float> progress, Action<AssetBundle> downloadedCallback, Action<string> failureCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequestAssetBundle.GetAssetBundle(url), progress, (UnityWebRequest req) =>
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
                    if (bundle)
                    {
                        downloadedCallback?.Invoke(bundle);
                    }
                },failureCallback));
            }
            public static Coroutine DownloadAssetBundleBytesAsync(string url, Action<float> progress, Action<byte[]> downloadedCallback, Action<string> failureCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
                {
                    var bundleBytes = req.downloadHandler.data;
                    if (bundleBytes != null)
                    {
                        downloadedCallback?.Invoke(bundleBytes);
                    }
                },failureCallback));
            }
            static IEnumerator EnumUnityWebRequest(UnityWebRequest unityWebRequest, Action<float> progress, Action<UnityWebRequest> downloadedCallback,Action<string>failureCallback)
            {
                using (UnityWebRequest request = unityWebRequest)
                {
                    var operation= request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        progress?.Invoke(operation.progress);
                        yield return null;
                    }
                    if (!request.isNetworkError && !request.isHttpError)
                    {
                        if (request.isDone)
                        {
                            progress?.Invoke(1);
                            downloadedCallback(request);
                        }
                    }
                    else
                    {
                        failureCallback?.Invoke(request.error);
                    }
                }
            }

            #endregion

            #region Coroutine
            public class CoroutineProvider : MonoBehaviour
            {
                public Coroutine StartCoroutine(Action handler)
                {
                    return StartCoroutine(EnumCoroutine(handler));
                }
                public Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
                {
                    return StartCoroutine(EnumPredicateCoroutine(handler, callBack));
                }
                public Coroutine StartCoroutine(Coroutine routine, Action callBack)
                {
                    return StartCoroutine(EnumCoroutine(routine, callBack));
                }
                public Coroutine StartCoroutine(Action handler, Action callback)
                {
                    return StartCoroutine(EnumCoroutine(handler, callback));
                }
                void Awake()
                {
                    this.gameObject.hideFlags = HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(gameObject);
                }
                IEnumerator EnumCoroutine(Action handler)
                {
                    handler?.Invoke();
                    yield return null;
                }
                IEnumerator EnumCoroutine(Coroutine routine, Action callBack)
                {
                    yield return routine;
                    callBack?.Invoke();
                }
                IEnumerator EnumCoroutine(Action handler, Action callack)
                {
                    yield return StartCoroutine(handler);
                    callack?.Invoke();
                }
                IEnumerator EnumPredicateCoroutine(Func<bool> handler, Action callBack)
                {
                    yield return new WaitUntil(handler);
                    callBack();
                }
            }
            static CoroutineProvider coroutineProvider;
            static CoroutineProvider CoroutineProviderComp
            {
                get
                {
                    if (coroutineProvider == null)
                    {
                        var go = new GameObject("QuarkCoroutinePorvider");
                        coroutineProvider = go.AddComponent<CoroutineProvider>();
                    }
                    return coroutineProvider;
                }
            }
            public static Coroutine StartCoroutine(IEnumerator routine)
            {
                return CoroutineProviderComp.StartCoroutine(routine);
            }
            public static Coroutine StartCoroutine(Action handler)
            {
                return CoroutineProviderComp.StartCoroutine(handler);
            }
            public static Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
            {
                return CoroutineProviderComp.PredicateCoroutine(handler, callBack);
            }
            public static Coroutine StartCoroutine(Coroutine routine, Action callBack)
            {
                return CoroutineProviderComp.StartCoroutine(routine, callBack);
            }
            public static Coroutine StartCoroutine(Action handler, Action callback)
            {
                return CoroutineProviderComp.StartCoroutine(handler, callback);
            }
            public static void StopCoroutine(IEnumerator routine)
            {
                CoroutineProviderComp.StopCoroutine(routine);
            }
            public static void StopCoroutine(Coroutine routine)
            {
                CoroutineProviderComp.StopCoroutine(routine);
            }
            #endregion
        }
    }
}
