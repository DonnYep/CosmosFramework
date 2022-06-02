using UnityEngine;
using Cosmos.Resource;
using System;
using Quark;

public class QuarkLoader : IResourceLoadHelper
{
    public bool IsProcessing { get { return isLoading; } }
    bool isLoading = false;

    public T[] LoadAllAsset<T>(string assetName) where T : UnityEngine.Object
    {
        return null;
    }
    public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
    {
        return QuarkResources.LoadAsset<T>(assetName);
    }
    public Coroutine LoadAssetAsync<T>(string assetName, Action<T> loadDoneCallback, Action<float> progress = null) where T : UnityEngine.Object
    {
        return QuarkResources.LoadAssetAsync<T>(assetName, loadDoneCallback);
    }
    public Coroutine LoadSceneAsync(SceneAssetInfo assetName, Action loadDoneCallback, Action<float> progress = null)
    {
        return QuarkResources.LoadSceneAsync(assetName.SceneName, progress, loadDoneCallback);
    }
    public void UnloadAllAsset(bool unloadAllLoadedObjects = false)
    {
        QuarkResources.UnloadAllAssetBundle(unloadAllLoadedObjects);
    }
    public void UnloadAsset(string assetName)
    {
        QuarkResources.UnloadAsset(assetName);
    }
    public T[] LoadAssetWithSubAssets<T>(string assetName) where T : UnityEngine.Object
    {
        return QuarkResources.LoadAssetWithSubAssets<T>(assetName);
    }
    public Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
    {
        return QuarkResources.LoadAssetWithSubAssetsAsync<T>(assetName, string.Empty, callback);
    }
    public Coroutine LoadSceneAsync(SceneAssetInfo assetName, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
    {
        return QuarkResources.LoadSceneAsync(assetName.SceneName, progressProvider, progress, condition, callback, assetName.Additive);
    }
    public Coroutine UnloadSceneAsync(SceneAssetInfo assetName, Action<float> progress, Func<bool> condition, Action callback)
    {
        return QuarkResources.UnloadSceneAsync(assetName.SceneName, progress, callback);
    }
}
