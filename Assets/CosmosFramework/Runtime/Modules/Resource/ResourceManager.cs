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
        /// <inheritdoc/>
        public ResourceLoadMode ResourceLoadMode { get { return currentResourceLoadMode; } }
        #endregion
        #region Methods
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null)
            where T : UnityEngine.Object
        {
            return currentLoadHelper.LoadAssetAsync<T>(assetName, callback, progress);
        }
        /// <inheritdoc/>
        public Coroutine LoadAssetAsync(string assetName, Type type, Action<UnityEngine.Object> callback, Action<float> progress = null)
        {
            return currentLoadHelper.LoadAssetAsync(assetName, type, callback, progress);
        }
        /// <inheritdoc/>
        public Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return currentLoadHelper.LoadAssetWithSubAssetsAsync<T>(assetName, callback, progress);
        }
        /// <inheritdoc/>
        public Coroutine LoadAssetWithSubAssetsAsync(string assetName, Type type, Action<UnityEngine.Object[]> callback, Action<float> progress = null)
        {
            return currentLoadHelper.LoadAssetWithSubAssetsAsync(assetName, type, callback, progress);
        }
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action<float> progress, Action callback)
        {
            return currentLoadHelper.LoadSceneAsync(info, null, progress, null, callback);
        }
        /// <inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            return currentLoadHelper.LoadSceneAsync(info, null, progress, condition, callback);
        }
        /// <inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Action callback)
        {
            return currentLoadHelper.LoadSceneAsync(info, progressProvider, progress, null, callback);
        }
        /// <inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            return currentLoadHelper.LoadSceneAsync(info, progressProvider, progress, condition, callback);
        }
        /// <inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Action callback)
        {
            return currentLoadHelper.UnloadSceneAsync(info, progress, null, callback);
        }
        /// <inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            return currentLoadHelper.UnloadSceneAsync(info, progress, condition, callback);
        }
        /// <inheritdoc/>
        public async Task<T> LoadAssetAsync<T>(string assetName)
            where T : UnityEngine.Object
        {
            T asset = null;
            await currentLoadHelper.LoadAssetAsync<T>(assetName, a => asset = a, null);
            return asset;
        }
        /// <inheritdoc/>
        public async Task<UnityEngine.Object> LoadAssetAsync(string assetName, Type type)
        {
            UnityEngine.Object asset = null;
            await currentLoadHelper.LoadAssetAsync(assetName, type, a => asset = a, null);
            return asset;
        }
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneAssetInfo info)
        {
            await currentLoadHelper.LoadSceneAsync(info, null, null, null, null);
        }
        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneAssetInfo info, Action<float> progress)
        {
            await currentLoadHelper.LoadSceneAsync(info, null, progress, null, null);
        }
        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition)
        {
            await currentLoadHelper.LoadSceneAsync(info, null, progress, condition, null);
        }
        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress)
        {
            await currentLoadHelper.LoadSceneAsync(info, progressProvider, progress, null, null);
        }
        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition)
        {
            await currentLoadHelper.LoadSceneAsync(info, progressProvider, progress, condition, null);
        }
        /// <inheritdoc/>
        public async Task UnloadSceneAsync(SceneAssetInfo info)
        {
            await currentLoadHelper.UnloadSceneAsync(info, null, null, null);
        }
        /// <inheritdoc/>
        public async Task UnloadSceneAsync(SceneAssetInfo info, Action<float> progress)
        {
            await currentLoadHelper.UnloadSceneAsync(info, progress, null, null);
        }
        /// <inheritdoc/>
        public async Task UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition)
        {
            await currentLoadHelper.UnloadSceneAsync(info, progress, condition, null);
        }
        /// <inheritdoc/>
        public void UnloadAsset(string assetName)
        {
            currentLoadHelper.UnloadAsset(assetName);
        }
        /// <inheritdoc/>
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

