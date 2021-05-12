using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Reflection;
namespace Cosmos.Resource
{
    [Module]
    internal sealed partial class ResourceManager : Module,IResourceManager
    {
        #region Properties
        public int ResourceLoadChannelCount { get { return channelDict.Count; } }
        Dictionary<byte, ResourceLoadChannel> channelDict;
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            channelDict = new Dictionary<byte, ResourceLoadChannel>();
        }
        public override void OnTermination()
        {
            ClearMemory();
        }
        public void AddLoadChannel(ResourceLoadChannel loadChannel)
        {
            channelDict.TryAdd(loadChannel.ResourceLoadChannelId, loadChannel);
        }
        public void RemoveLoadChannel(byte channelId)
        {
            channelDict.TryRemove(channelId, out _);
        }
        public bool HasLoadChannel(byte channelId)
        {
            return channelDict.ContainsKey(channelId);
        }
        /// <summary>
        /// 特性无效！；
        /// 加载资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        public T LoadAsset<T>(byte channelId, AssetInfo info)
            where T : UnityEngine.Object
        {
            T asset = null;
            if (channelDict.TryGetValue(channelId, out var channel))
            {
                asset = channel.ResourceLoadHelper.LoadAsset<T>(info);
            }
            else
                throw new ArgumentNullException($"channelId :{channelId} is invalid !");
            return asset;
        }
        /// <summary>
        /// 特性无效！；
        /// 加载资源（同步）；
        /// 注意：AB环境下会获取bundle中所有T类型的对象；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        public T[] LoadAllAsset<T>(byte channelId, AssetInfo info)
        where T : UnityEngine.Object
        {
            T[] assets = null;
            if (channelDict.TryGetValue(channelId, out var channel))
            {
                assets = channel.ResourceLoadHelper.LoadAllAsset<T>(info);
            }
            else
                throw new ArgumentNullException($"channelId :{channelId} is invalid !");
            return assets;
        }
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="type">类对象类型</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public GameObject LoadPrefab(byte channelId, Type type, bool instantiate = false)
        {
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return LoadPrefab(channelId, info, instantiate);
            }
            else
                return null;
        }
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public GameObject LoadPrefab<T>(byte channelId, bool instantiate = false)
            where T : class
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return LoadPrefab(channelId, info, instantiate);
            }
            else
                return null;
        }
        /// <summary>
        /// 特性无效！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public GameObject LoadPrefab(byte channelId, AssetInfo info, bool instantiate = false)
        {
            GameObject go = null;
            if (channelDict.TryGetValue(channelId, out var channel))
            {
                var srcGo = channel.ResourceLoadHelper.LoadAsset<GameObject>(info);
                if (instantiate)
                    go = GameObject.Instantiate(srcGo);
                else
                    go = srcGo;
            }
            else
                throw new ArgumentNullException($"channelId :{channelId} is invalid !");
            return go;
        }
        /// <summary>
        /// 特性无效！；
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadAssetAsync<T>(byte channelId, AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null)
            where T : UnityEngine.Object
        {
            if (channelDict.TryGetValue(channelId, out var channel))
            {
                return channel.ResourceLoadHelper.LoadAssetAsync<T>(info, loadDoneCallback, loadingCallback);
            }
            else
                throw new ArgumentNullException($"channelId :{channelId} is invalid !");
        }
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="type">类对象类型</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync(byte channelId, Type type, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
        {
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return LoadPrefabAsync(channelId, info, loadDoneCallback, loadingCallback, instantiate);
            }
            else
                return null;
        }
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync<T>(byte channelId, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
            where T : class
        {
            var type = typeof(T);
            return LoadPrefabAsync(channelId, type, loadDoneCallback, loadingCallback, instantiate);
        }
        /// <summary>
        /// 特性无效！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync(byte channelId, AssetInfo info, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
        {
            if (channelDict.TryGetValue(channelId, out var channel))
            {
                return channel.ResourceLoadHelper.LoadAssetAsync<GameObject>(info, (srcGo) =>
                {
                    if (instantiate)
                    {
                        var go = GameObject.Instantiate(srcGo);
                        loadDoneCallback?.Invoke(go);
                    }
                    else
                        loadDoneCallback?.Invoke(srcGo);
                }, loadingCallback);
            }
            else
                throw new ArgumentNullException($"channelId :{channelId} is invalid !");
        }
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadSceneAsync(byte channelId, SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null)
        {
            if (channelDict.TryGetValue(channelId, out var channel))
            {
                return channel.ResourceLoadHelper.LoadSceneAsync(info, loadDoneCallback, loadingCallback);
            }
            else
                throw new ArgumentNullException($"channelId :{channelId} is invalid !");
        }
        /// <summary>
        /// 卸载资源;
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAsset(byte channelId, object customData, bool unloadAllLoadedObjects = false)
        {
            if (channelDict.TryGetValue(channelId, out var channel))
            {
                channel.ResourceLoadHelper.UnLoadAsset(customData, unloadAllLoadedObjects);
            }
            else
                throw new ArgumentNullException($"channelId :{channelId} is invalid !");
        }
        /// <summary>
        /// 卸载所有资源;
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAllAsset(byte channelId, bool unloadAllLoadedObjects = false)
        {
            if (channelDict.TryGetValue(channelId, out var channel))
            {
                channel.ResourceLoadHelper.UnLoadAllAsset(unloadAllLoadedObjects);
            }
            else
                throw new ArgumentNullException($"channelId :{channelId} is invalid !");
        }
        /// <summary>
        /// 清理内存，释放空闲内存
        /// </summary>
        void ClearMemory()
        {
            GC.Collect();
        }
        #endregion
    }
}

