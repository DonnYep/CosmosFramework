﻿using Quark.Asset;
using System;
using UnityEngine;
namespace Quark
{
    public sealed class QuarkResources
    {
        public static ulong QuarkEncryptionOffset
        {
            get { return QuarkDataProxy.QuarkEncryptionOffset; }
            set { QuarkDataProxy.QuarkEncryptionOffset = value; }
        }
        public static byte[] QuarkAESEncryptionKey
        {
            get { return QuarkDataProxy.QuarkAESEncryptionKey; }
            set { QuarkDataProxy.QuarkAESEncryptionKey = value; }
        }
        public static QuarkAssetLoadMode QuarkAssetLoadMode
        {
            get { return QuarkEngine.Instance.QuarkAssetLoadMode; }
            set { QuarkEngine.Instance.QuarkAssetLoadMode = value; }
        }
        /// <summary>
        /// 当检测到最新的；
        /// </summary>
        public static event Action<long> OnDetectedSuccess
        {
            add { QuarkEngine.Instance.OnDetectedSuccess += value; }
            remove { QuarkEngine.Instance.OnDetectedSuccess -= value; }
        }
        /// <summary>
        /// 当检测失败；
        /// </summary>
        public static event Action<string> OnDetectedFailure
        {
            add { QuarkEngine.Instance.OnDetectedFailure += value; }
            remove { QuarkEngine.Instance.OnDetectedFailure -= value; }
        }
        /// <summary>
        /// URL---DownloadPath
        /// </summary>
        public static event Action<string, string> OnDownloadStart
        {
            add { QuarkEngine.Instance.OnDownloadStart += value; }
            remove { QuarkEngine.Instance.OnDownloadStart -= value; }
        }
        /// <summary>
        /// URL---DownloadPath
        /// </summary>
        public static event Action<string, string> OnDownloadSuccess
        {
            add { QuarkEngine.Instance.OnDownloadSuccess += value; }
            remove { QuarkEngine.Instance.OnDownloadSuccess -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---ErrorMessage
        /// </summary>
        public static event Action<string, string, string> OnDownloadFailure
        {
            add { QuarkEngine.Instance.OnDownloadFailure += value; }
            remove { QuarkEngine.Instance.OnDownloadFailure -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---OverallProgress(0~100%)---IndividualProgress(0~100%)
        /// </summary>
        public static event Action<string, string, float, float> OnDownloadOverall
        {
            add { QuarkEngine.Instance.OnDownloadOverall += value; }
            remove { QuarkEngine.Instance.OnDownloadOverall -= value; }
        }
        /// <summary>
        /// SuccessURIs---FailureURIs---TimeSpan
        /// </summary>
        public static event Action<string[], string[], TimeSpan> OnDownloadFinish
        {
            add { QuarkEngine.Instance.OnDownloadFinish += value; }
            remove { QuarkEngine.Instance.OnDownloadFinish -= value; }
        }

        /// <summary>
        /// 启动下载；
        /// </summary>
        public static void LaunchDownload()
        {
            QuarkEngine.Instance.LaunchDownload();
        }
        public static void StopDownload()
        {
            QuarkEngine.Instance.StopDownload();
        }
        public static T LoadAsset<T>(string assetName, string assetExtension = null)
where T : UnityEngine.Object
        {
            return QuarkEngine.Instance.LoadAsset<T>(assetName, assetExtension);
        }
        public static Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback)
where T : UnityEngine.Object
        {
            return QuarkEngine.Instance.LoadAssetAsync<T>(assetName, string.Empty, callback);
        }
        public static Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback)
where T : UnityEngine.Object
        {
            return QuarkEngine.Instance.LoadAssetAsync<T>(assetName, assetExtension, callback);
        }
        public static GameObject LoadPrefab(string assetName, bool instantiate = false)
        {
            return QuarkEngine.Instance.LoadPrefab(assetName, string.Empty, instantiate);
        }
        public static GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false)
        {
            return QuarkEngine.Instance.LoadPrefab(assetName, assetExtension, instantiate);
        }
        public static T[] LoadAssetWithSubAssets<T>(string assetName) where T : UnityEngine.Object
        {
            return QuarkEngine.Instance.LoadAssetWithSubAssets<T>(assetName, string.Empty);
        }
        public static T[] LoadAssetWithSubAssets<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
            return QuarkEngine.Instance.LoadAssetWithSubAssets<T>(assetName, assetExtension);
        }
        public static Coroutine LoadPrefabAsync(string assetName, Action<GameObject> callback, bool instantiate = false)
        {
            return QuarkEngine.Instance.LoadPrefabAsync(assetName, string.Empty, callback, instantiate);
        }
        public static Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false)
        {
            return QuarkEngine.Instance.LoadPrefabAsync(assetName, assetExtension, callback, instantiate);
        }
        public static Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback) where T : UnityEngine.Object
        {
            return QuarkEngine.Instance.LoadAssetWithSubAssetsAsync<T>(assetName, string.Empty, callback);
        }
        public static Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback) where T : UnityEngine.Object
        {
            return QuarkEngine.Instance.LoadAssetWithSubAssetsAsync<T>(assetName, assetExtension, callback);
        }
        public static Coroutine LoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive = false)
        {
            return QuarkEngine.Instance.LoadSceneAsync(sceneName, null, progress, null, callback, additive);
        }
        public static Coroutine LoadSceneAsync(string sceneName, Action<float> progress, Func<bool> condition, Action callback, bool additive = false)
        {
            return QuarkEngine.Instance.LoadSceneAsync(sceneName, null, progress, condition, callback, additive);
        }
        public static Coroutine LoadSceneAsync(string sceneName, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback, bool additive = false)
        {
            return QuarkEngine.Instance.LoadSceneAsync(sceneName, progressProvider, progress, condition, callback, additive);
        }
        public static void UnloadAsset(string assetName)
        {
            QuarkEngine.Instance.UnloadAsset(assetName, string.Empty);
        }
        public static void UnloadAsset(string assetName, string assetExtension)
        {
            QuarkEngine.Instance.UnloadAsset(assetName, assetExtension);
        }
        public static void UnloadAllAssetBundle(bool unloadAllLoadedObjects = false)
        {
            QuarkEngine.Instance.UnloadAllAssetBundle(unloadAllLoadedObjects);
        }
        public static void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            QuarkEngine.Instance.UnloadAssetBundle(assetBundleName, unloadAllLoadedObjects);
        }
        public static Coroutine UnloadSceneAsync(string sceneName, Action<float> progress, Action callback)
        {
            return QuarkEngine.Instance.UnloadSceneAsync(sceneName, progress, callback);
        }
        public static Coroutine UnloadAllSceneAsync(Action<float> progress, Action callback)
        {
            return QuarkEngine.Instance.UnloadAllSceneAsync(progress, callback);
        }
        public static bool GetInfo<T>(string assetName, string assetExtension, out QuarkAssetObjectInfo info) where T : UnityEngine.Object
        {
            return QuarkEngine.Instance.GetInfo<T>(assetName, assetExtension, out info);
        }
        public static bool GetInfo<T>(string assetName, out QuarkAssetObjectInfo info) where T : UnityEngine.Object
        {
            return QuarkEngine.Instance.GetInfo<T>(assetName, string.Empty, out info);
        }
        public static bool GetInfo(string assetName, string assetExtension, out QuarkAssetObjectInfo info)
        {
            return QuarkEngine.Instance.GetInfo(assetName, assetExtension, out info);
        }
        public static bool GetInfo(string assetName, out QuarkAssetObjectInfo info)
        {
            return QuarkEngine.Instance.GetInfo(assetName, string.Empty, out info);
        }
        public static QuarkAssetObjectInfo[] GetAllLoadedInfos()
        {
            return QuarkEngine.Instance.GetAllLoadedInfos();
        }
    }
}
