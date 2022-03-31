using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cosmos.Scene;
namespace Cosmos
{
    public class DefaultSceneHelper : ISceneHelper
    {
        /// <summary>
        /// 是否正在加载；
        /// </summary>
        public bool IsLoading { get; private set; }
        public void LoadScene(ISceneInfo sceneInfo)
        {
            if (sceneInfo.Additive)
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneInfo.SceneName, LoadSceneMode.Additive);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneInfo.SceneName);
        }
        public IEnumerator LoadSceneAsync(ISceneInfo sceneInfo, Action<float> progressCallback, Action loadedCallback = null)
        {
            IsLoading = true;
            var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneInfo.SceneName, (LoadSceneMode)Convert.ToByte(sceneInfo.Additive));
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallback?.Invoke();
            IsLoading = false;
        }
        public IEnumerator LoadSceneAsync(ISceneInfo sceneInfo, Action<float> progressCallback, Func<bool> loadedPredicate, Action loadedCallback = null)
        {
            IsLoading = true;
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneInfo.SceneName, (LoadSceneMode)Convert.ToByte(sceneInfo.Additive));
            ao.allowSceneActivation = false;
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                if (ao.progress >= 0.9f)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitUntil(loadedPredicate);
            ao.allowSceneActivation = true;
            loadedCallback?.Invoke();
            IsLoading = false;
        }
        public IEnumerator UnLoadSceneAsync(ISceneInfo sceneInfo, Action<float> progressCallback, Action unLoadedCallback = null)
        {
            IsLoading = true;
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneInfo.SceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallback?.Invoke();
            IsLoading = false;
        }
        public IEnumerator UnLoadSceneAsync(ISceneInfo sceneInfo, Action<float> progressCallback, Func<bool> unLoadedPredicate, Action unLoadedCallback = null)
        {
            IsLoading = true;
            AsyncOperation ao;
            ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneInfo.SceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                if (ao.progress >= 0.9f)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitUntil(unLoadedPredicate);
            unLoadedCallback?.Invoke();
            IsLoading = false;
        }
    }
}
