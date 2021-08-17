using Cosmos.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos
{
    public class ResourceLoader : IResourceLoadHelper
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
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(info, loadDoneCallback, loadingCallback));
        }
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null)
        {
            return null;
        }
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            Resources.UnloadUnusedAssets();
        }
        public void UnLoadAsset(object customData, bool unloadAllLoadedObjects = false)
        {
            Resources.UnloadUnusedAssets();
        }
        IEnumerator EnumLoadAssetAsync<T>(AssetInfoBase info, Action<T> loadDoneCallback, Action<float> loadingCallback, bool instantiate = false)
            where T : UnityEngine.Object
        {
            UnityEngine.Object asset = null;
            ResourceRequest request = Resources.LoadAsync<T>(info.AssetPath);
            isLoading = true;
            while (!request.isDone)
            {
                loadingCallback?.Invoke(request.progress);
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
                loadDoneCallback?.Invoke(asset as T);
            }
            isLoading = false;
        }
    }
}
