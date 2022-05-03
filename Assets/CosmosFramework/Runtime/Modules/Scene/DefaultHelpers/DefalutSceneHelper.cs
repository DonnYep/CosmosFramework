using System;
using UnityEngine;
using Cosmos.Scene;
using Cosmos.Resource;
namespace Cosmos
{
    public class DefalutSceneHelper : ISceneHelper
    {
        /// <inheritdoc/>
        public Coroutine LoadSceneAsync(ISceneInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            //TODO DefalutSceneHelper 加载场景需要做GC优化
            return CosmosEntry.ResourceManager.LoadSceneAsync(new SceneAssetInfo(sceneInfo.SceneName, sceneInfo.Additive), progressProvider, progress, condition, callback);
        }
        /// <inheritdoc/>
        public Coroutine UnloadSceneAsync(ISceneInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            return CosmosEntry.ResourceManager.UnloadSceneAsync(new SceneAssetInfo(sceneInfo.SceneName, sceneInfo.Additive), progress, condition, callback);
        }
    }
}
