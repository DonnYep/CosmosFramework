using UnityEngine;
using Cosmos.Resource;
using System;
using Quark;

public class QuarkLoader : IResourceLoadHelper
{
    public bool IsProcessing { get { return isLoading; } }
    bool isLoading = false;

    public T[] LoadAllAsset<T>(AssetInfo info) where T : UnityEngine.Object
    {
        return null;
    }
    public T LoadAsset<T>(AssetInfo info) where T : UnityEngine.Object
    {
        return QuarkResources.LoadAsset<T>(info.AssetPath);
    }
    public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback, Action<float> progress = null) where T : UnityEngine.Object
    {
        return QuarkResources.LoadAssetAsync<T>(info.AssetPath, loadDoneCallback);
    }
    public Coroutine LoadSceneAsync(SceneAssetInfo info, Action loadDoneCallback, Action<float> progress = null)
    {
        return QuarkResources.LoadSceneAsync(info.AssetPath, progress, loadDoneCallback);
    }
    public void UnloadAllAsset(bool unloadAllLoadedObjects = false)
    {
        QuarkResources.UnloadAllAssetBundle(unloadAllLoadedObjects);
    }
    public void UnloadAsset(AssetInfo info)
    {
        QuarkResources.UnloadAsset(info.AssetPath);
    }
    public T[] LoadAssetWithSubAssets<T>(AssetInfo info) where T : UnityEngine.Object
    {
        return QuarkResources.LoadAssetWithSubAssets<T>(info.AssetPath);
    }
    public Coroutine LoadAssetWithSubAssetsAsync<T>(AssetInfo info, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
    {
        return QuarkResources.LoadAssetWithSubAssetsAsync<T>(info.AssetPath, string.Empty, callback);
    }
    public Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
    {
        return QuarkResources.LoadSceneAsync(info.AssetPath, progressProvider, progress, condition, callback, info.Additive);
    }
    public Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
    {
        return QuarkResources.UnloadSceneAsync(info.AssetPath, progress, callback);
    }
}
