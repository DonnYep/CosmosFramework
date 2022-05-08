using System;
using System.Collections;
using UnityEngine;
namespace Cosmos.Resource
{
    public class ResourcesLoader : IResourceLoadHelper
    {
        bool isProcessing = false;
        /// <summary>
        /// 单线下载等待
        /// </summary>
        private WaitUntil loadWait;
        public ResourcesLoader()
        {
            loadWait = new WaitUntil(() => { return !isProcessing; });
        }
        ///<inheritdoc/> 
        public bool IsProcessing { get { return isProcessing; } private set { isProcessing = value; } }
        ///<inheritdoc/> 
        public T LoadAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            var asset = Resources.Load<T>(info.AssetPath);
            if (asset == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {info.AssetPath}！");
            }
            return asset;
        }
        ///<inheritdoc/> 
        public T[] LoadAllAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            var asset = Resources.LoadAll<T>(info.AssetPath);
            if (asset == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {info.AssetPath}！");
            }
            return asset;
        }
        ///<inheritdoc/> 
        public T[] LoadAssetWithSubAssets<T>(AssetInfo info) where T : UnityEngine.Object
        {
            var assets = Resources.LoadAll<T>(info.AssetPath);
            if (assets == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {info.AssetPath}！");
            }
            return assets;
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(info, callback, progress));
        }
        ///<inheritdoc/> 
        public Coroutine LoadAssetWithSubAssetsAsync<T>(AssetInfo info, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssets(info, callback, progress));
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
        public void UnloadAsset(AssetInfo info)
        {
            Resources.UnloadUnusedAssets();
        }
        ///<inheritdoc/> 
        public void UnloadAllAsset(bool unloadAllLoadedObjects = false)
        {
            Resources.UnloadUnusedAssets();
        }
        IEnumerator EnumLoadAssetAsync<T>(AssetInfoBase info, Action<T> callback, Action<float> progress, bool instantiate = false)
            where T : UnityEngine.Object
        {
            if (isProcessing)
            {
                yield return loadWait;
            }
            UnityEngine.Object asset = null;
            ResourceRequest request = Resources.LoadAsync<T>(info.AssetPath);
            isProcessing = true;
            while (!request.isDone)
            {
                progress?.Invoke(request.progress);
                yield return null;
            }
            asset = request.asset;
            if (asset == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {info.AssetPath}！");
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
            isProcessing = false;
        }
        IEnumerator EnumLoadAssetWithSubAssets<T>(AssetInfoBase info, Action<T[]> callback, Action<float> progress)
            where T : UnityEngine.Object
        {
            T[] assets = null;
            assets = Resources.LoadAll<T>(info.AssetPath);
            isProcessing = true;
            yield return null;
            progress?.Invoke(1);
            if (assets == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {info.AssetPath}！");
            }
            else
            {
                callback?.Invoke(assets);
            }
            isProcessing = false;
        }
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (isProcessing)
            {
                yield return loadWait;
            }
            IsProcessing = true;
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
            IsProcessing = false;
        }
        IEnumerator EnumUnloadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            IsProcessing = true;
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
            IsProcessing = false;
        }
    }
}
