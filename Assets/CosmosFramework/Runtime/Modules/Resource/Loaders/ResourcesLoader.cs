using System;
using System.Collections;
using UnityEngine;
namespace Cosmos.Resource
{
    public class ResourcesLoader : IResourceLoadHelper
    {
        public bool IsLoading { get { return isLoading; } }
        bool isLoading = false;
        public T[] LoadAllAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            var asset = Resources.LoadAll<T>(info.AssetPath);
            if (asset == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {info.AssetPath}！");
            }
            return asset;
        }
        public T LoadAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            var asset = Resources.Load<T>(info.AssetPath);
            if (asset == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {info.AssetPath}！");
            }
            return asset;
        }
        public T[] LoadAssetWithSubAssets<T>(AssetInfo info) where T : UnityEngine.Object
        {
            var assets = Resources.LoadAll<T>(info.AssetPath);
            if (assets == null)
            {
                throw new ArgumentNullException($"Resources文件夹中不存在资源 {info.AssetPath}！");
            }
            return assets;
        }
        public Coroutine LoadAssetWithSubAssetsAsync<T>(AssetInfo info, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssets(info, callback, progress));
        }
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(info, callback, progress));
        }
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action callback, Action<float> progress = null)
        {
            return null;
        }
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            Resources.UnloadUnusedAssets();
        }
        public void UnLoadAsset(AssetInfo info)
        {
            Resources.UnloadUnusedAssets();
        }
        IEnumerator EnumLoadAssetWithSubAssets<T>(AssetInfoBase info, Action<T[]> callback, Action<float> progress)
            where T : UnityEngine.Object
        {
            T[] assets = null;
            assets = Resources.LoadAll<T>(info.AssetPath);
            isLoading = true;
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
            isLoading = false;
        }
        IEnumerator EnumLoadAssetAsync<T>(AssetInfoBase info, Action<T> callback, Action<float> progress, bool instantiate = false)
            where T : UnityEngine.Object
        {
            UnityEngine.Object asset = null;
            ResourceRequest request = Resources.LoadAsync<T>(info.AssetPath);
            isLoading = true;
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
            isLoading = false;
        }

    }
}
