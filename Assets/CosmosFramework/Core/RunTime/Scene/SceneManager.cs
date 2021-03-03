using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

namespace Cosmos.Scene
{
    [Module]
    internal sealed class SceneManager : Module, ISceneManager
    {
        ISceneHelper sceneHelper;
        IMonoManager monoManager;
        public override void OnPreparatory()
        {
            monoManager = GameManager.GetModule<IMonoManager>();
        }
        public void SetHelper(ISceneHelper sceneHelper)
        {
            this.sceneHelper = sceneHelper;
        }
        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        public void LoadScene(SceneInfo sceneInfo)
        {
            sceneHelper.LoadScene(sceneInfo);
        }
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="loadedCallback">加载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(SceneInfo sceneInfo, Action loadedCallback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return monoManager.StartCoroutine(sceneHelper.LoadSceneAsync(sceneInfo, null, loadedCallback));
        }
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Action loadedCallback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return monoManager.StartCoroutine(sceneHelper.LoadSceneAsync(sceneInfo, progressCallback, loadedCallback));
        }
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="customYield">自定义的yield</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(SceneInfo sceneInfo, Func<bool> loadedPredicate, Action loadedCallback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return monoManager.StartCoroutine(sceneHelper.LoadSceneAsync(sceneInfo, null, loadedPredicate, loadedCallback));
        }
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="customYield">自定义的yield</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        public Coroutine LoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Func<bool> loadedPredicate, Action loadedCallback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return monoManager.StartCoroutine(sceneHelper.LoadSceneAsync(sceneInfo, progressCallback, loadedPredicate, loadedCallback));
        }
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedPredicate">卸载场景完成的条件</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine UnLoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Func<bool> unLoadedPredicate, Action unLoadedCallback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return monoManager.StartCoroutine(sceneHelper.UnLoadSceneAsync(sceneInfo, progressCallback, unLoadedPredicate, unLoadedCallback));
        }
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine UnLoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Action unLoadedCallback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return monoManager.StartCoroutine(sceneHelper.UnLoadSceneAsync(sceneInfo, progressCallback, unLoadedCallback));
        }
        /// <summary>
        ///  异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="unLoadedPredicate">卸载场景完成的条件</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine UnLoadSceneAsync(SceneInfo sceneInfo, Func<bool> unLoadedPredicate, Action unLoadedCallback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return monoManager.StartCoroutine(sceneHelper.UnLoadSceneAsync(sceneInfo, null, unLoadedPredicate, unLoadedCallback));
        }
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        public Coroutine UnLoadSceneAsync(SceneInfo sceneInfo, Action unLoadedCallback = null)
        {
            if (sceneHelper == null)
                throw new ArgumentNullException($"{this.GetType()}: SceneHelper is invalid !");
            return monoManager.StartCoroutine(sceneHelper.UnLoadSceneAsync(sceneInfo, null, unLoadedCallback));
        }
    }
}