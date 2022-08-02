using UnityEngine;
using System;
using Cosmos.Resource;

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
        SceneAssetHelper sceneHelper;
        ///<inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, null, null, null, callback);
        }
        ///<inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, null, progress, null, callback);
        }
        ///<inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, null, null, condition, callback);
        }
        ///<inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, null, progress, condition, callback);
        }
        ///<inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, progressProvider, progress, null, callback);
        }
        ///<inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.LoadSceneAsync(sceneInfo, progressProvider, progress, condition, callback);
        }
        ///<inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo sceneInfo, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.UnloadSceneAsync(sceneInfo, null, null, callback);
        }
        ///<inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.UnloadSceneAsync(sceneInfo, progress, null, callback);
        }
        ///<inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo sceneInfo, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.UnloadSceneAsync(sceneInfo, null, condition, callback);
        }
        ///<inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return sceneHelper.UnloadSceneAsync(sceneInfo, progress, condition, callback);
        }
        protected override void OnInitialization()
        {
            sceneHelper = new SceneAssetHelper();
        }
    }
}