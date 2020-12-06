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
        void LoadScene(string sceneName, Action loadedCallBack = null);
        /// <summary>
        /// 同步加载 
        /// </summary>
        void LoadScene(string sceneName, bool additive, Action loadedCallBack = null);
        /// <summary>
        /// 同步加载 
        /// </summary>
        void LoadScene(int sceneIndex, Action loadedCallBack = null);
        /// <summary>
        /// 同步加载 
        /// </summary>
        void LoadScene(int sceneIndex, bool additive, Action loadedCallBack = null);
        Coroutine UnLoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallBack, Action unLoadedCallBack = null);
        Coroutine UnLoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallBack, Action unLoadedCallBack = null);
        Coroutine UnLoadSceneAsync(string sceneName, Action<float> progressCallBack, Action unLoadedCallBack = null);
        Coroutine UnLoadSceneAsync(int sceneIndex, Action<float> progressCallBack, Action unLoadedCallBack = null);
        Coroutine UnLoadSceneAsync(string sceneName, Action unLoadedCallBack = null);
        Coroutine UnLoadSceneAsync(int sceneIndex, Action unLoadedCallBack = null);
        Coroutine LoadSceneAsync(string sceneName, Action loadedCallBack = null);
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加场景</param>
        /// <param name="loadedCallBack">加载完毕后的回调</param>
        Coroutine LoadSceneAsync(string sceneName, bool additive, Action loadedCallBack = null);
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="progressCallBack">加载场景进度回调</param>
        /// <param name="loadedCallBack">场景加载完毕回调</param>
        Coroutine LoadSceneAsync(string sceneName, Action<float> progressCallBack, Action loadedCallBack = null);
        Coroutine LoadSceneAsync(string sceneName, bool additive, Action<float> progressCallBack, Action loadedCallBack = null);
        Coroutine LoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallBack, Action loadedCallBack = null);
        Coroutine LoadSceneAsync(string sceneName, bool additive, Action<AsyncOperation> progressCallback, Action loadedCallback = null);
        Coroutine LoadSceneAsync(int sceneIndex, Action loadedCallback = null);
        Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action<float> progressCallback, Action loadedCallback = null);
        /// <summary>
        /// 异步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="progressCallback"></param>
        Coroutine LoadSceneAsync(int sceneIndex, Action<float> progressCallback, Action loadDoneCallBack = null);
        Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action loadedCallBack = null);
        Coroutine LoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallback, Action loadedCallBack = null);
        Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action<AsyncOperation> progressCallBack, Action loadedCallBack = null);
    }
}
