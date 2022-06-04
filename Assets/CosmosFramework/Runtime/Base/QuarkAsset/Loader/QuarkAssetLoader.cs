using Quark.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
namespace Quark.Loader
{
    /// <summary>
    /// Runtime加载时的方案；
    /// <see cref="QuarkAssetLoadMode"/>
    /// </summary>
    internal abstract class QuarkAssetLoader
    {
        /// <summary>
        /// Key : [ABName] ; Value : [QuarkAssetBundle]
        /// </summary>
        protected Dictionary<string, QuarkAssetBundle> assetBundleDict = new Dictionary<string, QuarkAssetBundle>();
        /// <summary>
        /// Key : AssetName---Value :  Lnk [QuarkAssetABObjectWapper]
        /// </summary>` 
        protected Dictionary<string, LinkedList<QuarkAssetObjectWapper>> quarkAssetObjectDict
            = new Dictionary<string, LinkedList<QuarkAssetObjectWapper>>();

        /// <summary>
        /// Hash===QuarkAssetObjectInfo
        /// </summary>
        protected Dictionary<int, QuarkAssetObjectInfo> hashQuarkAssetObjectInfoDict = new Dictionary<int, QuarkAssetObjectInfo>();
        /// <summary>
        /// 被加载的场景字典；
        /// SceneName===Scene
        /// </summary>
        protected Dictionary<string, Scene> loadedSceneDict = new Dictionary<string, Scene>();
        public abstract void SetLoaderData(IQuarkLoaderData loaderData);
        public abstract T LoadAsset<T>(string assetName, string assetExtension) where T : Object;
        public abstract Object LoadAsset(string assetName, string assetExtension, Type type);
        public abstract GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false);
        public abstract T[] LoadAssetWithSubAssets<T>(string assetName, string assetExtension) where T : Object;
        public abstract Object[] LoadAssetWithSubAssets(string assetName, string assetExtension, Type type);
        public abstract Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback) where T : Object;
        public abstract Coroutine LoadAssetAsync(string assetName, string assetExtension, Type type, Action<Object> callback);
        public abstract Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false);
        public abstract Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback) where T : Object;
        public abstract Coroutine LoadAssetWithSubAssetsAsync(string assetName, string assetExtension, Type type, Action<Object[]> callback);
        public abstract Coroutine LoadSceneAsync(string sceneName, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback, bool additive = false);
        public abstract void UnloadAsset(string assetName, string assetExtension);
        public abstract void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false);
        public abstract void UnloadAllAssetBundle(bool unloadAllLoadedObjects = false);
        public abstract Coroutine UnloadSceneAsync(string sceneName, Action<float> progress, Action callback);
        public abstract Coroutine UnloadAllSceneAsync(Action<float> progress, Action callback);
        public bool GetInfo<T>(string assetName, string assetExtension, out QuarkAssetObjectInfo info) where T : Object
        {
            info = QuarkAssetObjectInfo.None;
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, typeof(T), out var wapper);
            if (hasWapper)
            {
                info = wapper.GetQuarkAssetObjectInfo();
                return true;
            }
            return false;
        }
        public bool GetInfo(string assetName, string assetExtension, Type type, out QuarkAssetObjectInfo info)
        {
            info = QuarkAssetObjectInfo.None;
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, type, out var wapper);
            if (hasWapper)
            {
                info = wapper.GetQuarkAssetObjectInfo();
                return true;
            }
            return false;
        }
        public bool GetInfo(string assetName, string assetExtension, out QuarkAssetObjectInfo info)
        {
            info = QuarkAssetObjectInfo.None;
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                info = wapper.GetQuarkAssetObjectInfo();
                return true;
            }
            return false;
        }
        public QuarkAssetObjectInfo[] GetAllLoadedInfos()
        {
            return hashQuarkAssetObjectInfoDict.Values.ToArray();
        }
        protected bool GetAssetObjectWapper(string assetName, string assetExtension, Type type, out QuarkAssetObjectWapper wapper)
        {
            wapper = null;
            var typeString = type.ToString();
            if (quarkAssetObjectDict.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    var obj = abLnk.First.Value;
                    if (obj.QuarkAssetObject.AssetType == typeString)
                        wapper = abLnk.First.Value;
                }
                else
                {
                    foreach (var abWapper in abLnk)
                    {
                        if (abWapper.QuarkAssetObject.AssetExtension == assetExtension && abWapper.QuarkAssetObject.AssetType == typeString)
                        {
                            wapper = abWapper;
                            break;
                        }
                    }
                }
            }
            return wapper != null;
        }
        protected bool GetAssetObjectWapper(string assetName, string assetExtension, out QuarkAssetObjectWapper wapper)
        {
            wapper = null;
            if (quarkAssetObjectDict.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    wapper = abLnk.First.Value;
                }
                else
                {
                    foreach (var abWapper in abLnk)
                    {
                        if (abWapper.QuarkAssetObject.AssetExtension == assetExtension)
                        {
                            wapper = abWapper;
                            break;
                        }
                    }
                }
            }
            return wapper != null;
        }
        protected IEnumerator EnumUnloadSceneAsync(string sceneName, Action<float> progress, Action callback)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                QuarkUtility.LogError("Scene name is invalid!");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            if (!loadedSceneDict.TryGetValue(sceneName, out var scene))
            {
                QuarkUtility.LogError($"Unload scene failure： {sceneName}  not loaded yet !");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            var hasWapper = GetAssetObjectWapper(sceneName, ".unity", out var wapper);
            if (hasWapper)
            {
                QuarkUtility.LogError($"Scene：{sceneName}.unity not existed !");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            var ao = SceneManager.UnloadSceneAsync(scene);
            while (!ao.isDone)
            {
                progress?.Invoke(ao.progress);
                yield return null;
            }
            loadedSceneDict.Remove(sceneName);
            DecrementQuarkAssetObject(wapper);
            progress?.Invoke(1);
            callback?.Invoke();
        }
        /// <summary>
        /// 增加一个引用计数；
        /// </summary>
        protected abstract void IncrementQuarkAssetObject(QuarkAssetObjectWapper wapper);
        /// <summary>
        /// 减少一个引用计数；
        /// </summary>
        protected abstract void DecrementQuarkAssetObject(QuarkAssetObjectWapper wapper);
    }
}
