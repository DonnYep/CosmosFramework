using UnityEngine;
using Cosmos.ObjectPool;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
using Cosmos.Resource;
using Cosmos.UI;
using System;

namespace Cosmos.Extensions
{
    //async/await在线上存在奔溃，因此隔离Coroutine与Task，保持module的纯净

    public static class CosmosExtensions 
{
        public static async Task<IObjectPool> RegisterObjectPoolAsync(this IObjectPoolManager @this, ObjectPoolAssetInfo assetInfo)
        {
            IObjectPool pool = null;
            await @this.RegisterObjectPoolAsync(assetInfo, (p) => { pool = p; });
            return pool;
        }

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
        public static async Task LoadSceneAsync(this IResourceManager @this, SceneAssetInfo sceneAssetInfo)
        {
            await @this.LoadSceneAsync(sceneAssetInfo, null, null, null, null);
        }
        public static async Task LoadSceneAsync(this IResourceManager @this, SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition)
        {
            await @this.LoadSceneAsync(sceneAssetInfo, progressProvider, progress, condition, null);
        }
        public static async Task UnloadSceneAsync(this IResourceManager @this, SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition)
        {
            await @this.UnloadSceneAsync(sceneAssetInfo, progress, condition, null);
        }

        public static async Task<T> OpenUIFormAsync<T>(this IUIManager @this, UIAssetInfo assetInfo)
    where T : class, IUIForm
        {
            T uiForm = null;
            await @this.OpenUIFormAsync<T>(assetInfo, pnl => uiForm = pnl);
            return uiForm;
        }
        public static async Task<IUIForm> OpenUIFormAsync(this IUIManager @this, UIAssetInfo assetInfo, Type uiType)
        {
            IUIForm uiForm = null;
            await @this.OpenUIFormAsync(assetInfo, uiType, pnl => uiForm = pnl);
            return uiForm;
        }
        public static async Task<IUIForm> PreloadUIFormAsync(this IUIManager @this, UIAssetInfo assetInfo, Type uiType)
        {
            IUIForm uiForm = null;
            await @this.PreloadUIFormAsync(assetInfo, uiType, pnl => uiForm = pnl);
            return uiForm;
        }
        public static async Task<T> PreloadUIFormAsync<T>(this IUIManager @this, UIAssetInfo assetInfo)
    where T : class, IUIForm
        {
            T uiForm = null;
            await @this.PreloadUIFormAsync<T>(assetInfo, pnl => uiForm = pnl);
            return uiForm;
        }
    }
}
