using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// AB加载方案
    /// </summary>
    public class AssetBundleLoader : IResourceLoadHelper
    {
        /// <summary>
        /// name===resourceObject
        /// </summary>
        Dictionary<string, ResourceObject> resourceObjectDict;
        /// <summary>
        /// abName===resourceBundle
        /// </summary>
        Dictionary<string, ResourceBundle> resourceBundleDict;

        public AssetBundleLoader()
        {
            resourceBundleDict = new Dictionary<string, ResourceBundle>();
            resourceObjectDict = new Dictionary<string, ResourceObject>();
        }

        public bool IsProcessing => throw new NotImplementedException();

        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public Coroutine LoadAssetAsync(string assetName, Type type, Action<UnityEngine.Object> callback, Action<float> progress = null)
        {
            throw new NotImplementedException();
        }

        public Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public Coroutine LoadAssetWithSubAssetsAsync(string assetName, Type type, Action<UnityEngine.Object[]> callback, Action<float> progress = null)
        {
            throw new NotImplementedException();
        }

        public Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            throw new NotImplementedException();
        }

        public void UnloadAllAsset(bool unloadAllLoadedObjects = false)
        {
            throw new NotImplementedException();
        }

        public void UnloadAsset(string assetName)
        {
            throw new NotImplementedException();
        }

        public Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 增加一个引用计数
        /// </summary>
        void IncrementReferenceCount(string assetName)
        {

        }
        /// <summary>
        /// 减少一个引用计数
        /// </summary>
        void DecrementReferenceCount(string assetName)
        {

        }
    }
}
