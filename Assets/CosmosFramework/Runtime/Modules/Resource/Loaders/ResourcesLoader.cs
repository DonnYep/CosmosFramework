using Cosmos.Resource.State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
namespace Cosmos.Resource
{
    /// <summary>
    /// Resources加载模式不记录引用计数
    /// </summary>
    public class ResourcesLoader : IResourceLoadHelper
    {
        /// <summary>
        /// 被加载的场景字典；
        /// SceneName===Scene
        /// </summary>
        readonly Dictionary<string, UnityEngine.SceneManagement.Scene> loadedSceneDict;
        /// <summary>
        /// 主动加载的场景列表；
        /// </summary>
        readonly List<string> loadSceneList;
        public ResourcesLoader()
        {
            loadSceneList = new List<string>();
            loadedSceneDict = new Dictionary<string, UnityEngine.SceneManagement.Scene>();
        }
        public void OnInitialize()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
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
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssets(assetName, typeof(T), assets =>
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
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssets(assetName, type, callback, progress));
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
        }
        ///<inheritdoc/> 
        public void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects)
        {
        }
        ///<inheritdoc/> 
        public void UnloadAllAsset(bool unloadAllLoadedObjects)
        {
            Resources.UnloadUnusedAssets();
        }
        ///<inheritdoc/> 
        public bool GetBundleState(string bundleName, out ResourceBundleState bundleState)
        {
            bundleState = ResourceBundleState.Default;
            return false;
        }
        ///<inheritdoc/> 
        public bool GetObjectState(string objectName, out ResourceObjectState objectState)
        {
            objectState = ResourceObjectState.Default;
            return false;
        }
        ///<inheritdoc/> 
        public ResourceBundleState[] GetLoadedBundleState()
        {
            return new ResourceBundleState[0];
        }
        ///<inheritdoc/> 
        public ResourceVersion GetResourceVersion()
        {
            return new ResourceVersion("Built-in_Resources", "Unity Built-in Resources");
        }
        ///<inheritdoc/> 
        public void Reset()
        {
            loadSceneList.Clear();
            loadedSceneDict.Clear();
        }
        ///<inheritdoc/> 
        public void OnTerminate()
        {
            loadSceneList.Clear();
            loadedSceneDict.Clear();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        ///<inheritdoc/> 
        public void ReleaseAsset(string assetName)
        {
        }
        IEnumerator EnumLoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress)
        {
            Object asset = null;
            ResourceRequest request = Resources.LoadAsync(assetName, type);
            while (!request.isDone)
            {
                progress?.Invoke(request.progress);
                yield return null;
            }
            progress?.Invoke(1);
            asset = request.asset;
            callback?.Invoke(asset);
        }
        IEnumerator EnumLoadAssetWithSubAssets(string assetName, Type type, Action<Object[]> callback, Action<float> progress)
        {
            Object[] assets = new Object[1];
            var asset = Resources.Load(assetName);
            assets[0] = asset;
            yield return null;
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
            var operation = SceneManager.LoadSceneAsync(sceneName, (LoadSceneMode)Convert.ToByte(sceneAssetInfo.Additive));
            if (operation == null)
            {
                //为空表示场景不存在
                progress?.Invoke(1);
                callback?.Invoke();
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
                yield return new WaitForEndOfFrame();
            }
            progress?.Invoke(1);
            yield return null;
            if (condition != null)
                yield return new WaitUntil(condition);
            operation.allowSceneActivation = true;
            yield return null;
            callback?.Invoke();
        }
        IEnumerator EnumUnloadSceneAsync(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback = null)
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
            var operation = SceneManager.UnloadSceneAsync(sceneName);
            while (!operation.isDone)
            {
                progress?.Invoke(operation.progress);
                if (operation.progress >= 0.9f)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
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
                var ao = SceneManager.UnloadSceneAsync(sceneName);
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
        IEnumerator EnumLoadAllAssetAsync(string assetBundleName, Action<float> progress, Action<Object[]> callback)
        {
            if (string.IsNullOrEmpty(assetBundleName))
                yield break;
            var assets = Resources.LoadAll(assetBundleName);
            progress?.Invoke(1);
            callback?.Invoke(assets);
        }
        void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
        {
            var sceneName = scene.name;
            loadSceneList.Remove(sceneName);
            loadedSceneDict.Remove(sceneName);
        }
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadSceneMode)
        {
            var sceneName = scene.name;
            if (loadSceneList.Contains(sceneName))
            {
                loadedSceneDict.TryAdd(sceneName, scene);
            }
        }
    }
}
