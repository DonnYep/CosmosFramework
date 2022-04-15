using UnityEngine;
using Cosmos.Resource;
using System;
using Quark;

public class QuarkLoader : IResourceLoadHelper
{
    public bool IsLoading { get { return isLoading; } }
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
    public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
    {
        QuarkResources.UnLoadAllAssetBundle(unloadAllLoadedObjects);
    }
    public void UnLoadAsset(AssetInfo info)
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
}
