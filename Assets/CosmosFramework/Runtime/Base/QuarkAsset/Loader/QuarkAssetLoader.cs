using Quark.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public abstract T LoadAsset<T>(string assetName, string assetExtension) where T : UnityEngine.Object;
        public abstract GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false);
        public abstract T[] LoadAssetWithSubAssets<T>(string assetName, string assetExtension) where T : UnityEngine.Object;
        public abstract Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback) where T : UnityEngine.Object;
        public abstract Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false);
        public abstract Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback) where T : UnityEngine.Object;
        public abstract Coroutine LoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive = false);
        public abstract void UnloadAsset(string assetName, string assetExtension);
        public abstract void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false);
        public abstract void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false);
        public abstract Coroutine UnLoadSceneAsync(string sceneName, Action<float> progress, Action callback);
        public abstract Coroutine UnLoadAllSceneAsync(Action<float> progress, Action callback);
        public bool GetInfo<T>(string assetName, string assetExtension, out QuarkAssetObjectInfo info) where T : UnityEngine.Object
        {
            info = QuarkAssetObjectInfo.None;
            var hasWapper = GetAssetObjectWapper<T>(assetName, assetExtension, out var wapper);
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
        protected bool GetAssetObjectWapper<T>(string assetName, string assetExtension, out QuarkAssetObjectWapper wapper)
        {
            wapper = null;
            var typeString = typeof(T).ToString();
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
    }
}
