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
    * 4、默认提供两种加载模式，分别为 Resource与AssetBundle；
    */
    //================================================
    [Module]
    internal sealed partial class ResourceManager : Module, IResourceManager
    {
        #region Properties
        ResourceLoadMode currentResourceLoadMode;
        public ResourceLoadMode CurrentResourceLoadMode { get { return currentResourceLoadMode; } }
        Dictionary<ResourceLoadMode, ResourceLoadChannel> builtInChannelDict;
        IResourceLoadHelper currentDefaultLoadHelper;
        #endregion
        #region Methods
        public void SwitchBuildInLoadMode(ResourceLoadMode resourceLoadMode)
        {
            if (builtInChannelDict.TryGetValue(resourceLoadMode, out var channel))
            {
                this.currentResourceLoadMode = resourceLoadMode;
                currentDefaultLoadHelper = channel.ResourceLoadHelper;
            }
            else
            {
                throw new ArgumentNullException($"ResourceLoadMode : {resourceLoadMode} is invalid !");
            }
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 添加者更新替换内置的加载帮助体；
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        /// <param name="loadHelper">加载帮助对象</param>
        public async void AddOrUpdateBuildInLoadHelper(ResourceLoadMode resourceLoadMode, IResourceLoadHelper loadHelper)
        {
            if (Utility.Assert.IsNull(loadHelper))
                throw new ArgumentNullException($"IResourceLoadHelper is invalid !");
            if (builtInChannelDict.TryGetValue(resourceLoadMode, out var channel))
                await new WaitUntil(() => channel.ResourceLoadHelper.IsLoading == false);
            builtInChannelDict[resourceLoadMode] = new ResourceLoadChannel(resourceLoadMode.ToString(), loadHelper);
            if (currentResourceLoadMode == resourceLoadMode)
                currentDefaultLoadHelper = loadHelper;
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        public T LoadAsset<T>(AssetInfo info)
            where T : UnityEngine.Object
        {
            return currentDefaultLoadHelper.LoadAsset<T>(info);
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载资源（同步）；
        /// 注意：AB环境下会获取bundle中所有T类型的对象；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        public T[] LoadAllAsset<T>(AssetInfo info)
        where T : UnityEngine.Object
        {
            return currentDefaultLoadHelper.LoadAllAsset<T>(info);
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="type">类对象类型</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public GameObject LoadPrefab(Type type, bool instantiate = false)
        {
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath);
                return LoadPrefab(info, instantiate);
            }
            else
                return null;
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public GameObject LoadPrefab<T>(bool instantiate = false)
            where T : class
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath);
                return LoadPrefab(info, instantiate);
            }
            else
                return null;
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public GameObject LoadPrefab(AssetInfo info, bool instantiate = false)
        {
            GameObject go = null;
            var srcGo = LoadAsset<GameObject>(info);
            if (instantiate)
                go = GameObject.Instantiate(srcGo);
            else
                go = srcGo;
            return go;
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 加载资源以及子资源；
        /// 加载资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源数组</returns>
        public T[] LoadAssetWithSubAssets<T>(AssetInfo info) where T : UnityEngine.Object
        {
            return currentDefaultLoadHelper.LoadAssetWithSubAssets<T>(info);
        }
        /// <summary>
        /// 使用自定义加载通道；
        /// 加载资源以及子资源；
        /// 加载资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadAssetWithSubAssetsAsync<T>(AssetInfo info, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return currentDefaultLoadHelper.LoadAssetWithSubAssetsAsync<T>(info, callback, progress);
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> callback, Action<float> progress = null)
            where T : UnityEngine.Object
        {
            return currentDefaultLoadHelper.LoadAssetAsync<T>(info, callback, progress);
        }
        /// <summary>
        /// 异步加载资源；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>加载task</returns>
        public async Task<T> LoadAssetAsync<T>(AssetInfo info)
            where T : UnityEngine.Object
        {
            T asset = null;
            await currentDefaultLoadHelper.LoadAssetAsync<T>(info, a=>asset=a, null);
            return asset;
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="type">类对象类型</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync(Type type, Action<GameObject> callback, Action<float> progress = null, bool instantiate = false)
        {
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath);
                return LoadPrefabAsync(info, callback, progress, instantiate);
            }
            else
                return null;
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync<T>(Action<GameObject> callback, Action<float> progress = null, bool instantiate = false)
            where T : class
        {
            var type = typeof(T);
            return LoadPrefabAsync(type, callback, progress, instantiate);
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync(AssetInfo info, Action<GameObject> callback, Action<float> progress = null, bool instantiate = false)
        {
            return currentDefaultLoadHelper.LoadAssetAsync<GameObject>(info, (srcGo) =>
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
        /// 使用默认加载模式；
        /// 特性无效！；
        /// 加载预制体资源（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载task</returns>
        public async Task<GameObject> LoadPrefabAsync(AssetInfo info, bool instantiate = false)
        {
            GameObject go= null;
            await currentDefaultLoadHelper.LoadAssetAsync<GameObject>(info, (asset) =>
            {
                if (instantiate)
                    go = GameObject.Instantiate(asset);
                else
                    go = asset;
            },null);
            return go;
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 加载场景（异步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action callback, Action<float> progress = null)
        {
            return currentDefaultLoadHelper.LoadSceneAsync(info, callback, progress);
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 加载场景（异步）；
        /// 须使用await获取结果；
        /// aysnc/await机制是使用状态机切换上下文。使用Task.Result会阻塞当前线程导致aysnc/await无法切换回线程上下文，引发锁死；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <returns>Task异步任务</returns>
        public async Task LoadSceneAsync(SceneAssetInfo info)
        {
            await currentDefaultLoadHelper.LoadSceneAsync(info, null, null);
        }
        /// <summary>
        /// 卸载资源;
        /// </summary>
        public void UnLoadAsset(AssetInfo info)
        {
            currentDefaultLoadHelper.UnLoadAsset(info);
        }
        /// <summary>
        /// 使用默认加载模式；
        /// 卸载所有资源;
        /// </summary>
        /// <param name="channelName">资源加载的通道id</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            currentDefaultLoadHelper.UnLoadAllAsset(unloadAllLoadedObjects);
        }
        protected override void OnInitialization()
        {
            builtInChannelDict = new Dictionary<ResourceLoadMode, ResourceLoadChannel>();
            builtInChannelDict.Add(ResourceLoadMode.Resource, new ResourceLoadChannel(ResourceLoadMode.Resource.ToString(), new ResourcesLoader()));
            builtInChannelDict.Add(ResourceLoadMode.AssetBundle, new ResourceLoadChannel(ResourceLoadMode.AssetBundle.ToString(), new AssetBundleLoader()));
            currentResourceLoadMode = ResourceLoadMode.Resource;
            currentDefaultLoadHelper = builtInChannelDict[ResourceLoadMode.Resource].ResourceLoadHelper;
        }


        #endregion
    }
}

