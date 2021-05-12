using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos
{
    public interface IResourceManager: IModuleManager
    {
        int ResourceLoadChannelCount { get; }
        void AddLoadChannel(ResourceLoadChannel loadChannel);
        void RemoveLoadChannel(byte channelId);
        bool HasLoadChannel(byte channelId);
        /// <summary>
        /// 特性无效！；
        /// 加载资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        T LoadAsset<T>(byte channelId, AssetInfo info)where T : UnityEngine.Object;
        /// 特性无效！；
        /// 加载资源（同步）；
        /// 注意：AB环境下会获取bundle中所有T类型的对象；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        T[] LoadAllAsset<T>(byte channelId, AssetInfo info) where T : UnityEngine.Object;
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="type">类对象类型</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab(byte channelId, Type type, bool instantiate = false);
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab<T>(byte channelId, bool instantiate = false)where T : class;
        /// <summary>
        /// 特性无效！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab(byte channelId, AssetInfo info, bool instantiate = false);
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
        Coroutine LoadAssetAsync<T>(byte channelId, AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null)
            where T : UnityEngine.Object;
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
        Coroutine LoadPrefabAsync(byte channelId, Type type, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false);
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
        Coroutine LoadPrefabAsync<T>(byte channelId, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
           where T : class;
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
        Coroutine LoadPrefabAsync(byte channelId, AssetInfo info, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false);
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        Coroutine LoadSceneAsync(byte channelId, SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null);
        /// <summary>
        /// 卸载资源;
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAsset(byte channelId, object customData, bool unloadAllLoadedObjects = false);
        /// <summary>
        /// 卸载所有资源;
        /// </summary>
        /// <param name="channelId">资源加载的通道id</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAllAsset(byte channelId, bool unloadAllLoadedObjects = false);
    }
}
