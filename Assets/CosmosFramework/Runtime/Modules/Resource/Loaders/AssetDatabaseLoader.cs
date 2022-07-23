using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    public class AssetDatabaseLoader : IResourceLoadHelper
    {
        ResourceDataset resourceDataset;
        /// <summary>
        /// assetName===resourceObjectWarpper
        /// </summary>
        readonly Dictionary<string, ResourceObjectWarpper> resourceObjectWarpperDict;
        /// <summary>
        /// bundleName===resourceBundleWarpper
        /// </summary>
        readonly Dictionary<string, ResourceBundleWarpper> resourceBundleWarpperDict;
        /// <summary>
        /// 被加载的场景字典；
        /// SceneName===Scene
        /// </summary>
        readonly Dictionary<string, UnityEngine.SceneManagement.Scene> loadedSceneDict;
        /// <summary>
        /// 资源寻址地址；
        /// </summary>
        readonly ResourceAddress resourceAddress;
        /// <summary>
        /// 主动加载的场景列表；
        /// </summary>
        readonly List<string> loadSceneList;
        public AssetDatabaseLoader()
        {
            loadSceneList = new List<string>();
            resourceAddress = new ResourceAddress();
            resourceBundleWarpperDict = new Dictionary<string, ResourceBundleWarpper>();
            resourceObjectWarpperDict = new Dictionary<string, ResourceObjectWarpper>();
            loadedSceneDict = new Dictionary<string, UnityEngine.SceneManagement.Scene>();
        }
        public void InitLoader(ResourceDataset resourceDataset)
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            this.resourceDataset = resourceDataset;
            var resourceBundleList = resourceDataset.ResourceBundleList;
            foreach (var resourceBundle in resourceBundleList)
            {
                resourceBundleWarpperDict.TryAdd(resourceBundle.BundleName, new ResourceBundleWarpper(resourceBundle));
                var resourceObjectList = resourceBundle.ResourceObjectList;
                var objectLength = resourceObjectList.Count;
                for (int i = 0; i < objectLength; i++)
                {
                    var resourceObject = resourceObjectList[i];
                    resourceObjectWarpperDict.TryAdd(resourceObject.AssetName, new ResourceObjectWarpper(resourceObject));
                }
            }
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, typeof(T), asset => { callback?.Invoke(asset as T); }, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, type, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadMainAndSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, typeof(T), assets =>
            {
                T[] rstAssets = new T[assets.Length];
                var length = rstAssets.Length;
                for (int i = 0; i < length; i++)
                {
                    rstAssets[i] = assets[i] as T;
                }
                callback?.Invoke(rstAssets);
            }, progress));
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
            OnResourceObjectUnload(assetName);
        }
        ///<inheritdoc/> 
        public void ReleaseAsset(string assetName)
        {
            if (resourceObjectWarpperDict.TryGetValue(assetName, out var objectWarpper))
                OnResoucreObjectRelease(objectWarpper);
        }
        ///<inheritdoc/> 
        public void ReleaseAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (resourceBundleWarpperDict.TryGetValue(assetBundleName, out var bundleWarpper))
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
            foreach (var objectWarpper in resourceObjectWarpperDict.Values)
            {
                OnResoucreObjectRelease(objectWarpper);
            }
        }
        ///<inheritdoc/> 
        public void Dispose()
        {
            resourceObjectWarpperDict.Clear();
            resourceBundleWarpperDict.Clear();
            resourceAddress.Clear();
            loadedSceneDict.Clear();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        IEnumerator EnumLoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress)
        {
            //DONE
            Object asset = null;
            var bundleName = string.Empty;
            var hasObject = PeekResourceObject(assetName, out var resourceObject);
            if (hasObject)
                bundleName = resourceObject.BundleName;
            else
            {
                progress?.Invoke(1);
                callback?.Invoke(asset);
                yield break;
            }
            if (string.IsNullOrEmpty(bundleName))
            {
                progress?.Invoke(1);
                callback?.Invoke(asset);
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(bundleName);
            var hasBundle = resourceBundleWarpperDict.TryGetValue(bundleName, out var bundleWarpper);
            if (!hasBundle)
            {
                progress?.Invoke(1);
                callback?.Invoke(asset);
                yield break;
            }
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(resourceObject.AssetPath))
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath(resourceObject.AssetPath, type);
            if (asset != null)
            {
                OnResourceObjectLoad(resourceObject);
            }
#endif
            yield return null;
            progress?.Invoke(1);
            callback?.Invoke(asset);
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress)
        {
            //DONE
            Object[] assets = default;
            var bundleName = string.Empty;
            var hasObject = PeekResourceObject(assetName, out var resourceObject);
            if (hasObject)
                bundleName = resourceObject.BundleName;
            else
            {
                progress?.Invoke(1);
                callback?.Invoke(assets);
                yield break;
            }
            if (string.IsNullOrEmpty(bundleName))
            {
                progress?.Invoke(1);
                callback?.Invoke(assets);
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(bundleName);
            var hasBundle = resourceBundleWarpperDict.TryGetValue(bundleName, out var bundleWarpper);
            if (!hasBundle)
            {
                progress?.Invoke(1);
                callback?.Invoke(assets);
                yield break;
            }
#if UNITY_EDITOR
            assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(resourceObject.AssetPath);
            if (assets != null)
            {
                OnResourceObjectLoad(resourceObject);
            }
#endif
            yield return null;
            progress?.Invoke(1);
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadAllAssetAsync(string bundleName, Action<float> progress, Action<Object[]> callback)
        {
            Object[] assets = null;
            var hasBundle = resourceBundleWarpperDict.TryGetValue(bundleName, out var bundleWarpper);
            if (!hasBundle)
            {
                progress?.Invoke(1);
                callback?.Invoke(assets);
                yield break;
            }
            if (string.IsNullOrEmpty(bundleName))
            {
                progress?.Invoke(1);
                callback?.Invoke(assets);
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(bundleName);

#if UNITY_EDITOR
            List<Object> assetList = new List<Object>();
            var resourceObjects = bundleWarpper.ResourceBundle.ResourceObjectList;
            foreach (var resourceObject in resourceObjects)
            {
                bundleWarpper.ReferenceCount++;
                var hasObject = resourceObjectWarpperDict.TryGetValue(resourceObject.AssetName, out var objectWarpper);
                if (hasObject)
                {
                    objectWarpper.ReferenceCount++;
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(resourceObject.AssetPath, typeof(Object));
                    assetList.Add(asset);
                }
            }
            assets = assetList.ToArray();
            OnResourceBundleAllAssetLoad(bundleName);
#endif
            callback?.Invoke(assets);
            progress?.Invoke(1);
        }
        IEnumerator EnumLoadDependenciesAssetBundleAsync(string bundleName)
        {
            yield return null;
        }
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            var sceneName = info.SceneName;
            if (loadedSceneDict.TryGetValue(sceneName, out var loadedScene))
            {
                progress?.Invoke(1);
                SceneManager.SetActiveScene(loadedScene);
                callback?.Invoke();
                yield break;
            }
            var bundleName = string.Empty;
            var hasObject = PeekResourceObject(sceneName, out var resourceObject);
            if (hasObject)
                bundleName = resourceObject.BundleName;
            else
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            if (string.IsNullOrEmpty(bundleName))
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(bundleName);
            LoadSceneMode loadSceneMode = info.Additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            if (operation == null)
            {
                //为空表示场景不存在
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            loadSceneList.Add(sceneName);
            operation.allowSceneActivation = false;
            var hasProviderProgress = progressProvider != null;
            while (!operation.isDone)
            {
                if (hasProviderProgress)
                {
                    var providerProgress = progressProvider();
                    var sum = providerProgress + operation.progress;
                    progress?.Invoke(sum / 2);
                    if (sum >= 1.9)
                        break;
                }
                else
                {
                    progress?.Invoke(operation.progress);
                    if (operation.progress >= 0.9f)
                        break;
                }
                yield return null;
            }
            progress?.Invoke(1);
            if (condition != null)
                yield return new WaitUntil(condition);
            operation.allowSceneActivation = true;
            callback?.Invoke();
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
            var operation = SceneManager.UnloadSceneAsync(scene);
            while (!operation.isDone)
            {
                progress?.Invoke(operation.progress);
                if (operation.progress >= 0.9f)
                    break;
                yield return null;
            }
            progress?.Invoke(1);
            if (condition != null)
                yield return new WaitUntil(condition);
            callback?.Invoke();
        }
        IEnumerator EnumUnloadAllSceneAsync(Action<float> progress, Action callback)
        {
            var sceneCount = loadedSceneDict.Count;
            //单位场景的百分比比率
            var unitResRatio = 100f / sceneCount;
            int currentSceneIndex = 0;
            float overallProgress = 0;
            foreach (var sceneName in loadSceneList)
            {
                var overallIndexPercent = 100 * ((float)currentSceneIndex / sceneCount);
                currentSceneIndex++;
                if (!loadedSceneDict.TryGetValue(sceneName, out var scene))
                    continue;
                var ao = SceneManager.UnloadSceneAsync(scene);
                while (!ao.isDone)
                {
                    overallProgress = overallIndexPercent + (unitResRatio * ao.progress);
                    progress?.Invoke(overallProgress / 100);
                    yield return null;
                }
                overallProgress = overallIndexPercent + (unitResRatio * 1);
                progress?.Invoke(overallProgress / 100); ;
            }
            loadSceneList.Clear();
            progress?.Invoke(1);
            callback?.Invoke();
        }
        bool PeekResourceObject(string assetName, out ResourceObject resourceObject)
        {
            resourceObject = null;
            if (resourceAddress.PeekAssetPath(assetName, out var path))
            {
                if (!resourceObjectWarpperDict.TryGetValue(path, out var resourceObjectWarpper))
                    return false;
                resourceObject = resourceObjectWarpper.ResourceObject;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 当对象被释放；
        /// </summary>
        void OnResoucreObjectRelease(ResourceObjectWarpper objectWarpper)
        {
            var count = objectWarpper.ReferenceCount;
            objectWarpper.ReferenceCount = 0;
            if (resourceBundleWarpperDict.TryGetValue(objectWarpper.ResourceObject.AssetName, out var bundleWarpper))
            {
                bundleWarpper.ReferenceCount -= count;
                OnResourceBundleDecrement(bundleWarpper);
            }
        }
        /// <summary>
        /// 当场景被加载；
        /// </summary>
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadSceneMode)
        {
            var sceneName = scene.name;
            if (loadSceneList.Contains(sceneName))
            {
                if (PeekResourceObject(sceneName, out var resourceObject))
                    OnResourceObjectLoad(resourceObject);
                loadedSceneDict.TryAdd(sceneName, scene);
            }
        }
        /// <summary>
        /// 当场景被卸载；
        /// </summary>
        void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
        {
            var sceneName = scene.name;
            if (loadSceneList.Remove(sceneName))
                OnResourceObjectUnload(sceneName);
            loadedSceneDict.Remove(sceneName);
        }
        /// <summary>
        /// 只负责计算引用计数
        /// </summary>ram>
        void OnResourceObjectLoad(ResourceObject resourceObject)
        {
            if (!resourceObjectWarpperDict.TryGetValue(resourceObject.AssetPath, out var resourceObjectWarpper))
                return;
            if (!resourceBundleWarpperDict.TryGetValue(resourceObject.BundleName, out var resourceBundleWarpper))
                return;
            resourceObjectWarpper.ReferenceCount++;
            resourceBundleWarpper.ReferenceCount++;
        }
        /// <summary>
        /// 当资源包中的所有对象被加载；
        /// </summary>
        void OnResourceBundleAllAssetLoad(string bundleName)
        {
            if (!resourceBundleWarpperDict.TryGetValue(bundleName, out var resourceBundleWarpper))
                return;
            var resourceObjectList = resourceBundleWarpper.ResourceBundle.ResourceObjectList;
            foreach (var resourceObject in resourceObjectList)
            {
                OnResourceObjectLoad(resourceObject);
            }
        }
        /// <summary>
        /// 当资源对象被卸载；
        /// </summary>
        void OnResourceObjectUnload(string assetName)
        {
            if (!resourceAddress.PeekAssetPath(assetName, out var assetPath))
                return;
            if (!resourceObjectWarpperDict.TryGetValue(assetPath, out var resourceObjectWarpper))
                return;
            if (!resourceBundleWarpperDict.TryGetValue(resourceObjectWarpper.ResourceObject.BundleName, out var resourceBundleWarpper))
                return;
            resourceObjectWarpper.ReferenceCount--;
            OnResourceBundleDecrement(resourceBundleWarpper);
        }
        /// <summary>
        /// 当包体引用计数-1
        /// </summary>
        void OnResourceBundleDecrement(ResourceBundleWarpper resourceBundleWarpper)
        {
            resourceBundleWarpper.ReferenceCount--;
            if (resourceBundleWarpper.ReferenceCount <= 0)
            {
                //当包体的引用计数小于等于0时，则表示该包已经未被依赖。
                if (resourceBundleWarpper.AssetBundle == null)
                    return;
                //卸载AssetBundle；
                resourceBundleWarpper.AssetBundle.Unload(true);
                var dependBundleNames = resourceBundleWarpper.ResourceBundle.DependList;
                var dependBundleNameLength = dependBundleNames.Count;
                //遍历查询依赖包
                for (int i = 0; i < dependBundleNameLength; i++)
                {
                    var dependBundleName = dependBundleNames[i];
                    if (!resourceBundleWarpperDict.TryGetValue(dependBundleName, out var dependBundleWarpper))
                        continue;
                    OnResourceBundleDecrement(dependBundleWarpper);
                }
            }
        }
        /// <summary>
        /// 递归减少包体引用计数；
        /// </summary>
        void OnDecrementDependenciesAssetBundleAsync(ResourceBundleWarpper resourceBundleWarpper)
        {
            resourceBundleWarpper.ReferenceCount--;
            if (resourceBundleWarpper.ReferenceCount <= 0)
            {
                //当包体的引用计数小于等于0时，则表示该包已经未被依赖。
                //卸载AssetBundle；
                resourceBundleWarpper.AssetBundle?.Unload(true);
            }
            var dependBundleNames = resourceBundleWarpper.ResourceBundle.DependList;
            var dependBundleNameLength = dependBundleNames.Count;
            //遍历查询依赖包
            for (int i = 0; i < dependBundleNameLength; i++)
            {
                var dependBundleName = dependBundleNames[i];
                if (!resourceBundleWarpperDict.TryGetValue(dependBundleName, out var dependBundleWarpper))
                    continue;
                OnDecrementDependenciesAssetBundleAsync(dependBundleWarpper);
            }
        }
    }
}
