using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Cosmos.Resource
{
    public class AssetDatabaseLoader : IResourceLoadHelper
    {
        /// <summary>
        /// 指定工作目录
        /// </summary>
        readonly string rootPath;
        bool isLoading;
        public AssetDatabaseLoader(string rootPath)
        {
            this.rootPath = rootPath;
        }

        public bool IsLoading { get { return IsLoading; } }
        public T LoadAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            if (info == null)
                return null;
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(info.AssetPath))
                throw new ArgumentNullException("Asset path is invalid!");
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(info.AssetPath);
            return asset;
#else
            return null;    
#endif
        }
        public T[] LoadAllAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            if (info == null)
                return null;
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(info.AssetPath))
                throw new ArgumentNullException("Asset path is invalid!");
            var asset = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(info.AssetPath);
            var arr = asset.Where(a => a.GetType() == typeof(T)).ToArray();
            T[] t_arr = new T[arr.Length];
            for (int i = 0; i < t_arr.Length; i++)
            {
                t_arr[i] = (T)arr[i];
            }
            return t_arr;
#else
                return null;
#endif
        }
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return null;
#else
            return null;

#endif
        }
        public T[] LoadAssetWithSubAssets<T>(AssetInfo info) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(info.AssetPath))
                throw new ArgumentNullException("Asset name is invalid!");
            var assetObj = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(info.AssetPath);
            var length = assetObj.Length;
            T[] assets = new T[length];
            for (int i = 0; i < length; i++)
            {
                assets[i] = assetObj[i] as T;
            }
            return assets;
#else
            return null;

#endif
        }
        public Coroutine LoadAssetWithSubAssetsAsync<T>(AssetInfo info, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return null;
#else
            return null;

#endif
        }
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action callback, Action<float> progress = null)
        {
#if UNITY_EDITOR
            return null;

#else
            return null;

#endif
        }
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
#if UNITY_EDITOR

#else

#endif
        }
        public void UnLoadAsset(AssetInfo info)
        {
#if UNITY_EDITOR
#else

#endif
        }
    }
}
