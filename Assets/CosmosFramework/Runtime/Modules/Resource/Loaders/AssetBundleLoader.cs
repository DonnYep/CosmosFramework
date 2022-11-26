using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Cosmos.WebRequest;
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
        readonly Dictionary<string, ResourceObjectWarpper> resourceObjectWarpperDict;
        /// <summary>
        /// bundleName===resourceBundleWarpper
        /// 从框架的角度出发，资源bundle设计上就是以文件夹做包体单位。且编辑器做了限制。因此在原生的模块中，理论上bundleName是唯一的。
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
        ResourceManifestRequester manifestRequester;
        bool manifestAcquired = false;
        bool requestDone = false;
        public AssetBundleLoader(IWebRequestManager webRequestManager)
        {
            loadSceneList = new List<string>();
            resourceAddress = new ResourceAddress();
            resourceBundleWarpperDict = new Dictionary<string, ResourceBundleWarpper>();
            resourceObjectWarpperDict = new Dictionary<string, ResourceObjectWarpper>();
            loadedSceneDict = new Dictionary<string, UnityEngine.SceneManagement.Scene>();
            manifestRequester = new ResourceManifestRequester(webRequestManager, OnManifestSuccess, OnManifestFailure);
        }
        public void InitLoader(string bundleFolderPath)
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            manifestRequester.StartRequestManifest(bundleFolderPath);
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
            if (resourceAddress.PeekAssetPath(assetName, out var path))
            {
                if (resourceObjectWarpperDict.TryGetValue(path, out var objectWarpper))
                    OnResoucreObjectRelease(objectWarpper);
            }
        }
        ///<inheritdoc/> 
        public void ReleaseAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (resourceBundleWarpperDict.TryGetValue(assetBundleName, out var bundleWarpper))
            {
                UnloadDependenciesAssetBundle(bundleWarpper, bundleWarpper.ReferenceCount, unloadAllLoadedObjects);
            }
        }
        ///<inheritdoc/> 
        public void ReleaseAllAsset(bool unloadAllLoadedObjects = false)
        {
            foreach (var objectWarpper in resourceObjectWarpperDict.Values)
            {
                objectWarpper.ReferenceCount = 0;
            }
            foreach (var bundleWarpper in resourceBundleWarpperDict.Values)
            {
                bundleWarpper.ReferenceCount = 0;
                bundleWarpper.AssetBundle?.Unload(unloadAllLoadedObjects);
            }
        }
        ///<inheritdoc/> 
        public void Dispose()
        {
            resourceObjectWarpperDict.Clear();
            resourceBundleWarpperDict.Clear();
            loadSceneList.Clear();
            resourceAddress.Clear();
            loadedSceneDict.Clear();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            manifestAcquired = false;
            requestDone = false;
            resourceManifest = null;
            manifestRequester.Clear();
        }
        IEnumerator EnumLoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null)
        {

            yield return new WaitUntil(() => requestDone);
            if (!manifestAcquired)
            {
                progress?.Invoke(1);
                callback?.Invoke(null);
                yield break;
            }
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
            yield return new WaitUntil(() => requestDone);
            if (!manifestAcquired)
            {
                progress?.Invoke(1);
                callback?.Invoke(null);
                yield break;
            }
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
            var hasBundle = resourceBundleWarpperDict.TryGetValue(bundleName, out var bundleWarpper);
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
            yield return new WaitUntil(() => requestDone);
            if (!manifestAcquired)
            {
                progress?.Invoke(1);
                callback?.Invoke(null);
                yield break;
            }
            //DONE
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
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            yield return new WaitUntil(() => requestDone);
            if (!manifestAcquired)
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
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
            yield return null;
            if (condition != null)
                yield return new WaitUntil(condition);
            operation.allowSceneActivation = true;
            yield return null;
            callback?.Invoke();
        }
        IEnumerator EnumUnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            yield return new WaitUntil(() => requestDone);
            if (!manifestAcquired)
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
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
            yield return null;
            if (condition != null)
                yield return new WaitUntil(condition);
            yield return null;
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
            yield return null;
            callback?.Invoke();
        }
        /// <summary>
        /// 加载依赖，包体与依赖包的引用计数+1
        /// </summary>
        IEnumerator EnumLoadDependenciesAssetBundleAsync(string bundleName)
        {
            //DONE
            var hasBundle = resourceBundleWarpperDict.TryGetValue(bundleName, out var bundleWarpper);
            if (!hasBundle)
                yield break; //若bundle信息为空，则终止；
            if (bundleWarpper.AssetBundle == null)
            {
                var abPath = Path.Combine(ResourceDataProxy.BundlePath, bundleWarpper.ResourceBundle.BundleKey);
                var abReq = AssetBundle.LoadFromFileAsync(abPath, 0, ResourceDataProxy.EncryptionOffset);
                yield return abReq;
                var bundle = abReq.assetBundle;
                if (bundle != null)
                {
                    bundleWarpper.AssetBundle = bundle;
                    bundleWarpper.ReferenceCount++; //AB包引用计数增加
                }
            }
            else
                bundleWarpper.ReferenceCount++; //AB包引用计数增加
            var dependentList = bundleWarpper.ResourceBundle.DependentList;
            var dependentLength = dependentList.Count;
            for (int i = 0; i < dependentLength; i++)
            {
                var dependentABName = dependentList[i];
                if (resourceBundleWarpperDict.TryGetValue(bundleName, out var dependentBundleWarpper))
                {
                    if (dependentBundleWarpper.AssetBundle == null)
                    {
                        var abPath = Path.Combine(ResourceDataProxy.BundlePath, dependentABName);
                        var abReq = AssetBundle.LoadFromFileAsync(abPath, 0, ResourceDataProxy.EncryptionOffset);
                        yield return abReq;
                        var bundle = abReq.assetBundle;
                        if (bundle != null)
                        {
                            dependentBundleWarpper.AssetBundle = bundle;
                            dependentBundleWarpper.ReferenceCount++; //AB包引用计数增加
                        }
                    }
                    else
                        dependentBundleWarpper.ReferenceCount++; //AB包引用计数增加
                }
            }
        }
        /// <summary>
        /// 递归减少包体引用计数；
        /// </summary>
        void UnloadDependenciesAssetBundle(ResourceBundleWarpper resourceBundleWarpper, int count = 1, bool unloadAllLoadedObjects = false)
        {
            resourceBundleWarpper.ReferenceCount -= count;
            if (resourceBundleWarpper.ReferenceCount <= 0)
            {
                //当包体的引用计数小于等于0时，则表示该包已经未被依赖。
                //卸载AssetBundle；
                resourceBundleWarpper.AssetBundle?.Unload(unloadAllLoadedObjects);
            }
            var dependentList = resourceBundleWarpper.ResourceBundle.DependentList;
            var dependentLength = dependentList.Count;
            //遍历查询依赖包
            for (int i = 0; i < dependentLength; i++)
            {
                var dependentBundleName = dependentList[i];
                if (resourceBundleWarpperDict.TryGetValue(dependentBundleName, out var dependentBundleWarpper))
                {
                    dependentBundleWarpper.ReferenceCount -= count;
                    if (dependentBundleWarpper.ReferenceCount <= 0)
                    {
                        dependentBundleWarpper.AssetBundle?.Unload(unloadAllLoadedObjects);
                    }
                }
            }
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
                UnloadDependenciesAssetBundle(bundleWarpper, count);
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
            resourceObjectWarpper.ReferenceCount++;
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
            UnloadDependenciesAssetBundle(resourceBundleWarpper);
        }
        void OnManifestFailure(string errorMessage)
        {
            Utility.Debug.LogError("ResourceManifest deserialization failed , check your file !");
            manifestAcquired = false;
            requestDone = true;
        }
        void OnManifestSuccess(ResourceManifest resourceManifest)
        {
            this.resourceManifest = resourceManifest;
            var bundleBuildInfoDict = resourceManifest.ResourceBundleBuildInfoDict;
            foreach (var bundleBuildInfo in bundleBuildInfoDict.Values)
            {
                var resourceBundle = bundleBuildInfo.ResourceBundle;
                resourceBundleWarpperDict.TryAdd(resourceBundle.BundleName, new ResourceBundleWarpper(resourceBundle));
                var resourceObjectList = resourceBundle.ResourceObjectList;
                var objectLength = resourceObjectList.Count;
                for (int i = 0; i < objectLength; i++)
                {
                    var resourceObject = resourceObjectList[i];
                    resourceObjectWarpperDict.TryAdd(resourceObject.AssetPath, new ResourceObjectWarpper(resourceObject));
                }
                resourceAddress.AddResourceObjects(resourceObjectList);
            }
            manifestAcquired = true;
            requestDone = true;
        }
    }
}