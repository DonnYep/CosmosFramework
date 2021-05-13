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
    //================================================
    //1、资源加载模块分为内置部分与自定义部分；
    //2、内置加载通道在初始化时自动被注册，通过SwitchBuildInLoadMode()
    //方法进行通道切换；
    //3、自定义部分加载前需要进行通道注册，加载时需要指定通道名称；
    //4、默认提供两种加载模式，分别为 Resource与AssetBundle；
    //================================================
    public interface IResourceManager: IModuleManager
    {
        int ResourceLoadChannelCount { get; }
        ResourceLoadMode CurrentResourceLoadMode { get; }

        #region BuildIn
        /// <summary>
        /// 切换当前默认的加载模式；
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        void SwitchBuildInLoadMode(ResourceLoadMode resourceLoadMode);
        /// <summary>
        /// 添加者更新替换内置的加载帮助体；
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        /// <param name="loadHelper">加载帮助对象</param>
        void AddOrUpdateBuildInLoadHelper(ResourceLoadMode resourceLoadMode, IResourceLoadHelper loadHelper);
        IResourceLoadHelper PeekBuildInLoadHelper(ResourceLoadMode resourceLoadMode);
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        T LoadAsset<T>(AssetInfo info) where T : UnityEngine.Object;
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载资源（同步）；
        /// 注意：AB环境下会获取bundle中所有T类型的对象；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        T[] LoadAllAsset<T>(AssetInfo info) where T : UnityEngine.Object;
        /// <summary>
        /// 使用默认加载模式；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="type">类对象类型</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab(Type type, bool instantiate = false);
        /// <summary>
        /// 使用默认加载模式；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab<T>(bool instantiate = false) where T : class;
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab(AssetInfo info, bool instantiate = false);
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>加载协程迭代器</returns>
        Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null) where T : UnityEngine.Object;
        /// <summary>
        /// 使用默认加载模式；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="type">类对象类型</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        Coroutine LoadPrefabAsync(Type type, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false);
        /// <summary>
        /// 使用默认加载模式；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        Coroutine LoadPrefabAsync<T>(Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false) where T : class;
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        Coroutine LoadPrefabAsync(AssetInfo info, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false);
        /// <summary>
        /// 使用默认加载模式；
        /// 加载场景（异步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null);
        /// <summary>
        /// 使用默认加载模式；
        /// 卸载资源；
        /// </summary>
        /// <param name="customData">自定义的数据</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAsset(object customData, bool unloadAllLoadedObjects = false);
        /// <summary>
        /// 使用默认加载模式；
        /// 卸载所有资源；
        /// </summary>
        /// <param name="channelName">资源加载的通道id</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAllAsset(bool unloadAllLoadedObjects = false);
        #endregion

        #region CustomChannel
        /// <summary>
        /// 注册自定义通道；
        /// </summary>
        /// <param name="loadChannel">自定义加载通道</param>
        void RegisterLoadChannel(ResourceLoadChannel loadChannel);
        /// <summary>
        /// 注销自定义通道；
        /// </summary>
        /// <param name="channelName">自定义加载通道名</param>
        void DeregisterLoadChannel(string channelName);
        /// <summary>
        /// 是否存在自定义通道
        /// </summary>
        /// <param name="channelName">自定义加载通道名</param>
        /// <returns>是否存在</returns>
        bool HasLoadChannel(string channelName);
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性无效！；
        /// 加载资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        T LoadAsset<T>(string channelName, AssetInfo info)where T : UnityEngine.Object;
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
        T[] LoadAllAsset<T>(string channelName, AssetInfo info) where T : UnityEngine.Object;
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="type">类对象类型</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab(string channelName, Type type, bool instantiate = false);
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab<T>(string channelName, bool instantiate = false)where T : class;
        /// <summary>
        /// 使用自定义加载通道；
        /// 特性无效！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab(string channelName, AssetInfo info, bool instantiate = false);
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
        Coroutine LoadAssetAsync<T>(string channelName, AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null)
            where T : UnityEngine.Object;
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
        Coroutine LoadPrefabAsync(string channelName, Type type, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false);
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
        Coroutine LoadPrefabAsync<T>(string channelName, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
           where T : class;
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
        Coroutine LoadPrefabAsync(string channelName, AssetInfo info, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false);
        /// <summary>
        /// 使用自定义加载通道；
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        Coroutine LoadSceneAsync(string channelName, SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null);
        /// <summary>
        /// 使用自定义加载通道；
        /// 卸载资源；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="customData">自定义的数据</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAsset(string channelName, object customData, bool unloadAllLoadedObjects = false);
        /// <summary>
        /// 使用自定义加载通道；
        /// 卸载所有资源；
        /// </summary>
        /// <param name="channelName">资源加载的通道</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAllAsset(string channelName, bool unloadAllLoadedObjects = false);
        #endregion
    }
}
