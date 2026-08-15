using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cosmos.Resource
{
    /// <summary>
    /// 卸载场景操作。
    /// </summary>
    public class UnloadSceneOperation : OperationBase
    {
        readonly string sceneName;
        AsyncOperation unloadRequest;

        internal UnloadSceneOperation(string sceneName)
        {
            this.sceneName = sceneName;
        }

        protected override void OnStart()
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid())
            {
                Error = $"Scene not found : {sceneName}";
                Status = OperationStatus.Failed;
                return;
            }
            unloadRequest = SceneManager.UnloadSceneAsync(sceneName);
            if (unloadRequest == null)
            {
                Error = $"Failed to unload scene : {sceneName}";
                Status = OperationStatus.Failed;
            }
        }

        protected override void OnUpdate()
        {
            if (IsDone)
                return;
            if (unloadRequest == null)
            {
                Status = OperationStatus.Failed;
                return;
            }
            if (unloadRequest.isDone)
            {
                Progress = 1;
                Status = OperationStatus.Succeeded;
            }
            else
            {
                Progress = unloadRequest.progress;
            }
        }
    }
}
