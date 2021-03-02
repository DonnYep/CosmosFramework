using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Cosmos
{
    public interface ISceneManager:IModuleManager
    {
        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加模式</param>
        void LoadScene(string sceneName, bool additive = false);
        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="additive">是否叠加模式</param>
        void LoadScene(int sceneIndex, bool additive = false);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallback, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallback, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(string sceneName, Action<float> progressCallback, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="progressCallback">卸载场景的进度</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(int sceneIndex, Action<float> progressCallback, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(string sceneName, Action unLoadedCallback = null);
        /// <summary>
        /// 异步卸载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="unLoadedCallback">场景卸载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine UnLoadSceneAsync(int sceneIndex, Action unLoadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="loadedCallback">加载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(string sceneName, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加场景</param>
        /// <param name="loadedCallback">加载完毕后的回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(string sceneName, bool additive, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(string sceneName, Action<float> progressCallback, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加模式</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(string sceneName, bool additive, Action<float> progressCallback, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallback, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加模式</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(string sceneName, bool additive, Action<AsyncOperation> progressCallback, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(int sceneIndex, Action loadedCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="additive">是否叠加模式</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action<float> progressCallback, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(int sceneIndex, Action<float> progressCallback, Action loadDoneCallback = null);
        /// <summary>
        ///  异步加载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="additive">是否叠加模式</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallback, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="additive">是否叠加模式</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action<AsyncOperation> progressCallback, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加模式</param>
        /// <param name="customYield">自定义的yield</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action<float> progressCallback, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加模式</param>
        /// <param name="customYield">自定义的yield</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="customYield">自定义的yield</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(string sceneName, CustomYieldInstruction customYield, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="additive">是否叠加模式</param>
        /// <param name="customYield">自定义的yield</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action<float> progressCallback, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="additive">是否叠加模式</param>
        /// <param name="customYield">自定义的yield</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action loadedCallback = null);
        /// <summary>
        /// 异步加载；
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="customYield">自定义的yield</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        Coroutine LoadSceneAsync(int sceneIndex, CustomYieldInstruction customYield, Action loadedCallBack = null);

    }
}
