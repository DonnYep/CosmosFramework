using UnityEngine;
using Cosmos.Resource;
using System;
using Quark;
namespace Cosmos.Test
{
    public class QuarkLoader : IResourceLoadHelper
    {
        ///<inheritdoc/> 
        public bool IsProcessing { get { return isLoading; } }
        bool isLoading = false;
        ///<inheritdoc/> 
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> loadDoneCallback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return QuarkResources.LoadAssetAsync<T>(assetName, loadDoneCallback);
        }
        ///<inheritdoc/>
        public Coroutine LoadAssetAsync(string assetName, Type type, Action<UnityEngine.Object> callback, Action<float> progress = null)
        {
            return QuarkResources.LoadAssetAsync(assetName, callback);
        }
        ///<inheritdoc/>
        public Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return QuarkResources.LoadAssetWithSubAssetsAsync<T>(assetName, string.Empty, callback);
        }
        ///<inheritdoc/>
        public Coroutine LoadAssetWithSubAssetsAsync(string assetName, Type type, Action<UnityEngine.Object[]> callback, Action<float> progress = null)
        {
            throw new NotImplementedException();
        }
        ///<inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo assetName, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            return QuarkResources.LoadSceneAsync(assetName.SceneName, progressProvider, progress, condition, callback, assetName.Additive);
        }
        ///<inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo assetName, Action<float> progress, Func<bool> condition, Action callback)
        {
            return QuarkResources.UnloadSceneAsync(assetName.SceneName, progress, callback);
        }
        ///<inheritdoc/>
        public void UnloadAsset(string assetName)
        {
            QuarkResources.UnloadAsset(assetName);
        }
        ///<inheritdoc/>
        public void ReleaseAllAsset(bool unloadAllLoadedObjects = false)
        {
            QuarkResources.UnloadAllAssetBundle(unloadAllLoadedObjects);
        }
        ///<inheritdoc/> 
        public void Dispose()
        {
        }
        ///<inheritdoc/> 
        public Coroutine UnloadAllSceneAsync(Action<float> progress, Action callback)
        {
            return QuarkResources.UnloadAllSceneAsync(progress, callback);
        }
        ///<inheritdoc/> 
        public void ReleaseAsset(string assetName)
        {
        }
    }
}