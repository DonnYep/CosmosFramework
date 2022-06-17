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
        /// assetName===resourceObjectWarpper
        /// </summary>
        readonly Dictionary<string, ResourceObjectWarpper> resourceObjectDict;
        /// <summary>
        /// bundleName===resourceBundleWarpper
        /// </summary>
        readonly Dictionary<string, ResourceBundleWarpper> resourceBundleDict;

        /// <summary>
        /// 被加载的场景字典；
        /// SceneName===Scene
        /// </summary>
        readonly Dictionary<string, UnityEngine.SceneManagement.Scene> loadedSceneDict;

        bool isProcessing;
        ResourceDataset resourceDataset;
        /// <summary>
        /// 单线下载等待
        /// </summary>
        private WaitUntil loadWait;
        public AssetDatabaseLoader(ResourceDataset resourceDataset)
        {
            loadWait = new WaitUntil(() => { return !isProcessing; });
            this.resourceDataset = resourceDataset;
            resourceBundleDict = new Dictionary<string, ResourceBundleWarpper>();
            resourceObjectDict = new Dictionary<string, ResourceObjectWarpper>();
            loadedSceneDict = new Dictionary<string, UnityEngine.SceneManagement.Scene>();
            InitData();
        }
        ///<inheritdoc/> 
        public bool IsProcessing { get { return isProcessing; } }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync(string assetName, Type type, Action<UnityEngine.Object> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetWithSubAssetsAsync(string assetName, Type type, Action<UnityEngine.Object[]> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, callback, progress));
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
        public Coroutine UnloadAllSceneAsync(Action<float> progress, Action callback)
        {
            return Utility.Unity.StartCoroutine(EnumUnloadAllSceneAsync(progress, callback));
        }
        ///<inheritdoc/> 
        public void UnloadAsset(string assetName)
        {
            DecrementReferenceCount(assetName);
        }
        ///<inheritdoc/> 
        public void ReleaseAsset(string assetName)
        {
            if (resourceObjectDict.TryGetValue(assetName, out var objectWarpper))
                ReleaseObject(objectWarpper);
        }
        ///<inheritdoc/> 
        public void ReleaseAllAsset(bool unloadAllLoadedObjects = false)
        {
            foreach (var objectWarpper in resourceObjectDict.Values)
            {
                ReleaseObject(objectWarpper);
            }
        }
        public void Dispose()
        {
            resourceObjectDict.Clear();
            resourceBundleDict.Clear();
        }
        void InitData()
        {
            var bundles = resourceDataset.ResourceBundleList;
            foreach (var bundle in bundles)
            {
                resourceBundleDict.TryAdd(bundle.BundleName, new ResourceBundleWarpper(bundle));
                var objList = bundle.ResourceObjectList;
                var length = objList.Count;
                for (int i = 0; i < length; i++)
                {
                    var obj = objList[i];
                    resourceObjectDict.TryAdd(obj.AssetName, new ResourceObjectWarpper(obj));
                }
            }
        }
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (isProcessing)
            {
                yield return loadWait;
            }
            isProcessing = true;
            if (loadedSceneDict.TryGetValue(info.SceneName, out var scene))
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            LoadSceneMode loadSceneMode = info.Additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
#if UNITY_EDITOR
            var ao = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(info.SceneName, new LoadSceneParameters(loadSceneMode));
#else
            var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(info.SceneName, loadSceneMode);
#endif
            ao.allowSceneActivation = false;
            var hasProviderProgress = progressProvider != null;
            while (!ao.isDone)
            {
                if (hasProviderProgress)
                {
                    var providerProgress = progressProvider();
                    var sum = providerProgress + ao.progress;
                    progress?.Invoke(sum / 2);
                    if (sum >= 1.9)
                    {
                        break;
                    }
                }
                else
                {
                    progress?.Invoke(ao.progress);
                    if (ao.progress >= 0.9f)
                    {
                        break;
                    }
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
        IEnumerator EnumLoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress)
where T : UnityEngine.Object
        {
            T[] assets = default;
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            string assetPath = string.Empty;
            var hasObjWarpper = resourceObjectDict.TryGetValue(assetName, out var objectWarpper);
            if (!hasObjWarpper)
            {
                progress?.Invoke(1);
                callback?.Invoke(assets);
                yield break;
            }
            else
            {
                assetPath = objectWarpper.ResourceObject.AssetPath;
            }
#if UNITY_EDITOR
            var assetObj = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
            var length = assetObj.Length;
            assets = new T[length];
            for (int i = 0; i < length; i++)
            {
                assets[i] = assetObj[i] as T;
            }
#endif
            yield return null;
            progress?.Invoke(1);
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadAsssetAsync<T>(string assetName, Action<T> callback, Action<float> progress) where T : UnityEngine.Object
        {
            T asset = default;
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset path is invalid!");
            string assetPath = string.Empty;
            var hasObjWarpper = resourceObjectDict.TryGetValue(assetName, out var objectWarpper);
            if (!hasObjWarpper)
            {
                progress.Invoke(1);
                callback?.Invoke(asset);
                yield break;
            }
            else
            {
                assetPath = objectWarpper.ResourceObject.AssetPath;
            }
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(assetPath))
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetName);
#endif
            yield return null;
            progress.Invoke(1);
            callback?.Invoke(asset);
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

        IEnumerator EnumUnloadAllSceneAsync(Action<float> progress, Action callback)
        {
            var sceneCount = loadedSceneDict.Count;
            //单位场景的百分比比率
            var unitResRatio = 100f / sceneCount;
            int currentSceneIndex = 0;
            float overallProgress = 0;
            foreach (var scene in loadedSceneDict)
            {
                var overallIndexPercent = 100 * ((float)currentSceneIndex / sceneCount);
                currentSceneIndex++;
                var sceneName = scene.Key;
                var hasObject = resourceObjectDict.TryGetValue(sceneName, out var objectWapper);
                if (!hasObject)
                {
                    overallProgress = overallIndexPercent + (unitResRatio * 1);
                    progress?.Invoke(overallProgress / 100);
                }
                else
                {
                    var ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene.Value);
                    while (!ao.isDone)
                    {
                        overallProgress = overallIndexPercent + (unitResRatio * ao.progress);
                        progress?.Invoke(overallProgress / 100);
                        yield return null;
                    }
                    overallProgress = overallIndexPercent + (unitResRatio * 1);
                    progress?.Invoke(overallProgress / 100);
                    ReleaseObject(objectWapper);
                }
            }
            loadedSceneDict.Clear();
            progress?.Invoke(1);
            callback?.Invoke();
        }
        void ReleaseObject(ResourceObjectWarpper objectWarpper)
        {
            var count = objectWarpper.ReferenceCount;
            objectWarpper.ReferenceCount = 0;
            if (resourceBundleDict.TryGetValue(objectWarpper.ResourceObject.AssetName, out var bundleWarpper))
            {
                bundleWarpper.ReferenceCount -= count;
                if (bundleWarpper.ReferenceCount == 0)
                {
      
                }
            }
        }

    }
}
