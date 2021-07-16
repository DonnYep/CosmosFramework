using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using UnityEngine;
using UnityEngine.Networking;
namespace Cosmos.Quark
{
    public partial class QuarkUtility
    {
        public static class Unity
        {
            #region UnityWebRequest
            public static Coroutine DownloadTextAsync(string url, Action<float> progress, Action<string> downloadedCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
                {
                    downloadedCallback?.Invoke(req.downloadHandler.text);
                }));
            }
            public static Coroutine DownloadTextsAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<string[]> downloadedCallback)
            {
                var length = urls.Length;
                var requests = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    requests.Add(UnityWebRequest.Get(urls[i]));
                }
                return StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Length;
                    var texts = new string[reqLength];
                    for (int i = 0; i < reqLength; i++)
                    {
                        texts[i] = reqs[i].downloadHandler.text;
                    }
                    downloadedCallback?.Invoke(texts);
                }));
            }
            public static Coroutine DownloadTextureAsync(string url, Action<float> progress, Action<Texture2D> downloadedCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequestTexture.GetTexture(url), progress, (UnityWebRequest req) =>
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(req);
                    downloadedCallback?.Invoke(texture);
                }));
            }
            public static Coroutine DownloadTexturesAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<Texture2D[]> downloadedCallback)
            {
                var length = urls.Length;
                var requests = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    requests.Add(UnityWebRequestTexture.GetTexture(urls[i]));
                }
                return StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Length;
                    var textures = new Texture2D[reqLength];
                    for (int i = 0; i < reqLength; i++)
                    {
                        textures[i] = DownloadHandlerTexture.GetContent(reqs[i]);
                    }
                    downloadedCallback?.Invoke(textures);
                }));
            }
            public static Coroutine DownloadAudioAsync(string url, AudioType audioType, Action<float> progress, Action<AudioClip> downloadedCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequestMultimedia.GetAudioClip(url, audioType), progress, (UnityWebRequest req) =>
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                    downloadedCallback?.Invoke(clip);
                }));
            }
            public static Coroutine DownloadAudiosAsync(IDictionary<string, AudioType> urlDict, Action<float> overallProgress, Action<float> progress, Action<AudioClip[]> downloadedCallback)
            {
                var length = urlDict.Count;
                var requests = new List<UnityWebRequest>();
                foreach (var url in urlDict)
                {
                    requests.Add(UnityWebRequestMultimedia.GetAudioClip(url.Key, url.Value));
                }
                return StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Length;
                    var audios = new AudioClip[reqLength];
                    for (int i = 0; i < reqLength; i++)
                    {
                        audios[i] = DownloadHandlerAudioClip.GetContent(reqs[i]);
                    }
                    downloadedCallback?.Invoke(audios);
                }));
            }
            public static Coroutine DownloadAssetBundleAsync(string url, Action<float> progress, Action<AssetBundle> downloadedCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequestAssetBundle.GetAssetBundle(url), progress, (UnityWebRequest req) =>
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
                    if (bundle)
                    {
                        downloadedCallback?.Invoke(bundle);
                    }
                }));
            }
            public static Coroutine DownloadAssetBundlesAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<AssetBundle[]> downloadedCallback)
            {
                var length = urls.Length;
                var requests = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    requests.Add(UnityWebRequestAssetBundle.GetAssetBundle(urls[i]));
                }
                return StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Length;
                    var assetbundles = new AssetBundle[reqLength];
                    for (int i = 0; i < reqLength; i++)
                    {
                        assetbundles[i] = DownloadHandlerAssetBundle.GetContent(reqs[i]);
                    }
                    downloadedCallback?.Invoke(assetbundles);
                }));
            }
            public static Coroutine DownloadAssetBundleBytesAsync(string url, Action<float> progress, Action<byte[]> downloadedCallback)
            {
                return StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
                {
                    var bundleBytes = req.downloadHandler.data;
                    if (bundleBytes != null)
                    {
                        downloadedCallback?.Invoke(bundleBytes);
                    }
                }));
            }
            public static Coroutine DownloadAssetBundlesBytesAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<IList<byte[]>> downloadedCallback)
            {
                var length = urls.Length;
                var requests = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    requests.Add(UnityWebRequest.Get(urls[i]));
                }
                return StartCoroutine(EnumBytesUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
                {
                    var reqLength = reqs.Count;
                    var bundleByteList = new List<byte[]>();
                    for (int i = 0; i < reqLength; i++)
                    {
                        bundleByteList.Add(reqs[i]);
                    }
                    downloadedCallback?.Invoke(bundleByteList);
                }));
            }
            static IEnumerator EnumUnityWebRequest(UnityWebRequest unityWebRequest, Action<float> progress, Action<UnityWebRequest> downloadedCallback)
            {
                using (UnityWebRequest request = unityWebRequest)
                {
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        progress?.Invoke(request.downloadProgress);
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
                        //throw new ArgumentException($"UnityWebRequest：{request.url } : {request.error } ！");
                    }
                }
            }
            static IEnumerator EnumUnityWebRequests(UnityWebRequest[] unityWebRequests, Action<float> overallProgress, Action<float> progress, Action<UnityWebRequest[]> downloadedCallback)
            {
                var length = unityWebRequests.Length;
                var count = length - 1;
                var requestList = new List<UnityWebRequest>();
                for (int i = 0; i < length; i++)
                {
                    overallProgress?.Invoke((float)i / (float)count);
                    yield return EnumUnityWebRequest(unityWebRequests[i], progress, (request) => { requestList.Add(request); });
                }
                downloadedCallback.Invoke(requestList.ToArray());
            }
            static IEnumerator EnumBytesUnityWebRequests(UnityWebRequest[] unityWebRequests, Action<float> overallProgress, Action<float> progress, Action<IList<byte[]>> downloadedCallback)
            {
                var length = unityWebRequests.Length;
                var count = length - 1;
                var requestBytesList = new List<byte[]>();
                for (int i = 0; i < length; i++)
                {
                    overallProgress?.Invoke((float)i / (float)count);
                    yield return EnumUnityWebRequest(unityWebRequests[i], progress, (request) => { requestBytesList.Add(request.downloadHandler.data); });
                }
                downloadedCallback.Invoke(requestBytesList);
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
            public static Coroutine StartCoroutine(IEnumerator routine)
            {
                return coroutineProvider.StartCoroutine(routine);
            }
            public static Coroutine StartCoroutine(Action handler)
            {
                return coroutineProvider.StartCoroutine(handler);
            }
            public static Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
            {
                return coroutineProvider.PredicateCoroutine(handler, callBack);
            }
            public static Coroutine StartCoroutine(Coroutine routine, Action callBack)
            {
                return coroutineProvider.StartCoroutine(routine, callBack);
            }
            public static Coroutine StartCoroutine(Action handler, Action callback)
            {
                return coroutineProvider.StartCoroutine(handler, callback);
            }
            public static void StopCoroutine(IEnumerator routine)
            {
                coroutineProvider.StopCoroutine(routine);
            }
            public static void StopCoroutine(Coroutine routine)
            {
                coroutineProvider.StopCoroutine(routine);
            }
            [RuntimeInitializeOnLoadMethod]
            static void InitQuarkCoroutine()
            {
                var go = new GameObject("QuarkCoroutinePorvider");
                coroutineProvider = go.AddComponent<CoroutineProvider>();
            }
            #endregion
        }
    }
}
