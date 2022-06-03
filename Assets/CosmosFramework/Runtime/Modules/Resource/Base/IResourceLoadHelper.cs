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
        /// 是否进行中
        /// </summary>
        bool IsProcessing { get; }
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
        /// 加载资源以及子资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源信息</param>
        /// <param name="callback">加载完成事件</param>
        /// <param name="progress">加载中事件</param>
        /// <returns>协程对象</returns>
        Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object;
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
        /// <param name="info">资源信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="assetName">资源信息</param>
        void UnloadAsset(string assetName);
        /// <summary>
        /// 卸载所有资源;
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnloadAllAsset(bool unloadAllLoadedObjects = false);
    }
}
