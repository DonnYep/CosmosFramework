using System;
using System.Threading.Tasks;
using Cosmos.Resource;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos.Extensions
{
    //async/await在线上存在奔溃，因此隔离Coroutine与Task，保持module的纯净
    public static class ResourceManagerExts
    {
        public static async Task<T> LoadAssetAsync<T>(this IResourceManager @this, string assetName)
            where T : Object
        {
            T asset = null;
            await @this.LoadAssetAsync<T>(assetName, a => asset = a, null);
            return asset;
        }
        public static async Task<Object> LoadAssetAsync(this IResourceManager @this, string assetName, Type type)
        {
            Object asset = null;
            await @this.LoadAssetAsync(assetName, type, a => asset = a, null);
            return asset;
        }
        public static async Task<GameObject> LoadPrefabAsync(this IResourceManager @this, string assetName, bool instantiate = false)
        {
            GameObject go = null;
            await @this.LoadAssetAsync<GameObject>(assetName, (asset) =>
            {
                if (instantiate)
                {
                    if (asset != null)
                        go = GameObject.Instantiate(asset);
                }
                else
                    go = asset;
            }, null);
            return go;
        }
        public static async Task<Object[]> LoadAllAssetAsync(this IResourceManager @this, string assetPack, Action<float> progress = null)
        {
            Object[] assets = null;
            await @this.LoadAllAssetAsync(assetPack, (a) => { assets = a; }, progress);
            return assets;
        }
        public static async Task LoadSceneAsync(this IResourceManager @this, SceneAssetInfo info)
        {
            await @this.LoadSceneAsync(info, null, null, null, null);
        }
        public static async Task LoadSceneAsync(this IResourceManager @this, SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition)
        {
            await @this.LoadSceneAsync(info, progressProvider, progress, condition, null);
        }
        public static async Task UnloadSceneAsync(this IResourceManager @this, SceneAssetInfo info, Action<float> progress, Func<bool> condition)
        {
            await @this.UnloadSceneAsync(info, progress, condition, null);
        }
    }
}
