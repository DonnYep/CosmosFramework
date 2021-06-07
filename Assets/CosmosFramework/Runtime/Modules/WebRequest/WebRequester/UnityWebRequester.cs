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
    /// Unity封装的WebRequest；
    /// </summary>
    public class UnityWebRequester:IWebRequester
    {
        #region UnityWebRequest
        public static Coroutine DownloadTextAsync(string url, Action<float> progress, Action<string> downloadedCallback)
        {
            return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
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
            return Utility.Unity.StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
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
            return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequestTexture.GetTexture(url), progress, (UnityWebRequest req) =>
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(req);
                //var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
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
            return Utility.Unity.StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
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
            return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequestMultimedia.GetAudioClip(url, audioType), progress, (UnityWebRequest req) =>
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
            return Utility.Unity.StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
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
            return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequestAssetBundle.GetAssetBundle(url), progress, (UnityWebRequest req) =>
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
            return Utility.Unity.StartCoroutine(EnumUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
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
            return Utility.Unity.StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
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
            return Utility.Unity.StartCoroutine(EnumBytesUnityWebRequests(requests.ToArray(), overallProgress, progress, (reqs) =>
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
    }
}
