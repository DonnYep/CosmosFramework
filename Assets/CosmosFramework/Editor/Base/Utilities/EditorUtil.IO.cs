using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine;

namespace Cosmos.CosmosEditor
{
    public static partial class EditorUtil
    {
        public static class IO
        {
            /// <summary>
            /// 遍历文件夹下的文件；
            /// 传入的文件夹名参考：Assets/Game
            /// </summary>
            /// <param name="folder">文件夹名</param>
            /// <param name="handler">处理方法</param>
            public static void TraverseFolderFile(string folder, Action<UnityEngine.Object> handler)
            {
                if (string.IsNullOrEmpty(folder))
                    throw new ArgumentNullException("Folder Name is invalid !");
                if (handler == null)
                    throw new ArgumentNullException("Handler is invalid !");
                if (AssetDatabase.IsValidFolder(folder))
                {
                    var assets = GetAllAssets<UnityEngine.Object>(folder);
                    if (assets != null)
                    {
                        var length = assets.Length;
                        for (int i = 0; i < length; i++)
                        {
                            handler.Invoke(assets[i]);
                        }
                    }
                    var subFolder = AssetDatabase.GetSubFolders(folder);
                    if (subFolder != null)
                    {
                        foreach (var subF in subFolder)
                        {
                            TraverseFolderFile(subF, handler);
                        }
                    }
                }
            }
            public static void TraverseAllFolderFile(Action<UnityEngine.Object> handler)
            {
                if (handler == null)
                    throw new ArgumentNullException("Handler is invalid !");
                var assets = GetAllAssets<UnityEngine.Object>("Assets");
                if (assets != null)
                {
                    var length = assets.Length;
                    for (int i = 0; i < length; i++)
                    {
                        handler.Invoke(assets[i]);
                    }
                }
                var subFolder = AssetDatabase.GetSubFolders("Assets");
                if (subFolder != null)
                {
                    foreach (var subF in subFolder)
                    {
                        TraverseFolderFile(subF, handler);
                    }
                }
            }
            /// <summary>
            /// 获取所有Asset目录下，除文件夹、CS脚本以外的资源路径；
            /// </summary>
            /// <returns></returns>
            public static string[] GetAllBundleableFilePath()
            {
                var paths = AssetDatabase.GetAllAssetPaths();
                List<string> pathList = new List<string>();
                foreach (var path in paths)
                {
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        var file = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                        if (!(file is MonoScript))
                        {
                            pathList.Add(path);
                        }
                    }
                }
                return pathList.ToArray();
            }


          
            public static EditorCoroutine DownloadAssetBundleAsync(string url, Action<float> progress, Action<AssetBundle> downloadedCallback)
            {
                return EditorUtil.Coroutine.StartCoroutine(EnumUnityWebRequest(UnityWebRequestAssetBundle.GetAssetBundle(url), progress, (UnityWebRequest req) =>
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
                    if (bundle)
                    {
                        downloadedCallback?.Invoke(bundle);
                    }
                }));
            }
            public static EditorCoroutine DownloadAssetBundlesAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<AssetBundle[]> downloadedCallback)
            {
                return EditorUtil.Coroutine.StartCoroutine(EnumUnityWebRequests(urls, overallProgress, progress, downloadedCallback));
            }
            public static EditorCoroutine DownloadAssetBundleBytesAsync(string url, Action<float> progress, Action<byte[]> downloadedCallback)
            {
                return EditorUtil.Coroutine.StartCoroutine(EnumUnityWebRequest(UnityWebRequest.Get(url), progress, (UnityWebRequest req) =>
                {
                    var bundleBytes = req.downloadHandler.data;
                    if (bundleBytes != null)
                    {
                        downloadedCallback?.Invoke(bundleBytes);
                    }
                }));
            }
            public static EditorCoroutine DownloadAssetBundlesBytesAsync(string[] urls, Action<float> overallProgress, Action<float> progress, Action<IList<byte[]>> downloadedCallback)
            {
                return EditorUtil.Coroutine.StartCoroutine(EnumBytesUnityWebRequests(urls, overallProgress, progress, downloadedCallback));
            }
            static IEnumerator EnumUnityWebRequests(string[] urls, Action<float> overallProgress, Action<float> progress, Action<AssetBundle[]> downloadedCallback)
            {
                var length = urls.Length;
                var count = length - 1;
                var assetBundleList = new List<AssetBundle>();
                for (int i = 0; i < length; i++)
                {
                    overallProgress?.Invoke((float)i / (float)count);
                    yield return DownloadAssetBundleAsync(urls[i], progress, (request) => { assetBundleList.Add(request); });
                }
                downloadedCallback.Invoke(assetBundleList.ToArray());
            }
            static IEnumerator EnumBytesUnityWebRequests(string[] urls, Action<float> overallProgress, Action<float> progress, Action<IList<byte[]>> downloadedCallback)
            {
                var length = urls.Length;
                var count = length - 1;
                var assetBundleList = new List<byte[]>();
                for (int i = 0; i < length; i++)
                {
                    overallProgress?.Invoke((float)i / (float)count);
                    yield return DownloadAssetBundleBytesAsync(urls[i], progress, (request) => { assetBundleList.Add(request); });
                }
                downloadedCallback.Invoke(assetBundleList);
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
                        throw new ArgumentException($"UnityWebRequest：{request.url } : {request.error } ！");
                    }
                }
            }
        }
    }
}
