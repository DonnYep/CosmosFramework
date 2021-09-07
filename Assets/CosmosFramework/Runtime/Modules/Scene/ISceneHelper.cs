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
        /// <summary>
        /// 是否正在加载；
        /// </summary>
        bool IsLoading { get; }
        void LoadScene(ISceneInfo sceneInfo);
        IEnumerator LoadSceneAsync(ISceneInfo sceneInfo,Action startLoadCallback,Action<float> progressCallback, Action loadedCallback = null);
        IEnumerator LoadSceneAsync(ISceneInfo sceneInfo, Action startLoadCallback, Action<float> progressCallback, Func<bool> loadedPredicate, Action loadedCallback = null);
        IEnumerator UnLoadSceneAsync(ISceneInfo sceneInfo, Action startUnloadCallback, Action<float> progressCallback, Action unLoadedCallback = null);
        IEnumerator UnLoadSceneAsync(ISceneInfo sceneInfo, Action startUnloadCallback,Action<float> progressCallback, Func<bool> unLoadedPredicate, Action unLoadedCallback = null);
    }
}
