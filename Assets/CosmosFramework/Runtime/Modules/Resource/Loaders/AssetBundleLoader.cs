using Cosmos.Resource.State;
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
        /// <summary>
        /// assetPath===resourceObjectWarpper
        /// <para>理论上资源地址在unity中应该是唯一的</para> 
        /// <para>资源地址相同但文件bytes内容改变，打包时生成的hash也会与之不同。因此理论上应该是assetPath是唯一的</para>
        /// </summary>
        readonly Dictionary<string, ResourceObjectWarpper> resourceObjectWarpperDict;
        /// <summary>
        /// bundleName===resourceBundleWarpper
        /// <para>从框架的角度出发，资源bundle设计上就是以文件夹做包体单位。且编辑器做了限制。因此在原生的模块中，理论上bundleName是唯一的</para> 
        /// </summary>
        readonly Dictionary<string, ResourceBundleWarpper> resourceBundleWarpperDict;
        /// <summary>
        /// AB异步请求记录
        /// </summary>
        readonly Dictionary<string, AssetBundleCreateRequest> abRequestDict;
        /// <summary>
        /// bundleKey===bundleName
        /// </summary>
        readonly Dictionary<string, string> resourceBundleKeyDict;
        /// <summary>
        /// 被加载的场景字典，SceneName===Scene
        /// </summary>
        readonly Dictionary<string, UnityEngine.SceneManagement.Scene> loadedSceneDict;
        /// <summary>
        /// 资源寻址地址
        /// </summary>
        readonly ResourceAddress resourceAddress;
        /// <summary>
        /// 主动加载的场景列表
        /// </summary>
        readonly List<string> loadSceneList;
        bool manifestAcquired = false;
        bool requestDone = false;
        bool abort = false;
        ResourceManifest resourceManifest;
        readonly List<ResourceBundleState> bundleStateCache = new List<ResourceBundleState>();
        public long TaskId { get; internal set; }
        public AssetBundleLoader()
        {
            loadSceneList = new List<string>();
            resourceAddress = new ResourceAddress();
            resourceBundleKeyDict = new Dictionary<string, string>();
            resourceBundleWarpperDict = new Dictionary<string, ResourceBundleWarpper>();
            resourceObjectWarpperDict = new Dictionary<string, ResourceObjectWarpper>();
            loadedSceneDict = new Dictionary<string, UnityEngine.SceneManagement.Scene>();
            abRequestDict = new Dictionary<string, AssetBundleCreateRequest>();
        }
        ///<inheritdoc/> 
        public void OnInitialize()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            CosmosEntry.ResourceManager.ResourceRequestManifestFailure += RequestManifestFailure;
            CosmosEntry.ResourceManager.ResourceRequestManifestSuccess += RequestManifestSuccess;
            abort = false;
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
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            return Utility.Unity.StartCoroutine(EnumLoadSceneAsync(sceneAssetInfo, progressProvider, progress, condition, callback));
        }
        ///<inheritdoc/> 
        public Coroutine UnloadSceneAsync(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback)
        {
            return Utility.Unity.StartCoroutine(EnumUnloadSceneAsync(sceneAssetInfo, progress, condition, callback));
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
        public void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects)
        {
            if (resourceBundleWarpperDict.TryGetValue(assetBundleName, out var bundleWarpper))
            {
                UnloadDependenciesAssetBundle(bundleWarpper, bundleWarpper.ReferenceCount, unloadAllLoadedObjects);
            }
        }
        ///<inheritdoc/> 
        public void UnloadAllAsset(bool unloadAllLoadedObjects)
        {
            foreach (var objectWarpper in resourceObjectWarpperDict.Values)
            {
                objectWarpper.ReferenceCount = 0;
            }
            foreach (var bundleWarpper in resourceBundleWarpperDict.Values)
            {
                bundleWarpper.ReferenceCount = 0;
                if (bundleWarpper.AssetBundle != null)
                {
                    bundleWarpper.AssetBundle?.Unload(unloadAllLoadedObjects);
                    bundleWarpper.AssetBundle = null;
                }
            }
        }
        ///<inheritdoc/> 
        public bool GetBundleState(string bundleName, out ResourceBundleState bundleState)
        {
            bundleState = ResourceBundleState.Default;
            var hasBundle = resourceBundleWarpperDict.TryGetValue(bundleName, out var bundleWarpper);
            bundleState = new ResourceBundleState()
            {
                ResourceBundleName = bundleWarpper.ResourceBundle.BundleName,
                ResourceBundleReferenceCount = bundleWarpper.ReferenceCount,
                ResourceObjectCount = bundleWarpper.ResourceBundle.ResourceObjectList.Count
            };
            return hasBundle;
        }
        ///<inheritdoc/> 
        public bool GetObjectState(string objectName, out ResourceObjectState objectState)
        {
            objectState = ResourceObjectState.Default;
            var hasObject = PeekResourceObject(objectName, out var resourceObject);
            if (!hasObject)
                return false;
            if (!resourceObjectWarpperDict.TryGetValue(resourceObject.ObjectPath, out var resourceObjectWarpper))
                return false;
            objectState = new ResourceObjectState()
            {
                ResourceBundleName = resourceObject.BundleName,
                ResourceExtension = resourceObject.Extension,
                ResourceObjectName = resourceObject.ObjectName,
                ResourceObjectPath = resourceObject.ObjectPath,
                ResourceReferenceCount = resourceObjectWarpper.ReferenceCount
            };
            return hasObject;
        }
        ///<inheritdoc/> 
        public ResourceBundleState[] GetLoadedBundleState()
        {
            bundleStateCache.Clear();
            foreach (var bundleWarpper in resourceBundleWarpperDict.Values)
            {
                var bundleState = new ResourceBundleState()
                {
                    ResourceBundleName = bundleWarpper.ResourceBundle.BundleName,
                    ResourceBundleReferenceCount = bundleWarpper.ReferenceCount,
                    ResourceObjectCount = bundleWarpper.ResourceBundle.ResourceObjectList.Count
                };
                bundleStateCache.Add(bundleState);
            }
            return bundleStateCache.ToArray();
        }
        ///<inheritdoc/> 
        public ResourceVersion GetResourceVersion()
        {
            var version = resourceManifest == null ? Constants.NULL : resourceManifest.BuildVersion;
            return new ResourceVersion(version, "Build by resouce pipeline");
        }
        ///<inheritdoc/> 
        public void Reset()
        {
            resourceObjectWarpperDict.Clear();
            resourceBundleWarpperDict.Clear();
            resourceBundleKeyDict.Clear();
            loadSceneList.Clear();
            resourceAddress.Clear();
            loadedSceneDict.Clear();
            manifestAcquired = false;
            requestDone = false;
            abort = false;
        }
        ///<inheritdoc/> 
        public void OnTerminate()
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
            abort = true;
            CosmosEntry.ResourceManager.ResourceRequestManifestFailure -= RequestManifestFailure;
            CosmosEntry.ResourceManager.ResourceRequestManifestSuccess -= RequestManifestSuccess;
        }
        IEnumerator EnumLoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null)
        {
            yield return new WaitUntil(() => requestDone || abort);
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
                OnResourceObjectNotExists(assetName);
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
            else
            {
                OnResourceObjectNotExists(assetName);
            }
            callback?.Invoke(asset);
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress)
        {
            yield return new WaitUntil(() => requestDone || abort);
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
                OnResourceObjectNotExists(assetName);
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
            else
            {
                OnResourceObjectNotExists(assetName);
            }
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadAllAssetAsync(string bundleName, Action<float> progress, Action<Object[]> callback)
        {
            yield return new WaitUntil(() => requestDone || abort);
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
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            yield return new WaitUntil(() => requestDone || abort);
            if (!manifestAcquired)
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            var sceneName = sceneAssetInfo.SceneName;
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
                OnResourceObjectNotExists(sceneName);
                yield break;
            }
            if (string.IsNullOrEmpty(bundleName))
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(bundleName);
            LoadSceneMode loadSceneMode = sceneAssetInfo.Additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var operation = SceneManager.LoadSceneAsync(sceneAssetInfo.SceneName, loadSceneMode);
            if (operation == null)
            {
                //为空表示场景不存在
                progress?.Invoke(1);
                callback?.Invoke();
                OnResourceObjectNotExists(sceneName);
                yield break;
            }
            loadSceneList.Add(sceneAssetInfo.SceneName);
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
            yield return operation.isDone;
            yield return null;
            callback?.Invoke();
        }
        IEnumerator EnumUnloadSceneAsync(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback)
        {
            yield return new WaitUntil(() => requestDone || abort);
            if (!manifestAcquired)
            {
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            var sceneName = sceneAssetInfo.SceneName;
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
            if (operation == null)
            {
                OnSceneUnloaded(scene);
                progress?.Invoke(1);
                yield return null;
                callback?.Invoke();
                yield break;
            }
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
            yield return operation.isDone;
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
            var loadSceneArr = loadSceneList.ToArray();
            foreach (var sceneName in loadSceneArr)
            {
                var overallIndexPercent = 100 * ((float)currentSceneIndex / sceneCount);
                currentSceneIndex++;
                if (!loadedSceneDict.TryGetValue(sceneName, out var scene))
                    continue;
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation != null)
                {
                    while (!operation.isDone)
                    {
                        overallProgress = overallIndexPercent + (unitResRatio * operation.progress);
                        progress?.Invoke(overallProgress / 100);
                        yield return null;
                    }
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
                yield break; //若bundleWrapper信息为空，则终止
            yield return EnumLoadAssetBundleAsync(bundleName);
            var bundleDependencies = bundleWarpper.ResourceBundle.BundleDependencies;
            var bundleDependenciesLength = bundleDependencies.Count;
            for (int i = 0; i < bundleDependenciesLength; i++)
            {
                var bundleDependency = bundleDependencies[i];
                var hasDependentBundle = resourceBundleKeyDict.TryGetValue(bundleDependency.BundleKey, out var dependentBundleName);
                if (hasDependentBundle)
                {
                    yield return EnumLoadAssetBundleAsync(dependentBundleName);
                }
            }
            yield return null;
        }
        IEnumerator EnumLoadAssetBundleAsync(string bundleName)
        {
            var hasBundle = resourceBundleWarpperDict.TryGetValue(bundleName, out var bundleWarpper);
            if (!hasBundle)
                yield break; //若bundleWrapper信息为空，则终止
            if (bundleWarpper.AssetBundle == null)
            {
                var abPath = Path.Combine(bundleWarpper.BundlePath, bundleWarpper.ResourceBundle.BundleKey);
                if (!string.IsNullOrEmpty(bundleWarpper.BundleExtension))
                {
                    abPath = Utility.Text.Combine(abPath, ".", bundleWarpper.BundleExtension);
                }
                if (abRequestDict.TryGetValue(abPath, out var abReq))
                {
                    yield return new WaitUntil(() => abReq.isDone);
                }
                else
                {
                    abReq = AssetBundle.LoadFromFileAsync(abPath, 0, bundleWarpper.BundleOffset);
                    abRequestDict.Add(abPath, abReq);
                    yield return abReq;
                }
                abRequestDict.Remove(abPath);
                var bundle = abReq.assetBundle;
                if (bundle != null)
                {
                    bundleWarpper.AssetBundle = bundle;
                    bundleWarpper.ReferenceCount++; //AB包引用计数增加
                }
            }
            else
                bundleWarpper.ReferenceCount++; //AB包引用计数增加
            yield return null;
        }
        /// <summary>
        /// 递归减少包体引用计数
        /// </summary>
        void UnloadDependenciesAssetBundle(ResourceBundleWarpper resourceBundleWarpper, int count, bool unloadAllLoadedObjects)
        {
            resourceBundleWarpper.ReferenceCount -= count;
            if (resourceBundleWarpper.ReferenceCount <= 0)
            {
                //当包体的引用计数小于等于0时，则表示该包已经未被依赖。
                //卸载AssetBundle；
                if (resourceBundleWarpper.AssetBundle != null)
                {
                    resourceBundleWarpper.AssetBundle?.Unload(unloadAllLoadedObjects);
                    resourceBundleWarpper.AssetBundle = null;
                }
            }
            var bundleDependencies = resourceBundleWarpper.ResourceBundle.BundleDependencies;
            var bundleDependenciesLength = bundleDependencies.Count;
            //遍历查询依赖包
            for (int i = 0; i < bundleDependenciesLength; i++)
            {
                var bundleDependency = bundleDependencies[i];
                if (resourceBundleWarpperDict.TryGetValue(bundleDependency.BundleKey, out var dependentBundleWarpper))
                {
                    dependentBundleWarpper.ReferenceCount -= count;
                    if (dependentBundleWarpper.ReferenceCount > 0)
                        continue;
                    if (dependentBundleWarpper.AssetBundle != null)
                    {
                        dependentBundleWarpper.AssetBundle?.Unload(unloadAllLoadedObjects);
                        dependentBundleWarpper.AssetBundle = null;
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
        /// 当对象被释放
        /// </summary>
        void OnResoucreObjectRelease(ResourceObjectWarpper objectWarpper)
        {
            var count = objectWarpper.ReferenceCount;
            objectWarpper.ReferenceCount = 0;
            if (resourceBundleWarpperDict.TryGetValue(objectWarpper.ResourceObject.ObjectName, out var bundleWarpper))
            {
                UnloadDependenciesAssetBundle(bundleWarpper, count, ResourceDataProxy.UnloadAllLoadedObjectsWhenBundleUnload);
            }
        }
        /// <summary>
        /// 当场景被加载
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
        /// 当场景被卸载
        /// </summary>
        void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
        {
            var sceneName = scene.name;
            if (loadSceneList.Remove(sceneName))
                OnResourceObjectUnload(sceneName);
            loadedSceneDict.Remove(sceneName);
        }
        /// <summary>
        /// 只负责计算资源对象的引用计数
        /// </summary>
        void OnResourceObjectLoad(ResourceObject resourceObject)
        {
            if (!resourceObjectWarpperDict.TryGetValue(resourceObject.ObjectPath, out var resourceObjectWarpper))
                return;
            resourceObjectWarpper.ReferenceCount++;
        }
        /// <summary>
        /// 当资源包中的所有对象被加载
        /// </summary>
        void OnResourceBundleAllAssetLoad(string bundleName)
        {
            if (!resourceBundleWarpperDict.TryGetValue(bundleName, out var resourceBundleWarpper))
                return;
            var resourceObjectList = resourceBundleWarpper.ResourceBundle.ResourceObjectList;
            var length = resourceObjectList.Count;
            for (int i = 0; i < length; i++)
            {
                var resourceObject = resourceObjectList[i];
                OnResourceObjectLoad(resourceObject);
            }
            resourceBundleWarpper.ReferenceCount += resourceObjectList.Count;
        }
        /// <summary>
        /// 当资源对象被卸载
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
            UnloadDependenciesAssetBundle(resourceBundleWarpper, 1, ResourceDataProxy.UnloadAllLoadedObjectsWhenBundleUnload);
        }
        void OnResourceObjectNotExists(string assetName)
        {
            if (ResourceDataProxy.PrintLogWhenAssetNotExists)
            {
                Utility.Debug.LogError($"{assetName} not found!");
            }
        }
        void RequestManifestSuccess(ResourceRequestManifestSuccessEventArgs eventArgs)
        {
            var taskId = eventArgs.TaskId;
            if (TaskId != taskId)
                return;
            Reset();
            resourceManifest = eventArgs.ResourceManifest;
            var bundleBuildInfoDict = resourceManifest.ResourceBundleBuildInfoDict;
            var bundlePath = eventArgs.BundlePath;
            foreach (var bundleBuildInfo in bundleBuildInfoDict.Values)
            {
                var resourceBundle = bundleBuildInfo.ResourceBundle;
                var resourceBundleWarpper = new ResourceBundleWarpper(resourceBundle, bundlePath, bundleBuildInfo.BudleExtension, resourceManifest.BundleOffset);
                resourceBundleWarpperDict.TryAdd(resourceBundle.BundleName, resourceBundleWarpper);
                resourceBundleKeyDict.TryAdd(resourceBundle.BundleKey, resourceBundle.BundleName);
                var resourceObjectList = resourceBundle.ResourceObjectList;
                var objectLength = resourceObjectList.Count;
                for (int i = 0; i < objectLength; i++)
                {
                    var resourceObject = resourceObjectList[i];
                    resourceObjectWarpperDict.TryAdd(resourceObject.ObjectPath, new ResourceObjectWarpper(resourceObject));
                }
                resourceAddress.AddResourceObjects(resourceObjectList);
            }
            manifestAcquired = true;
            requestDone = true;
        }
        void RequestManifestFailure(ResourceRequestManifestFailureEventArgs eventArgs)
        {
            var taskId = eventArgs.TaskId;
            if (TaskId != taskId)
                return;
            Utility.Debug.LogError("ResourceManifest deserialization failed , check your file !");
            manifestAcquired = false;
            requestDone = true;
        }
    }
}