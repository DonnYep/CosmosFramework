using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    /// <summary>
    /// AB加载方案
    /// </summary>
    public class AssetBundleLoader : IResourceLoadHelper
    {
        bool isProcessing;
        ResourceManifest resourceManifest;
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
        public AssetBundleLoader(ResourceManifest resourceManifest)
        {
            this.resourceManifest = resourceManifest;
            resourceBundleDict = new Dictionary<string, ResourceBundleWarpper>();
            resourceObjectDict = new Dictionary<string, ResourceObjectWarpper>();
            loadedSceneDict = new Dictionary<string, UnityEngine.SceneManagement.Scene>();
            InitData();
        }
        ///<inheritdoc/> 
        public bool IsProcessing { get { return isProcessing; } }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, type, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadMainAndSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync<T>(assetName, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetWithSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, type, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAllAssetAsync(string assetBundleName, Action<Object[]> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadAllAssetAsync(assetBundleName, progress, callback));
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
        public void ReleaseAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (resourceBundleDict.TryGetValue(assetBundleName, out var bundleWarpper))
            {
                var objs = bundleWarpper.ResourceBundle.ResourceObjectList;
                foreach (var obj in objs)
                {
                    ReleaseAsset(obj.AssetName);
                }
            }
        }
        ///<inheritdoc/> 
        public void ReleaseAllAsset(bool unloadAllLoadedObjects = false)
        {
            foreach (var objectWarpper in resourceObjectDict.Values)
            {
                ReleaseObject(objectWarpper);
            }
        }
        ///<inheritdoc/> 
        public void Dispose()
        {
            resourceObjectDict.Clear();
            resourceBundleDict.Clear();
        }
        void InitData()
        {
            var bundleBuildInfoDict = resourceManifest.ResourceBundleBuildInfoDict;
            foreach (var bundleBuildInfo in bundleBuildInfoDict.Values)
            {
                resourceBundleDict.TryAdd(bundleBuildInfo.ResourceBundle.BundleName, new ResourceBundleWarpper(bundleBuildInfo.ResourceBundle));
                var objList = bundleBuildInfo.ResourceBundle.ResourceObjectList;
                var length = objList.Count;
                for (int i = 0; i < length; i++)
                {
                    var obj = objList[i];
                    resourceObjectDict.TryAdd(obj.AssetName, new ResourceObjectWarpper(obj));
                }
            }
        }
        IEnumerator EnumLoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null)
            where T : Object
        {
            T asset = null;
            var bundleName = string.Empty;
            var hasObject = resourceObjectDict.TryGetValue(assetName, out var objectWarpper);
            if (hasObject)
                bundleName = objectWarpper.ResourceObject.BundleName;
            else
            {
                callback?.Invoke(asset);
                progress?.Invoke(1);
                yield break;
            }
            if (string.IsNullOrEmpty(bundleName))
                yield break;
            yield return EnumLoadDependenciesAssetBundleAsync(bundleName);
            var hasBundle = resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper);
            if (hasBundle)
            {
                if (bundleWarpper.AssetBundle != null)
                {
                    asset = bundleWarpper.AssetBundle.LoadAsset<T>(assetName);
                    bundleWarpper.ReferenceCount++;
                    objectWarpper.ReferenceCount++;
                }
            }
            callback?.Invoke(asset);
            progress?.Invoke(1);
        }
        IEnumerator EnumLoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null)
        {
            Object asset = null;
            var bundleName = string.Empty;
            var hasObject = resourceObjectDict.TryGetValue(assetName, out var objectWarpper);
            if (hasObject)
                bundleName = objectWarpper.ResourceObject.BundleName;
            else
            {
                callback?.Invoke(asset);
                progress?.Invoke(1);
                yield break;
            }
            if (string.IsNullOrEmpty(bundleName))
                yield break;
            yield return EnumLoadDependenciesAssetBundleAsync(bundleName);
            var hasBundle = resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper);
            if (hasBundle)
            {
                if (bundleWarpper.AssetBundle != null)
                {
                    asset = bundleWarpper.AssetBundle.LoadAsset(assetName, type);
                    bundleWarpper.ReferenceCount++;
                    objectWarpper.ReferenceCount++;
                }
            }
            callback?.Invoke(asset);
            progress?.Invoke(1);
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress)
    where T : Object
        {
            T[] assets = null;
            string bundleName = string.Empty;
            var hasObject = resourceObjectDict.TryGetValue(assetName, out var objectWarpper);
            if (hasObject)
            {
                bundleName = objectWarpper.ResourceObject.BundleName;
            }
            else
            {
                callback?.Invoke(assets);
                progress?.Invoke(1);
                yield break;
            }
            if (string.IsNullOrEmpty(bundleName))
                yield break;
            yield return EnumLoadDependenciesAssetBundleAsync(bundleName);
            var hasBundle = resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper);
            if (hasBundle)
            {
                var bundle = bundleWarpper.AssetBundle;
                if (bundle != null)
                {
                    assets = bundle.LoadAssetWithSubAssets<T>(assetName);
                    bundleWarpper.ReferenceCount++;
                    objectWarpper.ReferenceCount++;
                }
            }
            callback?.Invoke(assets);
            progress?.Invoke(1);
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress)
        {
            Object[] assets = null;
            string bundleName = string.Empty;
            var hasObject = resourceObjectDict.TryGetValue(assetName, out var objectWarpper);
            if (hasObject)
            {
                bundleName = objectWarpper.ResourceObject.BundleName;
            }
            else
            {
                callback?.Invoke(assets);
                progress?.Invoke(1);
                yield break;
            }
            if (string.IsNullOrEmpty(bundleName))
                yield break;
            yield return EnumLoadDependenciesAssetBundleAsync(bundleName);
            var hasBundle = resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper);
            if (hasBundle)
            {
                var bundle = bundleWarpper.AssetBundle;
                if (bundle != null)
                {
                    assets = bundle.LoadAssetWithSubAssets(assetName, type);
                    bundleWarpper.ReferenceCount++;
                    objectWarpper.ReferenceCount++;

                }
            }
            callback?.Invoke(assets);
            progress?.Invoke(1);
        }
        IEnumerator EnumLoadDependenciesAssetBundleAsync(string bundleName)
        {
            var hasBundle = resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper);
            if (!hasBundle)
            {
                //若bundle信息为空，则终止；
                yield break;
            }
            if (bundleWarpper.AssetBundle == null)
            {
                var abPath = Path.Combine(ResourceDataProxy.Instance.BundlePath, bundleName);
                var abReq = AssetBundle.LoadFromFileAsync(abPath, 0, ResourceDataProxy.Instance.EncryptionOffset);
                yield return abReq;
                var bundle = abReq.assetBundle;
                if (bundle != null)
                    bundleWarpper.AssetBundle = bundle;
                else
                    yield break;
            }
            var dependList = bundleWarpper.ResourceBundle.DependList;
            var length = dependList.Count;
            for (int i = 0; i < length; i++)
            {
                var dependentABName = dependList[i];
                var hasDependentBundle = resourceBundleDict.TryGetValue(bundleName, out var dependentBundleWarpper);
                if (hasDependentBundle)
                {
                    if (dependentBundleWarpper.AssetBundle == null)
                    {
                        var abPath = Path.Combine(ResourceDataProxy.Instance.BundlePath, dependentABName);
                        var abReq = AssetBundle.LoadFromFileAsync(abPath, 0, ResourceDataProxy.Instance.EncryptionOffset);
                        yield return abReq;
                        var bundle = abReq.assetBundle;
                        if (bundle != null)
                            dependentBundleWarpper.AssetBundle = bundle;
                    }
                }
            }
        }
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (loadedSceneDict.ContainsKey(info.SceneName))
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            var hasObjectWapper = resourceObjectDict.TryGetValue(info.SceneName, out var objectWarpper);
            if (!hasObjectWapper)
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(objectWarpper.ResourceObject.BundleName);
            LoadSceneMode loadSceneMode = info.Additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var operation = SceneManager.LoadSceneAsync(objectWarpper.ResourceObject.AssetPath, loadSceneMode);
            operation.allowSceneActivation = false;
            var hasProviderProgress = progressProvider != null;
            while (!operation.isDone)
            {
                if (hasProviderProgress)
                {
                    var providerProgress = progressProvider();
                    var sum = providerProgress + operation.progress;
                    if (sum >= 1.9)
                    {
                        break;
                    }
                    else
                    {
                        progress?.Invoke(sum / 2);
                    }
                }
                else
                {
                    progress?.Invoke(operation.progress);
                    if (operation.progress >= 0.9f)
                    {
                        break;
                    }
                }
                yield return null;
            }
            progress?.Invoke(1);
            if (condition != null)
                yield return new WaitUntil(condition);
            var scene = SceneManager.GetSceneByPath(objectWarpper.ResourceObject.AssetPath);
            loadedSceneDict.Add(info.SceneName, scene);
            operation.allowSceneActivation = true;
            callback?.Invoke();
        }
        IEnumerator EnumUnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            var sceneName = info.SceneName;
            if (string.IsNullOrEmpty(sceneName))
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            if (!loadedSceneDict.TryGetValue(sceneName, out var scene))
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            var hasObject = resourceObjectDict.ContainsKey(sceneName);
            if (!hasObject)
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            var ao = SceneManager.UnloadSceneAsync(scene);
            while (!ao.isDone)
            {
                progress?.Invoke(ao.progress);
                yield return null;
            }
            loadedSceneDict.Remove(sceneName);
            if (condition != null)
                yield return new WaitUntil(condition);
            progress?.Invoke(1);
            callback?.Invoke();
            DecrementReferenceCount(sceneName);
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
        IEnumerator EnumLoadAllAssetAsync(string assetBundleName, Action<float> progress, Action<Object[]> callback)
        {
            Object[] assets = null;
            if (string.IsNullOrEmpty(assetBundleName))
                yield break;
            yield return EnumLoadDependenciesAssetBundleAsync(assetBundleName);
            var hasBundle = resourceBundleDict.TryGetValue(assetBundleName, out var bundleWarpper);
            if (hasBundle)
            {
                if (bundleWarpper.AssetBundle == null)
                {
                    callback?.Invoke(assets);
                    progress?.Invoke(1);
                    yield break;
                }
                assets = bundleWarpper.AssetBundle.LoadAllAssets();
                var objs = bundleWarpper.ResourceBundle.ResourceObjectList;
                foreach (var obj in objs)
                {
                    bundleWarpper.ReferenceCount++;
                    var hasObject = resourceObjectDict.TryGetValue(obj.AssetName, out var objectWarpper);
                    if (hasObject)
                    {
                        objectWarpper.ReferenceCount++;
                    }
                }
            }
            callback?.Invoke(assets);
            progress?.Invoke(1);
        }
        void DecrementReferenceCount(string assetName)
        {
            var hasObject = resourceObjectDict.TryGetValue(assetName, out var objectWarpper);
            if (!hasObject)
                return;
            objectWarpper.ReferenceCount--;
            var hasBundle = resourceBundleDict.TryGetValue(objectWarpper.ResourceObject.BundleName, out var bundleWarpper);
            if (hasBundle)
            {
                bundleWarpper.ReferenceCount--;
                if (bundleWarpper.ReferenceCount <= 0)
                    bundleWarpper.AssetBundle.Unload(true);
            }
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
                    if (bundleWarpper.AssetBundle != null)
                        bundleWarpper.AssetBundle.Unload(true);
                }
            }
        }
        void IncrementReferenceCount(string assetName)
        {
            var hasObject = resourceObjectDict.TryGetValue(assetName, out var objectWarpper);
            if (!hasObject)
                return;
            objectWarpper.ReferenceCount++;
            var hasBundle = resourceBundleDict.TryGetValue(objectWarpper.ResourceObject.BundleName, out var bundleWarpper);
            if (hasBundle)
                bundleWarpper.ReferenceCount++;
        }


    }
}