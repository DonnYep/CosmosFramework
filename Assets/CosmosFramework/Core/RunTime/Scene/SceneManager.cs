using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cosmos.Event;
using System;
using UnityEngine.Events;

namespace Cosmos.Scene
{
    internal sealed class SceneManager : Module<SceneManager>
    {
        /// <summary>
        /// 同步加载 
        /// </summary>
        internal void LoadScene(string sceneName, Action loadedCallBack = null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            loadedCallBack?.Invoke();
        }
        /// <summary>
        /// 同步加载 
        /// </summary>
        internal void LoadScene(string sceneName, bool additive, Action loadedCallBack = null)
        {
            if (additive)
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            loadedCallBack?.Invoke();
        }
        /// <summary>
        /// 同步加载 
        /// </summary>
        internal void LoadScene(int sceneIndex, Action loadedCallBack = null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            loadedCallBack?.Invoke();
        }
        /// <summary>
        /// 同步加载 
        /// </summary>
        internal void LoadScene(int sceneIndex, bool additive, Action loadedCallBack = null)
        {
            if (additive)
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
            loadedCallBack?.Invoke();
        }

        internal void LoadSceneAsync(string sceneName,  Action loadedCallBack = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName,false, loadedCallBack));
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加场景</param>
        /// <param name="loadedCallBack">加载完毕后的回调</param>
        internal void LoadSceneAsync(string sceneName, bool additive, Action loadedCallBack = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName, additive, loadedCallBack));
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="progressCallBack">加载场景进度回调</param>
        /// <param name="loadedCallBack">场景加载完毕回调</param>
        internal void LoadSceneAsync(string sceneName, Action<float> progressCallBack ,Action loadedCallBack=null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName, false,progressCallBack, loadedCallBack));
        }
        internal void LoadSceneAsync(string sceneName, bool additive, Action<float> progressCallBack , Action loadedCallBack=null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName, additive, progressCallBack,loadedCallBack));
        }
        internal void LoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallBack ,Action loadedCallBack=null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName, false,progressCallBack,loadedCallBack));
        }
        internal void LoadSceneAsync(string sceneName, bool additive, Action<AsyncOperation> progressCallback ,Action loadedCallback=null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneName, additive, progressCallback,loadedCallback));
        }
        internal void LoadSceneAsync(int sceneIndex, Action loadedCallback = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex,false, loadedCallback));
        }
        internal void LoadSceneAsync(int sceneIndex, bool additive, Action<float> progressCallback ,Action loadedCallback=null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex, additive, progressCallback,loadedCallback));
        }
        /// <summary>
        /// 异步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="progressCallback"></param>
        internal void LoadSceneAsync(int sceneIndex, Action<float> progressCallback ,Action loadDoneCallBack=null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex,false, progressCallback,loadDoneCallBack));
        }
        internal void LoadSceneAsync(int sceneIndex, bool additive, Action loadedCallBack = null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex, additive, loadedCallBack));
        }
        internal void LoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallback ,Action loadedCallBack=null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex,false, progressCallback,loadedCallBack));
        }
        internal void LoadSceneAsync(int sceneIndex, bool additive, Action<AsyncOperation> progressCallBack ,Action loadedCallBack=null)
        {
            Facade.StartCoroutine(EnumLoadSceneAsync(sceneIndex, additive, progressCallBack,loadedCallBack));
        }
        /// <summary>
        /// 异步加载迭代器 
        /// </summary>
        IEnumerator EnumLoadSceneAsync(string sceneName, bool additive, Action loadedCallBack = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                yield return ao.progress;
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneAsync(string sceneName, bool additive, Action<float> progressCallBack ,Action loadedCallBack=null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao.progress);
                yield return ao.progress;
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneAsync(string sceneName, bool additive, Action<AsyncOperation> progressCallBack ,Action loadedCallBack=null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao);
                yield return ao.progress;
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneAsync(int sceneIndex, bool additive, Action loadedCallBack = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                yield return ao.progress;
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneAsync(int sceneIndex, bool additive, Action<float> progressCallBack ,Action loadedCallBack=null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao.progress);
                yield return ao.progress;
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneAsync(int sceneIndex, bool additive, Action<AsyncOperation> progressCallBack ,Action loadedCallBack=null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao);
                yield return ao.progress;
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
    }
}