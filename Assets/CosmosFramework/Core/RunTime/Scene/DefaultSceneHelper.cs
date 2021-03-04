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
        public void LoadScene(SceneInfo sceneInfo)
        {
            if (sceneInfo.Additive)
                SceneManager.LoadScene(sceneInfo.SceneName, LoadSceneMode.Additive);
            else
                SceneManager.LoadScene(sceneInfo.SceneName);
        }
        public IEnumerator LoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Action loadedCallback = null)
        {
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
        public IEnumerator LoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Func<bool> loadedPredicate, Action loadedCallback = null)
        {
            AsyncOperation ao;
            ao = SceneManager.LoadSceneAsync(sceneInfo.SceneName, (LoadSceneMode)Convert.ToByte(sceneInfo.Additive));
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            yield return loadedPredicate;
            loadedCallback?.Invoke();
        }
        public IEnumerator UnLoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Action unLoadedCallback = null)
        {
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
        public IEnumerator UnLoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Func<bool> unLoadedPredicate, Action unLoadedCallback = null)
        {
            AsyncOperation ao;
            ao = SceneManager.UnloadSceneAsync(sceneInfo.SceneName);
            while (!ao.isDone)
            {
                progressCallback?.Invoke(ao.progress);
                yield return new WaitForEndOfFrame();
            }
            yield return ao.isDone;
            yield return unLoadedPredicate;
            unLoadedCallback?.Invoke();
        }
    }
}
