using Quark.Asset;
using System;
using UnityEngine;
namespace Quark.Loader
{
    /// <summary>
    /// Runtime加载时的方案；
    /// <see cref="QuarkAssetLoadMode"/>
    /// </summary>
    public interface IQuarkAssetLoader
    {
        void SetLoaderData(IQuarkLoaderData  loaderData);
        T LoadAsset<T>(string assetName, string assetExtension) where T : UnityEngine.Object;
        GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false);
        T[] LoadAssetWithSubAssets<T>(string assetName, string assetExtension) where T : UnityEngine.Object;
        Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback) where T : UnityEngine.Object;
        Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false);
        Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension,Action<T[]>callback) where T : UnityEngine.Object;
        Coroutine LoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive = false);
        void UnloadAsset(string assetName, string assetExtension);
        void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false);
        void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false);
        Coroutine UnLoadSceneAsync(string sceneName, Action<float> progress, Action callback);
        Coroutine UnLoadAllSceneAsync( Action<float> progress, Action callback);
        QuarkAssetObjectInfo GetInfo<T>(string assetName, string assetExtension) where T : UnityEngine.Object;
        QuarkAssetObjectInfo GetInfo(string assetName, string assetExtension);
        QuarkAssetObjectInfo[] GetAllLoadedInfos();
    }
}
