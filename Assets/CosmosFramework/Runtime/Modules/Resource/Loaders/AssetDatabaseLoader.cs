using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cosmos.Resource
{
    public class AssetDatabaseLoader : IResourceLoadHelper
    {
        /// <summary>
        /// name===resourceObject
        /// </summary>
        Dictionary<string, ResourceObject> resourceObjectDict;
        /// <summary>
        /// abName===resourceBundle
        /// </summary>
        Dictionary<string, ResourceBundle> resourceBundleDict;
        bool isProcessing;
        /// <summary>
        /// 单线下载等待
        /// </summary>
        private WaitUntil loadWait;
        public AssetDatabaseLoader()
        {
            resourceBundleDict = new Dictionary<string, ResourceBundle>();
            resourceObjectDict = new Dictionary<string, ResourceObject>();

            loadWait = new WaitUntil(() => { return !isProcessing; });
        }
        ///<inheritdoc/> 
        public bool IsProcessing { get { return isProcessing; } }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return Utility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, callback, progress));
#else
            callback?.Invoke(null);
            progress?.Invoke(1);
            return null;
#endif
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync(string assetName,Type type, Action<UnityEngine.Object> callback, Action<float> progress = null)
        {
#if UNITY_EDITOR
            return Utility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, callback, progress));
#else
            callback?.Invoke(null);
            progress?.Invoke(1);
            return null;
#endif
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, callback));
#else
            callback?.Invoke(null);
            progress?.Invoke(1);
            return null;
#endif
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetWithSubAssetsAsync(string assetName,Type type, Action<UnityEngine.Object[]> callback, Action<float> progress = null) 
        {
#if UNITY_EDITOR
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, callback));
#else
            callback?.Invoke(null);
            progress?.Invoke(1);
            return null;
#endif
        }
        ///<inheritdoc/> 
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            return Utility.Unity.StartCoroutine(EnumLoadSceneAsync(info, progressProvider, progress, condition, callback));
        }
        ///<inheritdoc/> 
        public Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            return Utility.Unity.StartCoroutine(EnumUnloadSceneAsync(info, progress, condition, callback));
        }
        ///<inheritdoc/> 
        public void UnloadAllAsset(bool unloadAllLoadedObjects = false)
        {
#if UNITY_EDITOR

#else

#endif
        }
        ///<inheritdoc/> 
        public void UnloadAsset(string assetName)
        {
#if UNITY_EDITOR
#else

#endif
        }
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (isProcessing)
            {
                yield return loadWait;
            }
            isProcessing = true;
            LoadSceneMode loadSceneMode = info.Additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
#if UNITY_EDITOR
            var ao = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(info.SceneName, new LoadSceneParameters(loadSceneMode));
#else
            var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(info.SceneName, loadSceneMode);
#endif
            ao.allowSceneActivation = false;
            while (!ao.isDone)
            {
                progress?.Invoke(ao.progress);
                if (ao.progress >= 0.9f)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            progress?.Invoke(1);
            if (condition != null)
                yield return new WaitUntil(condition);
            ao.allowSceneActivation = true;
            callback?.Invoke();
            isProcessing = false;
        }
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        IEnumerator EnumUnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            if (isProcessing)
            {
                yield return loadWait;
            }
            isProcessing = true;
            var ao = SceneManager.UnloadSceneAsync(info.SceneName);
            while (!ao.isDone)
            {
                progress?.Invoke(ao.progress);
                if (ao.progress >= 0.9f)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            if (condition != null)
                yield return new WaitUntil(condition);
            progress?.Invoke(1);
            callback?.Invoke();
            isProcessing = false;
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback)
where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            var assetObj = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetName);
            var length = assetObj.Length;
            T[] assets = new T[length];
            for (int i = 0; i < length; i++)
            {
                assets[i] = assetObj[i] as T;
            }
            yield return null;
            callback?.Invoke(assets);
#else
            yield return null;
#endif
        }
        IEnumerator EnumLoadAsssetAsync<T>(string assetName, Action<T> callback, Action<float> progress) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset path is invalid!");
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetName);
            yield return null;
            progress.Invoke(1);
            callback?.Invoke(asset);
#else
            yield return null;
#endif
        }

        /// <summary>
        /// 增加一个引用计数
        /// </summary>
        void IncrementReferenceCount(string assetName)
        {

        }
        /// <summary>
        /// 减少一个引用计数
        /// </summary>
        void DecrementReferenceCount(string assetName)
        {

        }
    }
}
