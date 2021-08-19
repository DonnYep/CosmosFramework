using Quark.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Quark.Loader
{
    /// <summary>
    /// Runtime加载时的方案；
    /// <see cref="QuarkAssetLoadMode"/>
    /// </summary>
    public interface IQuarkAssetLoader
    {
        void SetLoaderData(object customeData);
        T LoadAsset<T>(string assetName, string assetExtension) where T : UnityEngine.Object;
        GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false);
        Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback) where T : UnityEngine.Object;
        Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false);
        Coroutine LoadScenetAsync(string sceneName, Action<float> progress, Action callback, bool additive = false);
        void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false);
        void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false);
        QuarkObjectInfo GetInfo<T>(string assetName, string assetExtension) where T : UnityEngine.Object;
        QuarkObjectInfo[] GetAllInfos();
    }
}
