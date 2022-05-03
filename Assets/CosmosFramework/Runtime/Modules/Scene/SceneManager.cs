using UnityEngine;
using System;
namespace Cosmos.Scene
{
    //================================================
    /*
     * 1、场景加载模块。
     * 
     * 2、处于玩家体验的考虑，此模块只提供异步方法。
     */
    //================================================
    [Module]
    internal sealed class SceneManager : Module, ISceneManager
    {
        /// <summary>
        /// 场景帮助体；
        /// </summary>
        ISceneHelper sceneHelper;
        /// <summary>
        /// 异步设置场景加载helper；
        /// </summary>
        /// <param name="sceneHelper">自定义实现的ISceneHelper</param>
        public void SetHelperAsync(ISceneHelper sceneHelper)
        {
            this.sceneHelper = sceneHelper;
        }
        /// <summary>
        ///  异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="callback">加载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, null, null, null, callback);
        }
        /// <summary>
        ///  异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action<float> progress, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, null, progress, null, callback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, null, null, condition, callback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, null, progress, condition, callback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, progressProvider, progress, null, callback);
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressProvider">自定义的加载进度0-1</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, progressProvider, progress, condition, callback);
        }
        /// <summary>
        /// 异步卸载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine UnloadSceneAsync(ISceneInfo sceneInfo, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.UnloadSceneAsync(sceneInfo, null, null, callback);
        }
        /// <summary>
        /// 异步卸载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine UnloadSceneAsync(ISceneInfo sceneInfo, Action<float> progress, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.UnloadSceneAsync(sceneInfo, progress, null, callback);
        }
        /// <summary>
        ///  异步卸载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine UnloadSceneAsync(ISceneInfo sceneInfo, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.UnloadSceneAsync(sceneInfo, null, condition, callback);
        }
        /// <summary>
        /// 异步卸载场景；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine UnloadSceneAsync(ISceneInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.UnloadSceneAsync(sceneInfo, progress, condition, callback);
        }
        protected override void OnInitialization()
        {
            sceneHelper = new DefalutSceneHelper();
        }
    }
}