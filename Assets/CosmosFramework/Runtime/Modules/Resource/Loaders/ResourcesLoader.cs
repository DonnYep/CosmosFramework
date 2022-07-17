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
        public ResourcesLoader()
        {
            loadedSceneDict = new Dictionary<string, UnityEngine.SceneManagement.Scene>();
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, type, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadMainAndSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssets(assetName, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetWithSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssets(assetName, type, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAllAssetAsync(string assetBundleName, Action<Object[]> callback, Action<float> progress = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadAllAssetAsync(assetBundleName,progress,callback));
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
        }
        ///<inheritdoc/> 
        public void ReleaseAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
        }
        ///<inheritdoc/> 
        public void ReleaseAllAsset(bool unloadAllLoadedObjects = false)
        {
            Resources.UnloadUnusedAssets();
        }
        ///<inheritdoc/> 
        public void Dispose()
        {
        }
        ///<inheritdoc/> 
        public void ReleaseAsset(string assetName)
        {
        }
        IEnumerator EnumLoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress, bool instantiate = false)
            where T : UnityEngine.Object
        {
            UnityEngine.Object asset = null;
            ResourceRequest request = Resources.LoadAsync<T>(assetName);
            while (!request.isDone)
            {
                progress?.Invoke(request.progress);
                yield return null;
            }
            asset = request.asset;
            if (asset == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {assetName}！");
            }
            else
            {
                if (instantiate)
                {
                    asset = GameObject.Instantiate(asset);
                }
            }
            if (asset != null)
            {
                callback?.Invoke(asset as T);
            }
        }
        IEnumerator EnumLoadAssetWithSubAssets<T>(string assetName, Action<T[]> callback, Action<float> progress)
            where T : UnityEngine.Object
        {
            T[] assets = null;
            assets = Resources.LoadAll<T>(assetName);
            yield return null;
            progress?.Invoke(1);
            if (assets == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {assetName}！");
            }
            else
            {
                callback?.Invoke(assets);
            }
        }
        IEnumerator EnumLoadAssetAsync(string assetName, Type type, Action<UnityEngine.Object> callback, Action<float> progress, bool instantiate = false)
        {
            UnityEngine.Object asset = null;
            ResourceRequest request = Resources.LoadAsync(assetName, type);
            while (!request.isDone)
            {
                progress?.Invoke(request.progress);
                yield return null;
            }
            asset = request.asset;
            if (asset == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {assetName}！");
            }
            else
            {
                if (instantiate)
                {
                    asset = GameObject.Instantiate(asset);
                }
            }
            if (asset != null)
            {
                callback?.Invoke(asset);
            }
        }
        IEnumerator EnumLoadAssetWithSubAssets(string assetName, Type type, Action<UnityEngine.Object[]> callback, Action<float> progress)
        {
            UnityEngine.Object[] assets = null;
            assets = Resources.LoadAll(assetName);
            yield return null;
            progress?.Invoke(1);
            if (assets == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {assetName}！");
            }
            else
            {
                callback?.Invoke(assets);
            }
        }
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneInfo.SceneName, (UnityEngine.SceneManagement.LoadSceneMode)Convert.ToByte(sceneInfo.Additive));
            ao.allowSceneActivation = false;
            var hasProviderProgress = progressProvider != null;
            while (!ao.isDone)
            {
                if (hasProviderProgress)
                {
                    var providerProgress = progressProvider();
                    var sum = providerProgress + ao.progress;
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
        }
        IEnumerator EnumUnloadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            var ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneInfo.SceneName);
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
            callback?.Invoke();
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
                var ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene.Value);
                while (!ao.isDone)
                {
                    overallProgress = overallIndexPercent + (unitResRatio * ao.progress);
                    progress?.Invoke(overallProgress / 100);
                    yield return null;
                }
                overallProgress = overallIndexPercent + (unitResRatio * 1);
                progress?.Invoke(overallProgress / 100);
            }
            loadedSceneDict.Clear();
            progress?.Invoke(1);
            callback?.Invoke();
        }
        IEnumerator EnumLoadAllAssetAsync(string assetBundleName, Action<float> progress, Action<Object[]> callback)
        {
            if (string.IsNullOrEmpty(assetBundleName))
                yield break;
            var assets= Resources.LoadAll(assetBundleName);
            callback?.Invoke(assets);
            progress?.Invoke(1);
        }
        void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
        {
            loadedSceneDict.Remove(scene.name);
        }
    }
}
