using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace Cosmos.Resource.YooAsset
{
    /// <summary>
    /// YooAsset 单资产加载操作。
    /// </summary>
    internal sealed class YooAssetAssetOperation<T> : ResourceProviderOperation
        where T : Object
    {
        readonly YooAssetProvider provider;
        AssetOperationHandle handle;

        public YooAssetAssetOperation(YooAssetProvider provider, ResourceRequest request) : base(provider, request)
        {
            this.provider = provider;
        }

        protected override void OnStartLoad()
        {
            handle = provider.Package.LoadAssetAsync<T>(AssetInfo.Key);
        }
        protected override void OnUpdateLoad()
        {
            if (handle == null)
            {
                Finish("yooasset asset handle is null");
                return;
            }
            if (!handle.IsDone)
            {
                SetProgress(handle.Progress);
                return;
            }
            if (handle.Status == EOperationStatus.Succeed)
            {
                SetAsset(handle.AssetObject);
                Finish();
            }
            else
            {
                Finish(handle.LastError);
            }
        }
        protected override void OnRelease()
        {
            if (handle != null && handle.IsValid)
                handle.Release();
        }
    }

    /// <summary>
    /// YooAsset 多资产加载操作（子资产 / 全部资产）。
    /// </summary>
    internal sealed class YooAssetAssetsOperation<T> : ResourceProviderOperation
        where T : Object
    {
        readonly YooAssetProvider provider;
        SubAssetsOperationHandle subAssetsHandle;
        AllAssetsOperationHandle allAssetsHandle;

        public YooAssetAssetsOperation(YooAssetProvider provider, ResourceRequest request) : base(provider, request)
        {
            this.provider = provider;
        }

        protected override void OnStartLoad()
        {
            if (LoadMode == AssetLoadMode.SubAssets)
                subAssetsHandle = provider.Package.LoadSubAssetsAsync<T>(AssetInfo.Key);
            else
                allAssetsHandle = provider.Package.LoadAllAssetsAsync<T>(AssetInfo.Key);
        }
        protected override void OnUpdateLoad()
        {
            if (LoadMode == AssetLoadMode.SubAssets)
            {
                if (subAssetsHandle == null)
                {
                    Finish("yooasset sub assets handle is null");
                    return;
                }
                if (!subAssetsHandle.IsDone)
                {
                    SetProgress(subAssetsHandle.Progress);
                    return;
                }
                if (subAssetsHandle.Status == EOperationStatus.Succeed)
                {
                    SetAssets(subAssetsHandle.AllAssetObjects);
                    Finish();
                }
                else
                {
                    Finish(subAssetsHandle.LastError);
                }
                return;
            }
            if (allAssetsHandle == null)
            {
                Finish("yooasset all assets handle is null");
                return;
            }
            if (!allAssetsHandle.IsDone)
            {
                SetProgress(allAssetsHandle.Progress);
                return;
            }
            if (allAssetsHandle.Status == EOperationStatus.Succeed)
            {
                SetAssets(allAssetsHandle.AllAssetObjects);
                Finish();
            }
            else
            {
                Finish(allAssetsHandle.LastError);
            }
        }
        protected override void OnRelease()
        {
            if (subAssetsHandle != null && subAssetsHandle.IsValid)
                subAssetsHandle.Release();
            if (allAssetsHandle != null && allAssetsHandle.IsValid)
                allAssetsHandle.Release();
        }
    }

    /// <summary>
    /// YooAsset 场景加载操作。
    /// <para>释放句柄时调用 SceneOperationHandle.UnloadAsync。</para>
    /// </summary>
    internal sealed class YooAssetSceneOperation : ResourceProviderOperation
    {
        readonly YooAssetProvider provider;
        SceneOperationHandle handle;

        public YooAssetSceneOperation(YooAssetProvider provider, ResourceRequest request) : base(provider, request)
        {
            this.provider = provider;
        }

        protected override void OnStartLoad()
        {
            handle = provider.Package.LoadSceneAsync(AssetInfo.Key, Request.SceneMode, false, (int)Priority);
        }
        protected override void OnUpdateLoad()
        {
            if (handle == null)
            {
                Finish("yooasset scene handle is null");
                return;
            }
            if (!handle.IsDone)
            {
                SetProgress(handle.Progress);
                return;
            }
            if (handle.Status != EOperationStatus.Succeed)
            {
                Finish(handle.LastError);
                return;
            }
            if (Request.ActivateOnLoad)
                handle.ActivateScene();
            var scene = handle.SceneObject;
            SetScene(scene, string.IsNullOrEmpty(AssetInfo.Name) ? scene.name : AssetInfo.Name);
            Finish();
        }
        protected override void OnRelease()
        {
            if (handle != null && handle.IsValid)
                handle.UnloadAsync();
        }
    }
}
