using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using Cosmos.WebRequest;
using Cosmos.Resource.State;
using Cosmos.Download;

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
        ResourceManifestRequester resourceManifestRequester;
        ResourceDownloader resourceDownloader;
        Action<ResourceRequestManifestSuccessEventArgs> resourceRequestManifestSuccess;
        Action<ResourceRequestManifestFailureEventArgs> resourceRequestManifestFailure;
        /// <inheritdoc/>
        public ResourceLoadMode ResourceLoadMode
        {
            get { return ResourceDataProxy.ResourceLoadMode; }
        }
        /// <inheritdoc/>
        public event Action<ResourceRequestManifestSuccessEventArgs> ResourceRequestManifestSuccess
        {
            add { resourceRequestManifestSuccess += value; }
            remove { resourceRequestManifestSuccess -= value; }
        }
        /// <inheritdoc/>
        public event Action<ResourceRequestManifestFailureEventArgs> ResourceRequestManifestFailure
        {
            add { resourceRequestManifestFailure += value; }
            remove { resourceRequestManifestFailure -= value; }
        }
        /// <inheritdoc/>
        public event Action<ResourceDownloadSuccessEventArgs> OnDownloadSuccess
        {
            add { resourceDownloader.OnDownloadSuccess += value; }
            remove { resourceDownloader.OnDownloadSuccess -= value; }
        }
        /// <inheritdoc/>
        public event Action<ResourceDownloadFailureEventArgs> OnDownloadFailure
        {
            add { resourceDownloader.OnDownloadFailure += value; }
            remove { resourceDownloader.OnDownloadFailure -= value; }
        }
        /// <inheritdoc/>
        public event Action<ResourceDownloadUpdateEventArgs> OnDownloadUpdate
        {
            add { resourceDownloader.OnDownloadUpdate += value; }
            remove { resourceDownloader.OnDownloadUpdate -= value; }
        }
        /// <inheritdoc/>
        public event Action<ResourceDownloadCompeleteEventArgs> OnDownloadComplete
        {
            add { resourceDownloader.OnDownloadComplete += value; }
            remove { resourceDownloader.OnDownloadComplete -= value; }
        }
        /// <inheritdoc/>
        public event Action<ResourceDownloadTasksCancelEventArgs> OnDownloadCancel
        {
            add { resourceDownloader.OnDownloadCancel += value; }
            remove { resourceDownloader.OnDownloadCancel -= value; }
        }
        /// <inheritdoc/>
        public int ResourceDownloadTaskCount
        {
            get { return resourceDownloader.DownloadTaskCount;}
        }
        /// <inheritdoc/>
        public bool UnloadAllLoadedObjectsWhenBundleUnload
        {
            get { return ResourceDataProxy.UnloadAllLoadedObjectsWhenBundleUnload; }
            set { ResourceDataProxy.UnloadAllLoadedObjectsWhenBundleUnload = value; }
        }
        #endregion
        #region Methods
        /// <inheritdoc/>
        public void SetDefaultLoadHeper(ResourceLoadMode resourceLoadMode, IResourceLoadHelper loadHelper)
        {
            if (loadHelper == null)
                throw new ArgumentNullException($"IResourceLoadHelper is invalid !");
            var channel = new ResourceLoadChannel(resourceLoadMode, loadHelper);
            if (loadChannelDict.TryRemove(resourceLoadMode, out var previouseChannel))
                previouseChannel.ResourceLoadHelper.OnTerminate();
            loadChannelDict.Add(resourceLoadMode, channel);
            ResourceDataProxy.ResourceLoadMode = resourceLoadMode;
            currentLoadHelper = channel.ResourceLoadHelper;
            currentLoadHelper.OnInitialize();
        }
        /// <inheritdoc/>
        public void SwitchLoadMode(ResourceLoadMode resourceLoadMode)
        {
            if (loadChannelDict.TryGetValue(resourceLoadMode, out var channel))
            {
                ResourceDataProxy.ResourceLoadMode = resourceLoadMode;
                currentLoadHelper = channel.ResourceLoadHelper;
            }
            else
            {
                throw new ArgumentNullException($"ResourceLoadMode : {resourceLoadMode} is invalid !");
            }
        }
        /// <inheritdoc/>
        public void ResetLoadHeper(ResourceLoadMode resourceLoadMode)
        {
            if (loadChannelDict.TryRemove(resourceLoadMode, out var channel))
                channel.ResourceLoadHelper.Reset();
        }
        /// <inheritdoc/>
        public void AddOrUpdateLoadHelper(ResourceLoadMode resourceLoadMode, IResourceLoadHelper loadHelper)
        {
            if (Utility.Assert.IsNull(loadHelper))
                throw new ArgumentNullException($"IResourceLoadHelper is invalid !");
            if (loadChannelDict.TryRemove(resourceLoadMode, out var previouseChannel))
                previouseChannel.ResourceLoadHelper.OnTerminate();
            var newChannel = new ResourceLoadChannel(resourceLoadMode, loadHelper);
            loadChannelDict.Add(resourceLoadMode, newChannel);
            newChannel.ResourceLoadHelper.OnInitialize();
            if (ResourceDataProxy.ResourceLoadMode == resourceLoadMode)
                currentLoadHelper = loadHelper;
        }
        /// <inheritdoc/>
        public long StartRequestManifest(string manifestPath, string manifestEncryptionKey, string bundlePath)
        {
            return resourceManifestRequester.StartRequestManifest(manifestPath, manifestEncryptionKey, bundlePath);
        }
        /// <inheritdoc/>
        public void StopRequestManifest()
        {
            resourceManifestRequester.StopRequestManifest();
        }
        /// <inheritdoc/>
        public long AddResourceDownloadTask(ResourceDownloadTask task)
        {
            return resourceDownloader.AddDownloadTask(task);
        }
        /// <inheritdoc/>
        public void RemoveResourceDownloadTask(long taskId)
        {
            resourceDownloader.RemoveDownloadTask(taskId);
        }
        /// <inheritdoc/>
        public void StartResourceDownload()
        {
            resourceDownloader.StartDownload();
        }
        /// <inheritdoc/>
        public void CancelResourceDownload()
        {
            resourceDownloader.CancelDownload();
        }
        /// <inheritdoc/>
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null)
            where T : Object
        {
            return currentLoadHelper.LoadAssetAsync<T>(assetName, callback, progress);
        }
        /// <inheritdoc/>
        public Coroutine LoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null)
        {
            return currentLoadHelper.LoadAssetAsync(assetName, type, callback, progress);
        }
        /// <inheritdoc/>
        public Coroutine LoadMainAndSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : Object
        {
            return currentLoadHelper.LoadMainAndSubAssetsAsync<T>(assetName, callback, progress);
        }
        /// <inheritdoc/>
        public Coroutine LoadMainAndSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress = null)
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
                    GameObject go = null;
                    if (srcGo != null)
                        go = GameObject.Instantiate(srcGo);
                    callback?.Invoke(go);
                }
                else
                    callback?.Invoke(srcGo);
            }, progress);
        }
        /// <inheritdoc/>
        public Coroutine LoadAllAssetAsync(string assetPack, Action<Object[]> callback, Action<float> progress = null)
        {
            return currentLoadHelper.LoadAllAssetAsync(assetPack, callback, progress);
        }
        /// <inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            return currentLoadHelper.LoadSceneAsync(sceneAssetInfo, progressProvider, progress, condition, callback);
        }
        /// <inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback)
        {
            return currentLoadHelper.UnloadSceneAsync(sceneAssetInfo, progress, condition, callback);
        }
        /// <inheritdoc/>
        public void UnloadAsset(string assetName)
        {
            currentLoadHelper.UnloadAsset(assetName);
        }
        /// <inheritdoc/>
        public void UnloadAssets(IEnumerable<string> assetNames)
        {
            foreach (var assetName in assetNames)
            {
                currentLoadHelper.UnloadAsset(assetName);
            }
        }
        /// <inheritdoc/>
        public void UnloadAllAsset(bool unloadAllLoadedObjects)
        {
            currentLoadHelper.UnloadAllAsset(unloadAllLoadedObjects);
        }
        /// <inheritdoc/>
        public void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects)
        {
            currentLoadHelper.UnloadAssetBundle(assetBundleName, unloadAllLoadedObjects);
        }
        /// <inheritdoc/>
        public bool GetBundleState(string bundleName, out ResourceBundleState bundleState)
        {
            return currentLoadHelper.GetBundleState(bundleName, out bundleState);
        }
        /// <inheritdoc/>
        public bool GetObjectState(string objectName, out ResourceObjectState objectState)
        {
            return currentLoadHelper.GetObjectState(objectName, out objectState);
        }
        /// <inheritdoc/>
        public ResourceBundleState[] GetLoadedBundleState()
        {
            return currentLoadHelper.GetLoadedBundleState();
        }
        /// <inheritdoc/>
        public ResourceVersion GetResourceVersion()
        {
            return currentLoadHelper.GetResourceVersion();
        }
        protected override void OnInitialization()
        {
            loadChannelDict = new Dictionary<ResourceLoadMode, ResourceLoadChannel>();
        }
        protected override void OnPreparatory()
        {
            var webRequestManager = GameManager.GetModule<IWebRequestManager>();
            resourceManifestRequester = new ResourceManifestRequester(webRequestManager, OnRequestManifestSuccess, OnRequestManifestFailure);
            resourceManifestRequester.OnInitialize();
            var downloadManager = GameManager.GetModule<IDownloadManager>();
            resourceDownloader = new ResourceDownloader(downloadManager);
            resourceDownloader.OnInitialize();
        }
        protected override void OnTermination()
        {
            resourceManifestRequester.OnTerminate();
            resourceDownloader.OnTerminate();
        }
        void OnRequestManifestSuccess(long taskId, string manifestPath, string bundlePath, ResourceManifest resourceManifest)
        {
            var eventArgs = ResourceRequestManifestSuccessEventArgs.Create(taskId, manifestPath, bundlePath, ResourceDataProxy.ResourceBundlePathType, resourceManifest);
            resourceRequestManifestSuccess?.Invoke(eventArgs);
            ResourceRequestManifestSuccessEventArgs.Release(eventArgs);
        }
        void OnRequestManifestFailure(long taskId, string manifestPath, string errorMessage)
        {
            var eventArgs = ResourceRequestManifestFailureEventArgs.Create(taskId, manifestPath, ResourceLoadMode, ResourceDataProxy.ResourceBundlePathType, errorMessage);
            resourceRequestManifestFailure?.Invoke(eventArgs);
            ResourceRequestManifestFailureEventArgs.Release(eventArgs);
        }
        #endregion
    }
}

