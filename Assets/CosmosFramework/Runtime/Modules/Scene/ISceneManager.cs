using Cosmos.Resource;
using System;
using UnityEngine;
namespace Cosmos.Scene
{
    public interface ISceneManager : IModuleManager
    {
        /// <summary>
        ///  异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="callback">加载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Action callback = null);
        /// <summary>
        ///  异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Action callback = null);
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Func<bool> condition, Action callback = null);
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null);
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Action callback = null);
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null);
        /// <summary>
        /// 异步卸载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadSceneAsync(SceneAssetInfo sceneInfo, Action callback = null);
        /// <summary>
        /// 异步卸载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Action callback = null);
        /// <summary>
        ///  异步卸载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadSceneAsync(SceneAssetInfo sceneInfo, Func<bool> condition, Action callback = null);
        /// <summary>
        /// 异步卸载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnloadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null);
    }
}
