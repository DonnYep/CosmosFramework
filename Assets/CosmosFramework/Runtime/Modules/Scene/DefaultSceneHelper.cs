using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cosmos
{
    public class DefaultSceneHelper : ISceneHelper
    {
        public void LoadScene(ISceneInfo sceneInfo)
        {
            if (sceneInfo.Additive)
                SceneManager.LoadScene(sceneInfo.SceneName, LoadSceneMode.Additive);
            else
                SceneManager.LoadScene(sceneInfo.SceneName);
        }
        public IEnumerator LoadSceneAsync(ISceneInfo sceneInfo, Action startLoadCallback, Action<float> progressCallback, Action loadedCallback = null)
        {
            startLoadCallback?.Invoke();
            AsyncOperation ao;
            ao = SceneManager.LoadSceneAsync(sceneInfo.SceneName, (LoadSceneMode)Convert.ToByte(sceneInfo.Additive));
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            loadedCallback?.Invoke();
        }
        public IEnumerator LoadSceneAsync(ISceneInfo sceneInfo, Action startLoadCallback, Action<float> progressCallback, Func<bool> loadedPredicate, Action loadedCallback = null)
        {
            startLoadCallback?.Invoke();
            AsyncOperation ao;
            ao = SceneManager.LoadSceneAsync(sceneInfo.SceneName, (LoadSceneMode)Convert.ToByte(sceneInfo.Additive));
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
        }
        public IEnumerator UnLoadSceneAsync(ISceneInfo sceneInfo, Action startUnloadCallback, Action<float> progressCallback, Action unLoadedCallback = null)
        {
            startUnloadCallback?.Invoke();
            AsyncOperation ao;
            ao = SceneManager.UnloadSceneAsync(sceneInfo.SceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            unLoadedCallback?.Invoke();
        }
        public IEnumerator UnLoadSceneAsync(ISceneInfo sceneInfo, Action startUnloadCallback, Action<float> progressCallback, Func<bool> unLoadedPredicate, Action unLoadedCallback = null)
        {
            startUnloadCallback?.Invoke();
            AsyncOperation ao;
            ao = SceneManager.UnloadSceneAsync(sceneInfo.SceneName);
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
        }
    }
}
