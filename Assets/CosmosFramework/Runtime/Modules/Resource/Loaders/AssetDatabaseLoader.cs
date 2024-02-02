using Cosmos.Resource.State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    public class AssetDatabaseLoader : IResourceLoadHelper
    {
        /// <summary>
        /// assetPath===resourceObjectWarpper
        /// <para>理论上资源地址在unity中应该是唯一的</para> 
        /// <para>资源地址相同但文件bytes内容改变，打包时生成的hash也会与之不同。因此理论上应该是assetPath是唯一的</para>
        /// </summary>
        readonly Dictionary<string, ResourceObjectWarpper> resourceObjectWarpperDict;
        /// <summary>
        /// bundleName===resourceBundleWarpper
        /// </summary>
        readonly Dictionary<string, ResourceBundleWarpper> resourceBundleWarpperDict;
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
        readonly List<ResourceBundleState> bundleStateCache = new List<ResourceBundleState>();
        public AssetDatabaseLoader()
        {
            loadSceneList = new List<string>();
            resourceAddress = new ResourceAddress();
            resourceBundleWarpperDict = new Dictionary<string, ResourceBundleWarpper>();
            resourceObjectWarpperDict = new Dictionary<string, ResourceObjectWarpper>();
            loadedSceneDict = new Dictionary<string, UnityEngine.SceneManagement.Scene>();
        }
        ///<inheritdoc/> 
        public void OnInitialize()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        public void SetResourceDataset(ResourceDataset resourceDataset)
        {
            var resourceBundleInfoList = resourceDataset.ResourceBundleInfoList;
            foreach (var resourceBundleInfo in resourceBundleInfoList)
            {
                var resourceBundle = new ResourceBundle()
                {
                    BundleKey = resourceBundleInfo.BundleKey,
                    BundleName = resourceBundleInfo.BundleName,
                    BundlePath = resourceBundleInfo.BundlePath,
                };
                resourceBundle.BundleDependencies.AddRange(resourceBundleInfo.BundleDependencies);
                var resourceObjectInfoList = resourceBundleInfo.ResourceObjectInfoList;
                var objectLength = resourceObjectInfoList.Count;
                for (int i = 0; i < objectLength; i++)
                {
                    var resourceObjectInfo = resourceObjectInfoList[i];
                    var resourceObject = new ResourceObject()
                    {
                        ObjectName = resourceObjectInfo.ObjectName,
                        ObjectPath = resourceObjectInfo.ObjectPath,
                        BundleName = resourceObjectInfo.BundleName,
                        Extension = resourceObjectInfo.Extension
                    };
                    resourceObjectWarpperDict.TryAdd(resourceObjectInfo.ObjectPath, new ResourceObjectWarpper(resourceObject));
                    resourceAddress.AddResourceObject(resourceObject);
                    resourceBundle.ResourceObjectList.Add(resourceObject);
                }
                resourceBundleWarpperDict.TryAdd(resourceBundleInfo.BundleName, new ResourceBundleWarpper(resourceBundle));
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
                UnloadDependenciesAssetBundle(bundleWarpper, bundleWarpper.ReferenceCount);
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
            return new ResourceVersion("ResourceDataset", "ResourceDataset for runtime ,editor only");
        }
        ///<inheritdoc/> 
        public void Reset()
        {
            resourceObjectWarpperDict.Clear();
            resourceBundleWarpperDict.Clear();
            loadSceneList.Clear();
            resourceAddress.Clear();
            loadedSceneDict.Clear();
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
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(resourceObject.ObjectPath))
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath(resourceObject.ObjectPath, type);
            if (asset != null)
            {
                OnResourceObjectLoad(resourceObject);
            }
            else
            {
                OnResourceObjectNotExists(assetName);
            }
#endif
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
#if UNITY_EDITOR
            assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(resourceObject.ObjectPath);
            if (assets != null)
            {
                OnResourceObjectLoad(resourceObject);
            }
            else
            {
                OnResourceObjectNotExists(assetName);
            }
#endif
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
                var hasObject = resourceObjectWarpperDict.TryGetValue(resourceObject.ObjectName, out var objectWarpper);
                if (hasObject)
                {
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(resourceObject.ObjectPath, typeof(Object));
                    assetList.Add(asset);
                }
                else
                {
                    OnResourceObjectNotExists(resourceObject.ObjectName);
                }
            }
            assets = assetList.ToArray();
            OnResourceBundleAllAssetLoad(bundleName);
#endif
            progress?.Invoke(1);
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
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
            AsyncOperation operation = null;
#if UNITY_EDITOR
            operation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(resourceObject.ObjectPath, new LoadSceneParameters(loadSceneMode));
#else
            operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
#endif
            if (operation == null)
            {
                //为空表示场景不存在
                progress?.Invoke(1);
                callback?.Invoke();
                OnResourceObjectNotExists(sceneName);
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
            yield return null;
            if (condition != null)
                yield return new WaitUntil(condition);
            operation.allowSceneActivation = true;
            yield return operation.isDone;
            yield return null;
            callback?.Invoke();
        }
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="sceneAssetInfo">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        IEnumerator EnumUnloadSceneAsync(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback)
        {
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
                yield break; //若bundle信息为空，终止
            bundleWarpper.ReferenceCount++; //AB包引用计数增加
            var bundleDependencies = bundleWarpper.ResourceBundle.BundleDependencies;
            var bundleDependenciesLength = bundleDependencies.Count;
            for (int i = 0; i < bundleDependenciesLength; i++)
            {
                var bundleDependency = bundleDependencies[i];
                if (resourceBundleWarpperDict.TryGetValue(bundleDependency.BundleKey, out var dependentBundleWarpper))
                {
                    dependentBundleWarpper.ReferenceCount++;
                }
            }
        }
        /// <summary>
        /// 递归减少包体引用计数
        /// </summary>
        void UnloadDependenciesAssetBundle(ResourceBundleWarpper resourceBundleWarpper, int decrementCount = 1)
        {
            resourceBundleWarpper.ReferenceCount -= decrementCount;
            var bundleDependencies = resourceBundleWarpper.ResourceBundle.BundleDependencies;
            var bundleDependenciesLength = bundleDependencies.Count;
            //遍历查询依赖包
            for (int i = 0; i < bundleDependenciesLength; i++)
            {
                var bundleDependency = bundleDependencies[i];
                if (resourceBundleWarpperDict.TryGetValue(bundleDependency.BundleKey, out var dependentBundleWarpper))
                {
                    dependentBundleWarpper.ReferenceCount -= decrementCount;
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
                UnloadDependenciesAssetBundle(bundleWarpper, count);
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
            UnloadDependenciesAssetBundle(resourceBundleWarpper);
        }
        void OnResourceObjectNotExists(string assetName)
        {
            if (ResourceDataProxy.PrintLogWhenAssetNotExists)
            {
                Utility.Debug.LogError($"{assetName} not found!");
            }
        }
    }
}
