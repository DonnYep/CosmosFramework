using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Reflection;
namespace Cosmos.Resource
{
    //================================================
    //1、用户可以自定义实现加载通道，使用通道前需要被注册；
    //2、加载时需要指定通道名称；
    //================================================
    internal sealed partial class ResourceManager
    {
        #region Properties
        public int ResourceLoadChannelCount { get { return channelDict.Count; } }
        Dictionary<string, ResourceLoadChannel> channelDict
            =new Dictionary<string, ResourceLoadChannel>();
        #endregion
        /// <summary>
        /// 注册自定义通道；
        /// </summary>
        /// <param name="loadChannel">自定义加载通道</param>
        public void RegisterLoadChannel(ResourceLoadChannel loadChannel)
        {
            Utility.Text.IsStringValid(loadChannel.ChannelName, "channelName is invalid !");
            channelDict.TryAdd(loadChannel.ChannelName, loadChannel);
        }
        /// <summary>
        /// 注销自定义通道；
        /// </summary>
        /// <param name="channelName">自定义加载通道名</param>
        public void DeregisterLoadChannel(string channelName)
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            channelDict.TryRemove(channelName, out _);
        }
        /// <summary>
        /// 是否存在自定义通道
        /// </summary>
        /// <param name="channelName">自定义加载通道名</param>
        /// <returns>是否存在</returns>
        public bool HasLoadChannel(string channelName)
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            return channelDict.ContainsKey(channelName);
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性无效！；
        /// 加载资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        public T LoadAsset<T>(string channelName, AssetInfo info)
            where T : UnityEngine.Object
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            T asset = null;
            if (channelDict.TryGetValue(channelName, out var channel))
            {
                asset = channel.ResourceLoadHelper.LoadAsset<T>(info);
            }
            else
                throw new ArgumentNullException($"channelName :{channelName} is invalid !");
            return asset;
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性无效！；
        /// 加载资源（同步）；
        /// 注意：AB环境下会获取bundle中所有T类型的对象；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        public T[] LoadAllAsset<T>(string channelName, AssetInfo info)
        where T : UnityEngine.Object
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            T[] assets = null;
            if (channelDict.TryGetValue(channelName, out var channel))
            {
                assets = channel.ResourceLoadHelper.LoadAllAsset<T>(info);
            }
            else
                throw new ArgumentNullException($"channelName :{channelName} is invalid !");
            return assets;
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="type">类对象类型</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public GameObject LoadPrefab(string channelName, Type type, bool instantiate = false)
        {
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return LoadPrefab(channelName, info, instantiate);
            }
            else
                return null;
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public GameObject LoadPrefab<T>(string channelName, bool instantiate = false)
            where T : class
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return LoadPrefab(channelName, info, instantiate);
            }
            else
                return null;
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性无效！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public GameObject LoadPrefab(string channelName, AssetInfo info, bool instantiate = false)
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            GameObject go = null;
            var srcGo = LoadAsset<GameObject>(channelName, info);
            if (instantiate)
                go = GameObject.Instantiate(srcGo);
            else
                go = srcGo;
            return go;
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性无效！；
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadAssetAsync<T>(string channelName, AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null)
            where T : UnityEngine.Object
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            if (channelDict.TryGetValue(channelName, out var channel))
            {
                return channel.ResourceLoadHelper.LoadAssetAsync<T>(info, loadDoneCallback, loadingCallback);
            }
            else
                throw new ArgumentNullException($"channelName :{channelName} is invalid !");
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="type">类对象类型</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync(string channelName, Type type, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
        {
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return LoadPrefabAsync(channelName, info, loadDoneCallback, loadingCallback, instantiate);
            }
            else
                return null;
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync<T>(string channelName, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
            where T : class
        {
            var type = typeof(T);
            return LoadPrefabAsync(channelName, type, loadDoneCallback, loadingCallback, instantiate);
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性无效！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync(string channelName, AssetInfo info, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            if (channelDict.TryGetValue(channelName, out var channel))
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
                throw new ArgumentNullException($"channelName :{channelName} is invalid !");
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadSceneAsync(string channelName, SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null)
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            if (channelDict.TryGetValue(channelName, out var channel))
            {
                return channel.ResourceLoadHelper.LoadSceneAsync(info, loadDoneCallback, loadingCallback);
            }
            else
                throw new ArgumentNullException($"channelName :{channelName} is invalid !");
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 卸载资源；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="customData">自定义的数据</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAsset(string channelName, object customData, bool unloadAllLoadedObjects = false)
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            if (channelDict.TryGetValue(channelName, out var channel))
            {
                channel.ResourceLoadHelper.UnLoadAsset(customData, unloadAllLoadedObjects);
            }
            else
                throw new ArgumentNullException($"channelName :{channelName} is invalid !");
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 卸载所有资源；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAllAsset(string channelName, bool unloadAllLoadedObjects = false)
        {
            Utility.Text.IsStringValid(channelName, "channelName is invalid !");
            if (channelDict.TryGetValue(channelName, out var channel))
            {
                channel.ResourceLoadHelper.UnLoadAllAsset(unloadAllLoadedObjects);
            }
            else
                throw new ArgumentNullException($"channelName :{channelName} is invalid !");
        }
    }
}
