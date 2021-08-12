using Quark.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Quark
{
    public sealed class QuarkResources
    {
        public static T LoadAsset<T>(string assetName, string assetExtension = null)
where T : UnityEngine.Object
        {
            return QuarkManager.Instance.LoadAsset<T>(assetName, assetExtension);
        }
        public static Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback)
where T : UnityEngine.Object
        {
            return QuarkManager.Instance.LoadAssetAsync<T>(assetName, callback);
        }
        public static Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback)
where T : UnityEngine.Object
        {
            return QuarkManager.Instance.LoadAssetAsync<T>(assetName, assetExtension, callback);
        }
        public static Coroutine LoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive = false)
        {
            return QuarkManager.Instance.LoadSceneAsync(sceneName, progress, callback, additive);
        }
        public static GameObject LoadPrefab(string assetName, string assetExtension = null)
        {
            return QuarkManager.Instance.LoadAsset<GameObject>(assetName, assetExtension);
        }
        public static GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false)
        {
            return QuarkManager.Instance.LoadAsset<GameObject>(assetName, assetExtension, instantiate);
        }
        public static Coroutine LoadPrefabAsync(string assetName, Action<GameObject> callback, bool instantiate = false)
        {
            return QuarkManager.Instance.LoadAssetAsync<GameObject>(assetName, callback, instantiate);
        }
        public static Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false)
        {
            return QuarkManager.Instance.LoadAssetAsync<GameObject>(assetName, assetExtension, callback, instantiate);
        }
        public static void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false)
        {
            QuarkManager.Instance.UnLoadAllAssetBundle(unloadAllLoadedObjects);
        }
        public static void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            QuarkManager.Instance.UnLoadAssetBundle(assetBundleName, unloadAllLoadedObjects);
        }
         public static QuarkObjectInfo GetInfo<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
            return QuarkManager.Instance.GetInfo<T>(assetName,assetExtension);
        }
        public static QuarkObjectInfo[] GetAllInfos()
        {
            return QuarkManager.Instance.GetAllInfos();
        }
    }
}
