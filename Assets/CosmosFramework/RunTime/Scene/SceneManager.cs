using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cosmos.Event;
namespace Cosmos.Scene{
    public sealed class SceneManager : Module<SceneManager>
    {
        /// <summary>
        /// 同步加载 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="action"></param>
        public void LoadScene(string sceneName,CFAction action=null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            if(action!=null)
                action();
        }
        /// <summary>
        /// 同步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="action"></param>
        public void LoadScene(int sceneIndex,CFAction action=null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            if (action != null)
                action();
        }
        /// <summary>
        /// 异步加载 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="action"></param>
        public void LoadSceneAsync(string sceneName, CFAction action=null)
        {
            Facade.Instance.StartCoroutine(EnumLoadSceneAsync(sceneName, action));
        }
        /// <summary>
        /// 异步加载 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="action"></param>
        public void LoadSceneAsync(int sceneIndex, CFAction action=null)
        {
            Facade.Instance.StartCoroutine(EnumLoadSceneAsync(sceneIndex, action));
        }
        /// <summary>
        /// 异步加载迭代器 name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator EnumLoadSceneAsync(string sceneName, CFAction action=null)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone)
            {
                ////EventManager.Instance.Fire();
                yield return ao.progress;
            }
            yield return ao.progress;
            if (action != null)
                action();
        }
        /// <summary>
        /// 异步加载迭代器 index
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator EnumLoadSceneAsync(int sceneIndex, CFAction action=null)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!ao.isDone)
            {
                ////EventManager.Instance.Fire();
                yield return ao.progress;
            }
            yield return ao.progress;
            if (action != null)
                action();
        }
    }
}