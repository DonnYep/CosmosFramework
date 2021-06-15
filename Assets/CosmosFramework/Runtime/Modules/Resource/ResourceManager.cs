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
    //1、资源加载模块分为内置部分与自定义部分；
    //2、内置加载通道在初始化时自动被注册，通过SwitchBuildInLoadMode()
    //方法进行通道切换；
    //3、自定义部分加载前需要进行通道注册，加载时需要指定通道名称；
    //4、默认提供两种加载模式，分别为 Resource与AssetBundle；
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
        public override void OnInitialization()
        {
            builtInChannelDict = new Dictionary<ResourceLoadMode, ResourceLoadChannel>();
            builtInChannelDict.Add(ResourceLoadMode.Resource, new ResourceLoadChannel(ResourceLoadMode.Resource.ToString(), new ResourceLoader()));
            builtInChannelDict.Add(ResourceLoadMode.AssetBundle, new ResourceLoadChannel(ResourceLoadMode.AssetBundle.ToString(), new AssetBundleLoader()));
            builtInChannelDict.Add(ResourceLoadMode.QuarkAsset, new ResourceLoadChannel(ResourceLoadMode.QuarkAsset.ToString(), new QuarkAssetLoader()));
            currentResourceLoadMode = ResourceLoadMode.Resource;
            currentDefaultLoadHelper = builtInChannelDict[ResourceLoadMode.Resource].ResourceLoadHelper;
        }
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
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
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
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
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
        /// 特性无效！；
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null)
            where T : UnityEngine.Object
        {
            return currentDefaultLoadHelper.LoadAssetAsync<T>(info, loadDoneCallback, loadingCallback);
        }
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
        public Coroutine LoadPrefabAsync(Type type, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
        {
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return LoadPrefabAsync(info, loadDoneCallback, loadingCallback, instantiate);
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
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync<T>(Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
            where T : class
        {
            var type = typeof(T);
            return LoadPrefabAsync(type, loadDoneCallback, loadingCallback, instantiate);
        }
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
        public Coroutine LoadPrefabAsync(AssetInfo info, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
        {
            return currentDefaultLoadHelper.LoadAssetAsync<GameObject>(info, (srcGo) =>
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
        /// <summary>
        /// 使用默认加载模式；
        /// 加载场景（异步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null)
        {
            return currentDefaultLoadHelper.LoadSceneAsync(info, loadDoneCallback, loadingCallback);
        }
        /// <summary>
        /// 卸载资源;
        /// </summary>
        /// <param name="customData">自定义的数据</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAsset(object customData, bool unloadAllLoadedObjects = false)
        {
            currentDefaultLoadHelper.UnLoadAsset(customData, unloadAllLoadedObjects);
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
        #endregion
    }
}

