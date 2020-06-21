using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cosmos.Event;
namespace Cosmos.Scene
{
    internal sealed class SceneManager : Module<SceneManager>
    {
        /// <summary>
        /// 同步加载 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="callBack"></param>
       internal void LoadScene(string sceneName, CFAction callBack = null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            callBack?.Invoke();
        }
        /// <summary>
        /// 同步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="callBack"></param>
        internal void LoadScene(int sceneIndex, CFAction callBack = null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            callBack?.Invoke();
        }
        /// <summary>
        /// 异步加载 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="callBack"></param>
       internal void LoadSceneAsync(string sceneName)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName));
        }
        internal void LoadSceneAsync(string sceneName, CFAction callBack = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName, callBack));
        }
       internal void LoadSceneAsync(string sceneName, CFAction<float> callBack = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName, callBack));
        }
       internal void LoadSceneAsync(string sceneName, CFAction<AsyncOperation> callBack = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName, callBack));
        }
       internal void LoadSceneAsync(int sceneIndex)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex));
        }
        /// <summary>
        /// 异步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="callBack"></param>
       internal void LoadSceneAsync(int sceneIndex, CFAction callBack = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex, callBack));
        }
       internal void LoadSceneAsync(int sceneIndex, CFAction<float> callBack = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex, callBack));
        }
       internal void LoadSceneAsync(int sceneIndex, CFAction<AsyncOperation> callBack = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex, callBack));
        }
        /// <summary>
        /// 异步加载迭代器 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator EnumLoadSceneAsync(string sceneName, CFAction callBack = null)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                yield return ao.progress;
            }
            yield return ao.isDone;
            callBack?.Invoke();
        }
        IEnumerator EnumLoadSceneAsync(string sceneName)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                yield return ao.progress;
            }
            yield return ao.progress;
        }
        IEnumerator EnumLoadSceneAsync(string sceneName, CFAction<float> callBack = null)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                callBack?.Invoke(ao.progress);
                yield return ao.progress;
            }
            yield return null;
        }
        IEnumerator EnumLoadSceneAsync(string sceneName, CFAction<AsyncOperation> callBack = null)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                callBack?.Invoke(ao);
                yield return null;
                //yield return ao.progress;
            }
        }
        /// <summary>
        /// 异步加载迭代器 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator EnumLoadSceneAsync(int sceneIndex, CFAction callBack = null)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                yield return ao.progress;
            }
            yield return ao.progress;
            callBack?.Invoke();
        }
        IEnumerator EnumLoadSceneAsync(int sceneIndex)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                yield return ao.progress;
            }
            yield return ao.progress;
        }
        IEnumerator EnumLoadSceneAsync(int sceneIndex, CFAction<float> callBack = null)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                callBack?.Invoke(ao.progress);
                yield return null;
                //yield return ao.progress;
            }
            //yield return null;
        }
        IEnumerator EnumLoadSceneAsync(int sceneIndex, CFAction<AsyncOperation> callBack = null)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                callBack?.Invoke(ao);
                yield return null;
                //yield return ao.progress;
            }
            //yield return null;
        }
    }
}