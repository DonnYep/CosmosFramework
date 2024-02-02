using Cosmos.Resource.State;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
        /// 当资源包被卸载时一同卸载已经被加载的资源
        /// </summary>
        bool UnloadAllLoadedObjectsWhenBundleUnload { get; set; }
        /// <summary>
        /// 当前资源的加载模式
        /// </summary>
        ResourceLoadMode ResourceLoadMode { get; }
        /// <summary>
        /// 请求资源文件清单成功事件
        /// <para>开始请求<see cref="StartRequestManifest"/></para>
        /// <para>结束请求<see cref="StopRequestManifest"/></para>
        /// </summary>
        event Action<ResourceRequestManifestSuccessEventArgs> ResourceRequestManifestSuccess;
        /// <summary>
        /// 请求资源文件清单失败事件
        /// <para>开始请求<see cref="StartRequestManifest"/></para>
        /// <para>结束请求<see cref="StopRequestManifest"/></para>
        /// </summary>
        event Action<ResourceRequestManifestFailureEventArgs> ResourceRequestManifestFailure;
        /// <summary>
        /// 任务下载成功事件
        /// <para>添加下载任务<see cref="AddResourceDownloadTask"/></para>
        /// <para>移除下载任务<see cref="RemoveResourceDownloadTask"/></para>
        /// </summary>
        event Action<ResourceDownloadSuccessEventArgs> OnDownloadSuccess;
        /// <summary>
        /// 任务下载失败事件
        /// <para>添加下载任务<see cref="AddResourceDownloadTask"/></para>
        /// <para>移除下载任务<see cref="RemoveResourceDownloadTask"/></para>
        /// </summary>
        event Action<ResourceDownloadFailureEventArgs> OnDownloadFailure;
        /// <summary>
        /// 整体任务下载更新事件
        /// <para>添加下载任务<see cref="AddResourceDownloadTask"/></para>
        /// <para>移除下载任务<see cref="RemoveResourceDownloadTask"/></para>
        /// </summary>
        event Action<ResourceDownloadUpdateEventArgs> OnDownloadUpdate;
        /// <summary>
        /// 所有任务完成事件
        /// <para>添加下载任务<see cref="AddResourceDownloadTask"/></para>
        /// <para>移除下载任务<see cref="RemoveResourceDownloadTask"/></para>
        /// </summary>
        event Action<ResourceDownloadCompeleteEventArgs> OnDownloadComplete;
        /// <summary>
        /// 下载任务取消回调
        /// <para>添加下载任务<see cref="AddResourceDownloadTask"/></para>
        /// <para>移除下载任务<see cref="RemoveResourceDownloadTask"/></para>
        /// </summary>
        event Action<ResourceDownloadTasksCancelEventArgs> OnDownloadCancel;
        /// <summary>
        /// 资源下载任务数量
        /// </summary>
        int ResourceDownloadTaskCount { get; }
        #region Methods
        /// <summary>
        /// 设置默认的加载方式
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        /// <param name="loadHelper">加载帮助对象</param>
        void SetDefaultLoadHeper(ResourceLoadMode resourceLoadMode, IResourceLoadHelper loadHelper);
        /// <summary>
        /// 切换当前默认的加载模式
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        void SwitchLoadMode(ResourceLoadMode resourceLoadMode);
        /// <summary>
        /// 重置加载器
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        void ResetLoadHeper(ResourceLoadMode resourceLoadMode);
        /// <summary>
        /// 添加者更新替换内置的加载帮助体
        /// </summary>
        /// <param name="resourceLoadMode">加载模式</param>
        /// <param name="loadHelper">加载帮助对象</param>
        void AddOrUpdateLoadHelper(ResourceLoadMode resourceLoadMode, IResourceLoadHelper loadHelper);
        /// <summary>
        /// 请求文件清单
        /// <para>成功回调<see cref="ResourceRequestManifestSuccess"/></para>
        /// <para>失败回调<see cref="ResourceRequestManifestFailure"/></para>
        /// </summary>
        /// <param name="manifestPath">文件清单地址</param>
        /// <param name="manifestEncryptionKey">文件解密密钥 [NULLABLE] </param>
        /// <param name="bundlePath">ab资源所在根目录</param>
        /// <returns>任务Id</returns>
        long StartRequestManifest(string manifestPath, string manifestEncryptionKey, string bundlePath = "");
        /// <summary>
        /// 停止请求文件清单
        /// </summary>
        void StopRequestManifest();
        /// <summary>
        /// 添加下载任务
        /// <para> <see cref="OnDownloadSuccess"/></para> 
        /// <para> <see cref="OnDownloadFailure"/> </para> 
        /// <para> <see cref="OnDownloadUpdate"/> </para> 
        /// <para> <see cref="OnDownloadComplete"/> </para> 
        /// </summary>
        /// <param name="task">下载任务</param>
        /// <returns>下载任务Id</returns>
        long AddResourceDownloadTask(ResourceDownloadTask task);
        /// <summary>
        /// 移除下载任务
        /// </summary>
        /// <param name="taskId">下载任务</param>
        /// <returns>移除结果，失败或成功</returns>
        void RemoveResourceDownloadTask(long taskId);
        /// <summary>
        /// 下载任务添加完毕后，开始下载
        /// </summary>
        void StartResourceDownload();
        /// <summary>
        /// 取消下载，此操作会情况下载任务
        /// <para> <see cref="OnDownloadCancel"/> </para> 
        /// </summary>
        void CancelResourceDownload();
        /// <summary>
        /// 加载资源（异步），增加一个引用计数
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : Object;
        /// <summary>
        /// 加载资源（异步），增加一个引用计数
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="type">资源类型</typeparam>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null);
        /// <summary>
        /// 加载资源以及子资源（异步），增加一个引用计数
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        Coroutine LoadMainAndSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : Object;
        /// <summary>
        /// 加载资源以及子资源（异步），增加一个引用计数
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="type">资源类型</typeparam>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        Coroutine LoadMainAndSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress = null);
        /// <summary>
        ///  加载预制体资源（异步），增加一个引用计数
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        Coroutine LoadPrefabAsync(string assetName, Action<GameObject> callback, Action<float> progress = null, bool instantiate = false);
        /// <summary>
        ///  加载资源包种的所有资源（异步），增加一个引用计数
        /// </summary>
        /// <param name="assetBundleName">资源包</param>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAllAssetAsync(string assetBundleName, Action<Object[]> callback, Action<float> progress = null);
        /// <summary>
        /// 加载场景（异步），增加一个引用计数
        /// </summary>
        /// <param name="sceneAssetInfo">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 卸载场景（异步），增加一个引用计数
        /// </summary>
        /// <param name="sceneAssetInfo">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadSceneAsync(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 卸载资源（同步），减少一个引用计数
        /// </summary>
        /// <param name="assetName">资源名</param>
        void UnloadAsset(string assetName);
        /// <summary>
        /// 卸载多个资源（同步），减少一个引用计数
        /// </summary>
        /// <param name="assetNames">资源名合集</param>
        void UnloadAssets(IEnumerable<string> assetNames);
        /// <summary>
        /// 释放所有资源（同步），引用计数归零
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnloadAllAsset(bool unloadAllLoadedObjects);
        /// <summary>
        /// 释放资源包（同步），引用计数归零
        /// </summary>
        /// <param name="assetBundleName">资源包名</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects);
        /// <summary>
        /// 获取bundle状态信息
        /// </summary>
        /// <param name="bundleName">资源包名</param>
        /// <param name="bundleState">资源包状态</param>
        /// <returns>是否存在</returns>
        bool GetBundleState(string bundleName, out ResourceBundleState bundleState);
        /// <summary>
        /// 获取object信息
        /// </summary>
        /// <param name="objectName">资源对象名</param>
        /// <param name="objectState">资源对象状态</param>
        /// <returns>是否存在</returns>
        bool GetObjectState(string objectName, out ResourceObjectState objectState);
        /// <summary>
        /// 获得已加载的bundle信息
        /// </summary>
        /// <returns>已加载的包体信息</returns>
        ResourceBundleState[] GetLoadedBundleState();
        /// <summary>
        /// Get version of resource.
        /// </summary>
        /// <returns>resource version</returns>
        ResourceVersion GetResourceVersion();
        #endregion
    }
}