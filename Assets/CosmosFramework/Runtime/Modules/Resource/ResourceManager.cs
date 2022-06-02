using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Threading.Tasks;

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
    * 4、默认提供三种加载模式，分别为Resource、AssetBundle和AssetDatabase。
    */
    //================================================
    [Module]
    internal sealed partial class ResourceManager : Module, IResourceManager
    {
        #region Properties
        Dictionary<ResourceLoadMode, ResourceLoadChannel> loadChannelDict;
        IResourceLoadHelper currentLoadHelper;
        ResourceLoadMode currentResourceLoadMode;
        /// <summary>
        /// 当前资源的加载模式；
        /// </summary>
        public ResourceLoadMode ResourceLoadMode { get { return currentResourceLoadMode; } }
        #endregion
        #region Methods
        /// <summary>
        /// 切换当前默认的加载模式；
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        public void SwitchLoadMode(ResourceLoadMode resourceLoadMode)
        {
            if (loadChannelDict.TryGetValue(resourceLoadMode, out var channel))
            {
                this.currentResourceLoadMode = resourceLoadMode;
                currentLoadHelper = channel.ResourceLoadHelper;
            }
            else
            {
                throw new ArgumentNullException($"ResourceLoadMode : {resourceLoadMode} is invalid !");
            }
        }
        /// <summary>
        /// 添加者更新替换内置的加载帮助体；
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        /// <param name="loadHelper">加载帮助对象</param>
        public async void AddOrUpdateBuildInLoadHelper(ResourceLoadMode resourceLoadMode, IResourceLoadHelper loadHelper)
        {
            if (Utility.Assert.IsNull(loadHelper))
                throw new ArgumentNullException($"IResourceLoadHelper is invalid !");
            if (loadChannelDict.TryGetValue(resourceLoadMode, out var channel))
                await new WaitUntil(() => channel.ResourceLoadHelper.IsProcessing == false);
            loadChannelDict[resourceLoadMode] = new ResourceLoadChannel(resourceLoadMode.ToString(), loadHelper);
            if (currentResourceLoadMode == resourceLoadMode)
                currentLoadHelper = loadHelper;
        }
        /// <summary>
        /// 特性无效！
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null)
            where T : UnityEngine.Object
        {
            return currentLoadHelper.LoadAssetAsync<T>(assetName, callback, progress);
        }
        /// <summary>
        /// 加载资源以及子资源；
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return currentLoadHelper.LoadAssetWithSubAssetsAsync<T>(assetName, callback, progress);
        }
        /// <summary>
        ///  加载资源（异步）；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync(string assetName, Action<GameObject> callback, Action<float> progress = null, bool instantiate = false)
        {
            return currentLoadHelper.LoadAssetAsync<GameObject>(assetName, (srcGo) =>
            {
                if (instantiate)
                {
                    var go = GameObject.Instantiate(srcGo);
                    callback?.Invoke(go);
                }
                else
                    callback?.Invoke(srcGo);
            }, progress);
        }
        /// <summary>
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action<float> progress, Action callback)
        {
            return currentLoadHelper.LoadSceneAsync(info, null,progress, null,callback);
        }
        /// <summary>
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            return currentLoadHelper.LoadSceneAsync(info, null,progress, condition, callback);
        }
        /// <summary>
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Action callback)
        {
            return currentLoadHelper.LoadSceneAsync(info, progressProvider, progress, null, callback);
        }
        /// <summary>
        /// 加载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider,  Action<float> progress, Func<bool> condition, Action callback)
        {
            return currentLoadHelper.LoadSceneAsync(info, progressProvider, progress, condition, callback);
        }
        /// <summary>
        /// 卸载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        public Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Action callback)
        {
            return currentLoadHelper.UnloadSceneAsync(info, progress, null, callback);
        }
        /// <summary>
        /// 卸载场景（异步）;
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        public Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            return currentLoadHelper.UnloadSceneAsync(info, progress, condition, callback);
        }
        /// <summary>
        /// 加载资源（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <returns>加载task</returns>
        public async Task<T> LoadAssetAsync<T>(string assetName)
            where T : UnityEngine.Object
        {
            T asset = null;
            await currentLoadHelper.LoadAssetAsync<T>(assetName, a => asset = a, null);
            return asset;
        }
        /// <summary>
        /// 特性无效！
        ///  加载资源（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载task</returns>
        public async Task<GameObject> LoadPrefabAsync(string assetName, bool instantiate = false)
        {
            GameObject go = null;
            await currentLoadHelper.LoadAssetAsync<GameObject>(assetName, (asset) =>
            {
                if (instantiate)
                    go = GameObject.Instantiate(asset);
                else
                    go = asset;
            }, null);
            return go;
        }
        /// <summary>
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <returns>Task异步任务</returns>
        public async Task LoadSceneAsync(SceneAssetInfo info)
        {
            await currentLoadHelper.LoadSceneAsync(info,null, null, null, null);
        }
        /// <summary>
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <returns>Task异步任务</returns>
        public async Task LoadSceneAsync(SceneAssetInfo info, Action<float> progress)
        {
            await currentLoadHelper.LoadSceneAsync(info,null, progress, null, null);
        }
        /// <summary>
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">加载场景完成的条件</param>
        /// <returns>Task异步任务</returns>
        public async Task LoadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition)
        {
            await currentLoadHelper.LoadSceneAsync(info, null,progress, condition, null);
        }
        /// <summary>
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <returns>Task异步任务</returns>
        public async Task LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress)
        {
            await currentLoadHelper.LoadSceneAsync(info, progressProvider, progress, null, null);
        }
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
        public async Task LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition)
        {
            await currentLoadHelper.LoadSceneAsync(info, progressProvider, progress, condition, null);
        }
        /// <summary>
        /// 卸载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <returns>Task异步任务</returns>
        public async Task UnloadSceneAsync(SceneAssetInfo info)
        {
            await currentLoadHelper.UnloadSceneAsync(info, null, null, null);
        }
        /// <summary>
        /// 卸载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <returns>Task异步任务</returns>
        public async Task UnloadSceneAsync(SceneAssetInfo info, Action<float> progress)
        {
            await currentLoadHelper.UnloadSceneAsync(info, progress, null, null);
        }
        /// <summary>
        /// 卸载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <returns>Task异步任务</returns>
        public async Task UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition)
        {
            await currentLoadHelper.UnloadSceneAsync(info, progress, condition, null);
        }
        /// <summary>
        /// 卸载资源（同步）；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        public void UnloadAsset(string assetName)
        {
            currentLoadHelper.UnloadAsset(assetName);
        }
        /// <summary>
        /// 卸载所有资源;
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnloadAllAsset(bool unloadAllLoadedObjects = false)
        {
            currentLoadHelper.UnloadAllAsset(unloadAllLoadedObjects);
        }
        protected override void OnInitialization()
        {
            loadChannelDict = new Dictionary<ResourceLoadMode, ResourceLoadChannel>();
            loadChannelDict.Add(ResourceLoadMode.Resource, new ResourceLoadChannel(ResourceLoadMode.Resource.ToString(), new ResourcesLoader()));
            currentResourceLoadMode = ResourceLoadMode.Resource;
            currentLoadHelper = loadChannelDict[ResourceLoadMode.Resource].ResourceLoadHelper;
        }
        #endregion
    }
}

