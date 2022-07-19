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
        ResourceManifest resourceManifest;
        /// <summary>
        /// assetPath===resourceObjectWarpper
        /// 理论上资源地址在unity中应该是唯一的。
        /// 资源地址相同但文件bytes内容改变，打包时生成的hash也会与之不同。因此理论上应该是assetPath是唯一的。
        /// </summary>
        readonly Dictionary<string, ResourceObjectWarpper> resourceObjectDict;
        /// <summary>
        /// bundleName===resourceBundleWarpper
        /// 从框架的角度出发，资源bundle设计上就是以文件夹做包体单位。且编辑器做了限制。因此在原生的模块中，理论上bundleName是唯一的。
        /// </summary>
        readonly Dictionary<string, ResourceBundleWarpper> resourceBundleDict;
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
        public AssetBundleLoader()
        {
            loadSceneList = new List<string>();
            resourceAddress = new ResourceAddress();
            resourceBundleDict = new Dictionary<string, ResourceBundleWarpper>();
            resourceObjectDict = new Dictionary<string, ResourceObjectWarpper>();
            loadedSceneDict = new Dictionary<string, UnityEngine.SceneManagement.Scene>();
        }
        public void InitLoader(ResourceManifest resourceManifest)
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            this.resourceManifest = resourceManifest;
            var bundleBuildInfoDict = resourceManifest.ResourceBundleBuildInfoDict;
            foreach (var bundleBuildInfo in bundleBuildInfoDict.Values)
            {
                var resourceBundle = bundleBuildInfo.ResourceBundle;
                resourceBundleDict.TryAdd(resourceBundle.BundleName, new ResourceBundleWarpper(resourceBundle));
                var resourceObjectList = resourceBundle.ResourceObjectList;
                var objectLength = resourceObjectList.Count;
                for (int i = 0; i < objectLength; i++)
                {
                    var resourceObject = resourceObjectList[i];
                    resourceObjectDict.TryAdd(resourceObject.AssetPath, new ResourceObjectWarpper(resourceObject));
                }
                resourceAddress.AddResourceObjects(resourceObjectList);
            }
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, typeof(T), (asset) => { callback?.Invoke(asset as T); }, progress));
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
            if (resourceObjectDict.TryGetValue(assetName, out var objectWarpper))
                OnResoucreObjectRelease(objectWarpper);
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
                OnResoucreObjectRelease(objectWarpper);
            }
        }
        ///<inheritdoc/> 
        public void Dispose()
        {
            resourceObjectDict.Clear();
            resourceBundleDict.Clear();
            resourceAddress.Clear();
            loadedSceneDict.Clear();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        IEnumerator EnumLoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null)
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
            var hasBundle = resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper);
            if (!hasBundle)
            {
                progress?.Invoke(1);
                callback?.Invoke(asset);
                yield break;
            }
            if (bundleWarpper.AssetBundle == null)
            {
                progress?.Invoke(1);
                callback?.Invoke(asset);
                yield break;
            }
            var req = bundleWarpper.AssetBundle.LoadAssetAsync(assetName, type);
            while (!req.isDone)
            {
                progress?.Invoke(req.progress);
                if (req.progress >= 0.9f)
                    break;
                yield return null;
            }
            progress?.Invoke(1);
            asset = req.asset;
            if (asset != null)
            {
                OnResourceObjectLoad(resourceObject);
            }
            callback?.Invoke(asset);
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress)
        {
            //DONE
            Object[] assets = null;
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
            var hasBundle = resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper);
            if (!hasBundle)
            {
                progress?.Invoke(1);
                callback?.Invoke(assets);
                yield break;
            }
            if (bundleWarpper.AssetBundle == null)
            {
                progress?.Invoke(1);
                callback?.Invoke(assets);
                yield break;
            }
            var req = bundleWarpper.AssetBundle.LoadAssetWithSubAssetsAsync(assetName, type);
            while (!req.isDone)
            {
                progress?.Invoke(req.progress);
                if (req.progress >= 0.9f)
                    break;
                yield return null;
            }
            progress?.Invoke(1);
            assets = req.allAssets;
            if (assets != null)
            {
                OnResourceObjectLoad(resourceObject);
            }
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadAllAssetAsync(string bundleName, Action<float> progress, Action<Object[]> callback)
        {
            //DONE
            Object[] assets = null;
            var hasBundle = resourceBundleDict.TryGetValue(bundleName, out var bundleWarpper);
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
            if (bundleWarpper.AssetBundle == null)
            {
                progress?.Invoke(1);
                callback?.Invoke(assets);
                yield break;
            }
            var req = bundleWarpper.AssetBundle.LoadAllAssetsAsync();
            while (!req.isDone)
            {
                progress?.Invoke(req.progress);
                if (req.progress >= 0.9f)
                    break;
                yield return null;
            }
            progress?.Invoke(1);
            assets = req.allAssets;
            if (assets != null)
            {
                OnResourceBundleAllAssetLoad(bundleName);
            }
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadDependenciesAssetBundleAsync(string bundleName)
        {
            //DONE
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
                        {
                            dependentBundleWarpper.AssetBundle = bundle;
                            //依赖的AB包引用计数增加
                            OnResourceBundleIncrement(dependentBundleWarpper);
                        }
                    }
                }
            }
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
            var operation = SceneManager.LoadSceneAsync(info.SceneName, loadSceneMode);
            if (operation == null)
            {
                //为空表示场景不存在
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            loadSceneList.Add(info.SceneName);
            operation.allowSceneActivation = false;
            var hasProviderProgress = progressProvider != null;
            while (!operation.isDone)
            {
                if (hasProviderProgress)
                {
                    var providerProgress = progressProvider();
                    var sum = providerProgress + operation.progress;
                    if (sum >= 1.9)
                        break;
                    else
                        progress?.Invoke(sum / 2);
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
                progress?.Invoke(overallProgress / 100);
            }
            loadSceneList.Clear();
            progress?.Invoke(1);
            callback?.Invoke();
        }
        bool PeekResourceObject(string assetName, out ResourceObject resourceObject)
        {
            resourceObject = ResourceObject.None;
            if (resourceAddress.PeekAssetPath(assetName, out var path))
            {
                if (!resourceObjectDict.TryGetValue(path, out var resourceObjectWarpper))
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
            if (resourceBundleDict.TryGetValue(objectWarpper.ResourceObject.AssetName, out var bundleWarpper))
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
            if (!resourceObjectDict.TryGetValue(resourceObject.AssetPath, out var resourceObjectWarpper))
                return;
            if (!resourceBundleDict.TryGetValue(resourceObject.BundleName, out var resourceBundleWarpper))
                return;
            resourceObjectWarpper.ReferenceCount++;
            resourceBundleWarpper.ReferenceCount++;
        }
        /// <summary>
        /// 当资源包中的所有对象被加载；
        /// </summary>
        void OnResourceBundleAllAssetLoad(string bundleName)
        {
            if (!resourceBundleDict.TryGetValue(bundleName, out var resourceBundleWarpper))
                return;
            var ResourceObjectList = resourceBundleWarpper.ResourceBundle.ResourceObjectList;
            foreach (var resourceObject in ResourceObjectList)
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
            if (!resourceObjectDict.TryGetValue(assetPath, out var resourceObjectWarpper))
                return;
            if (!resourceBundleDict.TryGetValue(resourceObjectWarpper.ResourceObject.BundleName, out var resourceBundleWarpper))
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
                    if (!resourceBundleDict.TryGetValue(dependBundleName, out var dependBundleWarpper))
                        continue;
                    OnResourceBundleDecrement(dependBundleWarpper);
                }
            }
        }
        /// <summary>
        /// 当包体引用计数+1
        /// </summary>
        void OnResourceBundleIncrement(ResourceBundleWarpper resourceBundleWarpper)
        {
            resourceBundleWarpper.ReferenceCount++;
            var dependBundleNames = resourceBundleWarpper.ResourceBundle.DependList;
            var dependBundleNameLength = dependBundleNames.Count;
            //遍历查询依赖包
            for (int i = 0; i < dependBundleNameLength; i++)
            {
                var dependBundleName = dependBundleNames[i];
                if (!resourceBundleDict.TryGetValue(dependBundleName, out var dependBundleWarpper))
                    continue;
                if (dependBundleWarpper.AssetBundle == null)
                    continue;
                //依赖包体引用计数+1
                dependBundleWarpper.ReferenceCount++;
                OnResourceBundleIncrement(dependBundleWarpper);
            }
        }
    }
}