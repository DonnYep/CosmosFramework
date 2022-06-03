using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Resource
{
    //================================================
    /*
     * 1、资源加载模块分为内置部分与自定义部分；
     * 
     * 2、内置加载通道在初始化时自动被注册，通过SwitchBuildInLoadMode()
    * 方法进行通道切换；
    * 
    * 3、自定义部分加载前需要进行通道注册，加载时需要指定通道名称；
    * 
    * 4、默认提供两种加载模式，分别为 Resource与AssetBundle；
    */
    //================================================
    public interface IResourceManager : IModuleManager
    {
        /// <summary>
        /// 当前资源的加载模式；
        /// </summary>
        ResourceLoadMode ResourceLoadMode { get; }
        #region Methods
        /// <summary>
        /// 切换当前默认的加载模式；
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        void SwitchLoadMode(ResourceLoadMode resourceLoadMode);
        /// <summary>
        /// 添加者更新替换内置的加载帮助体；
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        /// <param name="loadHelper">加载帮助对象</param>
        void AddOrUpdateBuildInLoadHelper(ResourceLoadMode resourceLoadMode, IResourceLoadHelper loadHelper);
        /// <summary>
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : UnityEngine.Object;
        /// <summary>
        /// 加载资源（异步）；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="type">资源类型</typeparam>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAssetAsync(string assetName, Type type,Action<UnityEngine.Object> callback, Action<float> progress = null);
        /// <summary>
        /// 加载资源以及子资源；
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object;
        /// <summary>
        /// 加载资源以及子资源；
        /// 加载资源（异步）；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="type">资源类型</typeparam>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAssetWithSubAssetsAsync(string assetName, Type type, Action<UnityEngine.Object[]> callback, Action<float> progress = null);
        /// <summary>
        ///  加载资源（异步）；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        Coroutine LoadPrefabAsync(string assetName, Action<GameObject> callback, Action<float> progress = null, bool instantiate = false);
        /// <summary>
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo info, Action<float> progress, Action callback);
        /// <summary>
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Action callback);
        /// <summary>
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 卸载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Action callback);
        /// <summary>
        /// 卸载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 加载资源（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <returns>加载task</returns>
        Task<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object;
        /// <summary>
        /// 加载资源（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="type">资源类型</typeparam>
        /// <returns>加载task</returns>
        Task<UnityEngine.Object> LoadAssetAsync(string assetName, Type type);
        /// <summary>
        ///  加载资源（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载task</returns>
        Task<GameObject> LoadPrefabAsync(string assetName, bool instantiate = false);
        /// <summary>
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <returns>Task异步任务</returns>
        Task LoadSceneAsync(SceneAssetInfo info);
        /// <summary>
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <returns>Task异步任务</returns>
        Task LoadSceneAsync(SceneAssetInfo info, Action<float> progress);
        /// <summary>
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">加载场景完成的条件</param>
        /// <returns>Task异步任务</returns>
        Task LoadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition);
        /// <summary>
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <returns>Task异步任务</returns>
        Task LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress);
        /// <summary>
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">加载场景完成的条件</param>
        /// <returns>Task异步任务</returns>
        Task LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition);
        /// <summary>
        /// 卸载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <returns>Task异步任务</returns>
        Task UnloadSceneAsync(SceneAssetInfo info);
        /// <summary>
        /// 卸载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <returns>Task异步任务</returns>
        Task UnloadSceneAsync(SceneAssetInfo info, Action<float> progress);
        /// <summary>
        /// 卸载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <returns>Task异步任务</returns>
        Task UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition);
        /// <summary>
        /// 卸载资源（同步）；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        void UnloadAsset(string assetName);
        /// <summary>
        /// 卸载所有资源;
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnloadAllAsset(bool unloadAllLoadedObjects = false);
        #endregion
    }
}
