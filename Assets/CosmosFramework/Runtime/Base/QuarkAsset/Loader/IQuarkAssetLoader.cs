using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Quark.Loader
{
    /// <summary>
    /// Runtime加载时的方案；
    /// <see cref="QuarkAssetLoadMode"/>
    /// </summary>
    public interface IQuarkAssetLoader
    {
        void SetLoaderData(object customeData);
        T LoadAsset<T>(string assetName, string assetExtension, bool instantiate = false) where T : UnityEngine.Object;
        Coroutine LoadScenetAsync(string sceneName, Action<float> progress, Action callback, bool additive = false);
        Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object;
        Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object;
        void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false);
        void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false);
        QuarkObjectInfo GetInfo<T>(string assetName, string assetExtension) where T : UnityEngine.Object;
        QuarkObjectInfo[] GetAllInfos();
    }
}
