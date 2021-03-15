using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Cosmos
{
    public interface ISceneManager : IModuleManager
    {
        void SetHelper(ISceneHelper sceneHelper);
        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        void LoadScene(ISceneInfo sceneInfo);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="loadedCallback">加载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="startLoadCallback">开始加载回调</param>
        /// <param name="loadedCallback">加载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action startLoadCallback, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action<float> progressCallback, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="startLoadCallback">开始加载回调</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action startLoadCallback, Action<float> progressCallback, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="loadedPredicate">场景加载完成的条件</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Func<bool> loadedPredicate, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="startLoadCallback">开始加载回调</param>
        /// <param name="loadedPredicate">场景加载完成的条件</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action startLoadCallback, Func<bool> loadedPredicate, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedPredicate">场景加载完成的条件</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action<float> progressCallback, Func<bool> loadedPredicate, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="startLoadCallback">开始加载回调</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedPredicate">场景加载完成的条件</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Action startLoadCallback, Action<float> progressCallback, Func<bool> loadedPredicate, Action loadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedPredicate">卸载场景完成的条件</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(ISceneInfo sceneInfo, Action<float> progressCallback, Func<bool> unLoadedPredicate, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="startUnloadCallback">开始卸载</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedPredicate">卸载场景完成的条件</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(ISceneInfo sceneInfo, Action startUnloadCallback, Action<float> progressCallback, Func<bool> unLoadedPredicate, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(ISceneInfo sceneInfo, Action<float> progressCallback, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="startUnloadCallback">开始卸载</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(ISceneInfo sceneInfo, Action startUnloadCallback, Action<float> progressCallback, Action unLoadedCallback = null);
        /// <summary>
        ///  异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="unLoadedPredicate">卸载场景完成的条件</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(ISceneInfo sceneInfo, Func<bool> unLoadedPredicate, Action unLoadedCallback = null);
        /// <summary>
        ///  异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="startUnloadCallback">开始卸载</param>
        /// <param name="unLoadedPredicate">卸载场景完成的条件</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(ISceneInfo sceneInfo, Action startUnloadCallback, Func<bool> unLoadedPredicate, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(ISceneInfo sceneInfo, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneInfo">场景信息</param>
        /// <param name="startUnloadCallback">开始卸载</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(ISceneInfo sceneInfo, Action startUnloadCallback, Action unLoadedCallback = null);
    }
}
