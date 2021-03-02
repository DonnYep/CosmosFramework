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
        public Coroutine UnLoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallback, Action unLoadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneOperationAsync(sceneIndex, progressCallback, unLoadedCallback));
        }
        public Coroutine UnLoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallback, Action unLoadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneOperationAsync(sceneName, progressCallback, unLoadedCallback));
        }
        public Coroutine UnLoadSceneAsync(string sceneName, Action<float> progressCallback, Action unLoadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneProgressAsync(sceneName, progressCallback, unLoadedCallback));
        }
        public Coroutine UnLoadSceneAsync(int sceneIndex, Action<float> progressCallback, Action unLoadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneProgressAsync(sceneIndex, progressCallback, unLoadedCallback));
        }
        public Coroutine UnLoadSceneAsync(string sceneName, Action unLoadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneProgressAsync(sceneName, null, unLoadedCallback));
        }
        public Coroutine UnLoadSceneAsync(int sceneIndex, Action unLoadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumUnLoadSceneProgressAsync(sceneIndex, null, unLoadedCallback));
        }
        public Coroutine LoadSceneAsync(string sceneName, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, false, null, loadedCallback));
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="additive">是否叠加场景</param>
        /// <param name="loadedCallback">加载完毕后的回调</param>
        public Coroutine LoadSceneAsync(string sceneName, bool additive, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, additive, null, loadedCallback));
        }
        /// <summary>
        /// 异步加载场景；
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="progressCallback">加载场景进度回调</param>
        /// <param name="loadedCallback">场景加载完毕回调</param>
        public Coroutine LoadSceneAsync(string sceneName, Action<float> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, false, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(string sceneName, bool additive, Action<float> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, additive, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(string sceneName, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneName, false, progressCallback, loadedCallback));
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
        public Coroutine LoadSceneAsync(int sceneIndex, Action<float> progressCallback, Action loadDoneCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, false, progressCallback, loadDoneCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, additive, null, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneIndex, false, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneIndex, additive, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action<float> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, additive, customYield, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(string sceneName, CustomYieldInstruction customYield, Action<float> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, false, customYield, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, additive, customYield, null, loadedCallback));
        }
        public Coroutine LoadSceneAsync(string sceneName,  CustomYieldInstruction customYield, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneName, false, customYield, null, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action<float> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, additive, customYield, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, CustomYieldInstruction customYield, Action<float> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, false, customYield, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, additive, customYield, null, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex,  CustomYieldInstruction customYield, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneProgressAsync(sceneIndex, false, customYield, null, loadedCallback));
        }
        public Coroutine LoadSceneAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneName, additive, customYield, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(string sceneName, CustomYieldInstruction customYield, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneName, false, customYield, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneIndex, additive, customYield, progressCallback, loadedCallback));
        }
        public Coroutine LoadSceneAsync(int sceneIndex, CustomYieldInstruction customYield, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneOperationAsync(sceneIndex, false, customYield, progressCallback, loadedCallback));
        }
        IEnumerator EnumLoadSceneProgressAsync(string sceneName, bool additive, Action<float> progressCallback, Action loadedCallback = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallback?.Invoke();
        }
        IEnumerator EnumLoadSceneOperationAsync(string sceneName, bool additive, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallback?.Invoke();
        }
        IEnumerator EnumLoadSceneProgressAsync(int sceneIndex, bool additive, Action<float> progressCallback, Action loadedCallback = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallback?.Invoke();
        }
        IEnumerator EnumLoadSceneOperationAsync(int sceneIndex, bool additive, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallback?.Invoke();
        }
        IEnumerator EnumUnLoadSceneOperationAsync(int sceneIndex, Action<AsyncOperation> progressCallback, Action unLoadedCallback = null)
        {
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallback?.Invoke();
        }
        IEnumerator EnumUnLoadSceneOperationAsync(string sceneName, Action<AsyncOperation> progressCallback, Action unLoadedCallback = null)
        {
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallback?.Invoke();
        }
        IEnumerator EnumUnLoadSceneProgressAsync(string sceneName, Action<float> progressCallback, Action unLoadedCallback = null)
        {
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallback?.Invoke();
        }
        IEnumerator EnumUnLoadSceneProgressAsync(int sceneIndex, Action<float> progressCallback, Action unLoadedCallback = null)
        {
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallback?.Invoke();
        }
        IEnumerator EnumLoadSceneProgressAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action<float> progressCallback, Action loadedCallback = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            yield return customYield;
            loadedCallback?.Invoke();
        }
        IEnumerator EnumLoadSceneProgressAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action<float> progressCallback, Action loadedCallback = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            yield return customYield;
            loadedCallback?.Invoke();
        }
        IEnumerator EnumLoadSceneOperationAsync(string sceneName, bool additive, CustomYieldInstruction customYield, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            yield return customYield;
            loadedCallback?.Invoke();
        }
        IEnumerator EnumLoadSceneOperationAsync(int sceneIndex, bool additive, CustomYieldInstruction customYield, Action<AsyncOperation> progressCallback, Action loadedCallback = null)
        {
            AsyncOperation ao;
            if (additive)
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            else
                ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            yield return customYield;
            loadedCallback?.Invoke();
        }
    }
}