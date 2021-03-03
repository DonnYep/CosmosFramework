using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 加载场景帮助类；
    /// 可实现区分AB加载与BuildScene加载；
    /// </summary>
    public interface ISceneHelper
    {
        void LoadScene(SceneInfo sceneInfo);
        IEnumerator LoadSceneAsync(SceneInfo sceneInfo,Action<float> progressCallback, Action loadedCallback = null);
        IEnumerator LoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Func<bool> loadedPredicate, Action loadedCallback = null);
        IEnumerator UnLoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Action loadedCallback = null);
        IEnumerator UnLoadSceneAsync(SceneInfo sceneInfo, Action<float> progressCallback, Func<bool> loadedPredicate, Action loadedCallback = null);
    }
}
