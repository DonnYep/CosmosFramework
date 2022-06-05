using System;
using UnityEngine;
using Cosmos.Scene;
using Cosmos.Resource;
namespace Cosmos
{
    public class DefalutSceneHelper : ISceneHelper
    {
        /// <inheritdoc/>
        public Coroutine LoadSceneAsync(SceneInfo sceneInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            return CosmosEntry.ResourceManager.LoadSceneAsync(new SceneAssetInfo(sceneInfo.SceneName, sceneInfo.Additive), progressProvider, progress, condition, callback);
        }
        /// <inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneInfo sceneInfo, Action<float> progress, Func<bool> condition, Action callback = null)
        {
            return CosmosEntry.ResourceManager.UnloadSceneAsync(new SceneAssetInfo(sceneInfo.SceneName, sceneInfo.Additive), progress, condition, callback);
        }
    }
}
