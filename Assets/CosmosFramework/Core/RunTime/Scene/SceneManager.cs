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
        IMonoManager monoManager;
        public override void OnPreparatory()
        {
            monoManager = GameManager.GetModule<IMonoManager>();
        }
        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加模式</param>
        public void LoadScene(string sceneName, bool additive = false)
        {
            if (additive)
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneIndex">场景序号</param>
        /// <param name="additive">是否叠加模式</param>
        public void LoadScene(int sceneIndex, bool additive = false)
        {
            if (additive)
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
        }
        public Coroutine UnLoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallBack, Action unLoadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneOperationAsync(sceneIndex, progressCallBack, unLoadedCallBack));
        }
        public Coroutine UnLoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallBack, Action unLoadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneOperationAsync(sceneName, progressCallBack, unLoadedCallBack));
        }
        public Coroutine UnLoadSceneAsync(string sceneName, Action<float> progressCallBack, Action unLoadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneProgressAsync(sceneName, progressCallBack, unLoadedCallBack));
        }
        public Coroutine UnLoadSceneAsync(int sceneIndex, Action<float> progressCallBack, Action unLoadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneProgressAsync(sceneIndex, progressCallBack, unLoadedCallBack));
        }
        public Coroutine UnLoadSceneAsync(string sceneName, Action unLoadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneProgressAsync(sceneName, null, unLoadedCallBack));
        }
        public Coroutine UnLoadSceneAsync(int sceneIndex, Action unLoadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneProgressAsync(sceneIndex, null, unLoadedCallBack));
        }
        public Coroutine LoadSceneAsync(string sceneName, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, false, null, loadedCallBack));
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加场景</param>
        /// <param name="loadedCallBack">加载完毕后的回调</param>
        public Coroutine LoadSceneAsync(string sceneName, bool additive, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, additive, null, loadedCallBack));
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="progressCallBack">加载场景进度回调</param>
        /// <param name="loadedCallBack">场景加载完毕回调</param>
        public Coroutine LoadSceneAsync(string sceneName, Action<float> progressCallBack, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, false, progressCallBack, loadedCallBack));
        }
        public Coroutine LoadSceneAsync(string sceneName, bool additive, Action<float> progressCallBack, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, additive, progressCallBack, loadedCallBack));
        }
        public Coroutine LoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallBack, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneName, false, progressCallBack, loadedCallBack));
        }
        public Coroutine LoadSceneAsync(string sceneName, bool additive, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneName, additive, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, false, null, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action<float> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, additive, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, Action<float> progressCallback, Action loadDoneCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, false, progressCallback, loadDoneCallBack));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, additive, null, loadedCallBack));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallback, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneIndex, false, progressCallback, loadedCallBack));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action<AsyncOperation> progressCallBack, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneIndex, additive, progressCallBack, loadedCallBack));
        }
        public Coroutine LoadSceneAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action<float> progressCallBack, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneAsync(sceneName, additive, customYield, progressCallBack, loadedCallBack));
        }
        public Coroutine LoadSceneAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneAsync(sceneName, additive, customYield, null, loadedCallBack));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action<float> progressCallBack, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneAsync(sceneIndex, additive, customYield, progressCallBack, loadedCallBack));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action loadedCallBack = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneAsync(sceneIndex, additive, customYield, null, loadedCallBack));
        }
        IEnumerator EnumLoadSceneProgressAsync(string sceneName, bool additive, Action<float> progressCallBack, Action loadedCallBack = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneOperationAsync(string sceneName, bool additive, Action<AsyncOperation> progressCallBack, Action loadedCallBack = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneProgressAsync(int sceneIndex, bool additive, Action<float> progressCallBack, Action loadedCallBack = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneOperationAsync(int sceneIndex, bool additive, Action<AsyncOperation> progressCallBack, Action loadedCallBack = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumUnLoadSceneOperationAsync(int sceneIndex, Action<AsyncOperation> progressCallBack, Action unLoadedCallBack = null)
        {
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallBack?.Invoke();
        }
        IEnumerator EnumUnLoadSceneOperationAsync(string sceneName, Action<AsyncOperation> progressCallBack, Action unLoadedCallBack = null)
        {
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallBack?.Invoke();
        }
        IEnumerator EnumUnLoadSceneProgressAsync(string sceneName, Action<float> progressCallBack, Action unLoadedCallBack = null)
        {
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallBack?.Invoke();
        }
        IEnumerator EnumUnLoadSceneProgressAsync(int sceneIndex, Action<float> progressCallBack, Action unLoadedCallBack = null)
        {
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action<float> progressCallBack, Action loadedCallBack = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            yield return customYield;
            loadedCallBack?.Invoke();
        }
        IEnumerator EnumLoadSceneAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action<float> progressCallBack, Action loadedCallBack = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallBack?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            yield return customYield;
            loadedCallBack?.Invoke();
        }
    }
}