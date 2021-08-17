using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.Resource;
using System;
using Quark.Asset;
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
    public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null) where T : UnityEngine.Object
    {
        return QuarkResources.LoadAssetAsync<T>(info.AssetPath, loadDoneCallback);
    }
    public Coroutine LoadSceneAsync(SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null)
    {
        return QuarkResources.LoadSceneAsync(info.AssetPath, loadingCallback,loadDoneCallback);
    }
    public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
    {
    }
    public void UnLoadAsset(object customData, bool unloadAllLoadedObjects = false)
    {
    }
}
