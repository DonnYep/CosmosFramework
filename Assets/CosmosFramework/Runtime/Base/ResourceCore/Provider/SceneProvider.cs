using Cosmos.Operation;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cosmos.Resource
{
    /// <summary>
    /// 场景资源提供器。
    /// </summary>
    internal class SceneProvider : ProviderBase
    {
        readonly ResourcePackage package;
        readonly bool additive;
        readonly bool activateOnLoad;
        BundleLoader pendingLoader;
        AsyncOperation sceneRequest;

        internal SceneProvider(ResourcePackage package, AssetInfo assetInfo, bool additive, bool activateOnLoad)
        {
            this.package = package;
            this.MainAssetInfo = assetInfo;
            this.LoadMode = AssetLoadMode.Scene;
            this.additive = additive;
            this.activateOnLoad = activateOnLoad;
            SceneName = assetInfo.Name;
        }

        protected override void OnStart()
        {
#if UNITY_EDITOR
            if (CFResources.ResourcePlayMode == CFResourcePlayMode.AssetDatabase)
            {
                var loadParameters = new LoadSceneParameters(additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                var scene = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(MainAssetInfo.AssetPath, loadParameters);
                SceneObject = scene;
                if (scene.IsValid())
                    InvokeCompletion(string.Empty, OperationStatus.Succeeded);
                else
                    InvokeCompletion($"Failed to load scene : {MainAssetInfo.AssetPath}", OperationStatus.Failed);
                return;
            }
#endif
            var bundleName = MainAssetInfo.BundleName;
            if (string.IsNullOrEmpty(bundleName))
            {
                InvokeCompletion($"Bundle name is empty for scene : {MainAssetInfo.AssetPath}", OperationStatus.Failed);
                return;
            }
            pendingLoader = package.GetBundles().LoadBundle(bundleName);
            if (pendingLoader == null)
            {
                InvokeCompletion($"Bundle not found : {bundleName}", OperationStatus.Failed);
            }
        }

        protected override void OnUpdate()
        {
            if (IsDone || IsDestroyed)
                return;
            if (pendingLoader != null)
            {
                if (!pendingLoader.IsDone)
                {
                    SetWaitForAsyncComplete();
                    Progress = pendingLoader.Progress * 0.5f;
                    return;
                }
                if (pendingLoader.Status != OperationStatus.Succeeded || pendingLoader.AssetBundle == null)
                {
                    InvokeCompletion($"Failed to load scene bundle : {pendingLoader.BundleName}", OperationStatus.Failed);
                    return;
                }
                StartSceneRequest();
                pendingLoader = null;
            }
            if (sceneRequest != null)
            {
                if (!sceneRequest.isDone)
                {
                    Progress = 0.5f + sceneRequest.progress * 0.5f;
                    return;
                }
                Progress = 1;
                SceneObject = SceneManager.GetSceneByName(SceneName);
                if (SceneObject.IsValid())
                    InvokeCompletion(string.Empty, OperationStatus.Succeeded);
                else
                    InvokeCompletion($"Scene loaded but not found : {SceneName}", OperationStatus.Failed);
            }
        }

        void StartSceneRequest()
        {
            sceneRequest = SceneManager.LoadSceneAsync(MainAssetInfo.AssetPath, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            if (sceneRequest == null)
            {
                InvokeCompletion($"Scene not found : {MainAssetInfo.AssetPath}", OperationStatus.Failed);
                return;
            }
            sceneRequest.allowSceneActivation = activateOnLoad;
        }

        protected override void OnDestroy()
        {
            if (pendingLoader != null)
            {
                package.GetBundles().UnloadBundle(pendingLoader);
                pendingLoader = null;
            }
        }

        internal override void WaitForCompletion()
        {
            int frame = 1000;
            while (!IsDone && frame-- > 0)
            {
                OperationSystem.Update();
                OnUpdate();
            }
        }
    }
}
