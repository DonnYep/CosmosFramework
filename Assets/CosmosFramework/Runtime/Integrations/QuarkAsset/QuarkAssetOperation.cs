using UnityEngine;
using UnityEngine.SceneManagement;
using Quark;
using Object = UnityEngine.Object;

namespace Cosmos.Resource.QuarkAsset
{
    /// <summary>
    /// QuarkAsset 单资产加载操作。
    /// </summary>
    internal sealed class QuarkAssetOperation<T> : ResourceProviderOperation
        where T : Object
    {
        bool loaded;

        public QuarkAssetOperation(QuarkAssetProvider provider, ResourceRequest request) : base(provider, request)
        {
        }

        protected override void OnStartLoad()
        {
            var key = AssetInfo.Key;
            QuarkResources.LoadAssetAsync<T>(key, asset =>
            {
                if (asset == null)
                {
                    Finish($"asset not found : {key}");
                    return;
                }
                loaded = true;
                SetAsset(asset);
                Finish();
            });
        }
        protected override void OnRelease()
        {
            if (loaded)
                QuarkResources.UnloadAsset(AssetInfo.Key);
        }
    }

    /// <summary>
    /// QuarkAsset 子资产加载操作。
    /// </summary>
    internal sealed class QuarkSubAssetsOperation<T> : ResourceProviderOperation
        where T : Object
    {
        bool loaded;

        public QuarkSubAssetsOperation(QuarkAssetProvider provider, ResourceRequest request) : base(provider, request)
        {
        }

        protected override void OnStartLoad()
        {
            var key = AssetInfo.Key;
            QuarkResources.LoadMainAndSubAssetsAsync<T>(key, assets =>
            {
                if (assets == null || assets.Length == 0)
                {
                    Finish($"sub assets not found : {key}");
                    return;
                }
                loaded = true;
                SetAssets(assets);
                Finish();
            });
        }
        protected override void OnRelease()
        {
            if (loaded)
                QuarkResources.UnloadAsset(AssetInfo.Key);
        }
    }

    /// <summary>
    /// QuarkAsset 全部资产加载操作（按包体名）。
    /// </summary>
    internal sealed class QuarkAllAssetsOperation : ResourceProviderOperation
    {
        bool loaded;

        public QuarkAllAssetsOperation(QuarkAssetProvider provider, ResourceRequest request) : base(provider, request)
        {
        }

        protected override void OnStartLoad()
        {
            var key = AssetInfo.Key;
            QuarkResources.LoadAllAssetAsync(key, assets =>
            {
                if (assets == null || assets.Length == 0)
                {
                    Finish($"assets not found : {key}");
                    return;
                }
                loaded = true;
                SetAssets(assets);
                Finish();
            });
        }
        protected override void OnRelease()
        {
            if (loaded)
                QuarkResources.UnloadAsset(AssetInfo.Key);
        }
    }

    /// <summary>
    /// QuarkAsset 场景加载操作。
    /// </summary>
    internal sealed class QuarkSceneOperation : ResourceProviderOperation
    {
        string sceneName;

        public QuarkSceneOperation(QuarkAssetProvider provider, ResourceRequest request) : base(provider, request)
        {
        }

        protected override void OnStartLoad()
        {
            sceneName = string.IsNullOrEmpty(AssetInfo.Name) ? AssetInfo.Key : AssetInfo.Name;
            QuarkResources.LoadSceneAsync(sceneName, SetProgress, OnSceneLoaded, Request.SceneMode == LoadSceneMode.Additive);
        }

        void OnSceneLoaded()
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid())
            {
                Finish($"scene loaded but not found : {sceneName}");
                return;
            }
            SetScene(scene, sceneName);
            Finish();
        }

        protected override void OnRelease()
        {
            if (string.IsNullOrEmpty(sceneName))
                return;
            QuarkResources.UnloadSceneAsync(sceneName, null, null);
        }
    }
}
