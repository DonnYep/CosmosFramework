using Cosmos.Resource.State;
using System;
using UnityEngine;
namespace Cosmos.Resource
{
    /// <summary>
    ///资源加载适配接口
    /// </summary>
    public interface IResourceLoadHelper
    {
        /// <summary>
        /// 初始化；
        /// </summary>
        void OnInitialize();
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
        /// <param name="type">资源类型</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAssetAsync(string assetName, Type type, Action<UnityEngine.Object> callback, Action<float> progress = null);
        /// <summary>
        ///  加载资源包种的所有资源（异步）；
        /// </summary>
        /// <param name="assetBundleName">资源包名</param>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAllAssetAsync(string assetBundleName, Action<UnityEngine.Object[]> callback, Action<float> progress = null);
        /// <summary>
        /// 加载资源以及子资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        Coroutine LoadMainAndSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object;
        /// <summary>
        /// 加载资源以及子资源（异步）；
        /// </summary>
        /// <param name="assetName">资源信息</param>
        /// <param name="type">资源类型</param>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAssetWithSubAssetsAsync(string assetName, Type type, Action<UnityEngine.Object[]> callback, Action<float> progress = null);
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="sceneAssetInfo">资源信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="sceneAssetInfo">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadSceneAsync(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 卸载全部场景（异步）
        /// </summary>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadAllSceneAsync(Action<float> progress, Action callback);
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetName">资源信息</param>
        void UnloadAsset(string assetName);
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="assetName">资源信息</param>
        void ReleaseAsset(string assetName);
        /// <summary>
        /// 释放资源包
        /// </summary>
        /// <param name="assetBundleName">资源包名</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects);
        /// <summary>
        /// 释放所有资源
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnloadAllAsset(bool unloadAllLoadedObjects);
        /// <summary>
        /// 获取bundle状态信息；
        /// </summary>
        /// <param name="bundleName">资源包名</param>
        /// <param name="bundleState">资源包状态</param>
        /// <returns>是否存在</returns>
        bool GetBundleState(string bundleName, out ResourceBundleState bundleState);
        /// <summary>
        /// 获取object信息；
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
        /// <summary>
        /// 重置loader;
        /// </summary>
        void Reset();
        /// <summary>
        /// 被终结移除；
        /// </summary>
        void OnTerminate();
    }
}
