using System;
using UnityEngine;

namespace Cosmos.Scene
{
    /// <summary>
    /// 加载场景帮助类；
    /// 可实现区分AB加载与BuildScene加载；
    /// </summary>
    public class SceneAssetHelper
    {
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            return CosmosEntry.ResourceManager.LoadSceneAsync(sceneInfo, progressProvider, progress, condition, callback);
        }
        /// <summary>
        /// 异步卸载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine UnloadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            return CosmosEntry.ResourceManager.UnloadSceneAsync(sceneInfo, progress, condition, callback);
        }
    }
}
