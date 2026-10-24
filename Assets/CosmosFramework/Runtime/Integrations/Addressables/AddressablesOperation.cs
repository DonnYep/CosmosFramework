using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AddressablesApi = UnityEngine.AddressableAssets.Addressables;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Cosmos.Resource.Addressables
{
    /// <summary>
    /// Addressables 单资产加载操作。
    /// </summary>
    internal sealed class AddressablesAssetOperation<T> : ResourceProviderOperation
        where T : Object
    {
        AsyncOperationHandle<T> handle;

        public AddressablesAssetOperation(AddressablesProvider provider, ResourceRequest request) : base(provider, request)
        {
        }

        protected override void OnStartLoad()
        {
            handle = AddressablesApi.LoadAssetAsync<T>(AssetInfo.Key);
        }
        protected override void OnUpdateLoad()
        {
            if (!handle.IsDone)
            {
                SetProgress(handle.PercentComplete);
                return;
            }
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                SetAsset(handle.Result);
                Finish();
            }
            else
            {
                Finish(AddressablesUtility.Describe(handle.OperationException));
            }
        }
        protected override void OnRelease()
        {
            if (handle.IsValid())
                AddressablesApi.Release(handle);
        }
    }

    /// <summary>
    /// Addressables 多资产加载操作（子资产 / 全部资产）。
    /// <para>Addressables 没有“子资产”这一独立 API，统一走 LoadAssetsAsync；如需取子资产可在地址后缀 [名称]。</para>
    /// </summary>
    internal sealed class AddressablesAssetsOperation<T> : ResourceProviderOperation
        where T : Object
    {
        AsyncOperationHandle<IList<T>> handle;

        public AddressablesAssetsOperation(AddressablesProvider provider, ResourceRequest request) : base(provider, request)
        {
        }

        protected override void OnStartLoad()
        {
            handle = AddressablesApi.LoadAssetsAsync<T>(AssetInfo.Key, null);
        }
        protected override void OnUpdateLoad()
        {
            if (!handle.IsDone)
            {
                SetProgress(handle.PercentComplete);
                return;
            }
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Finish(AddressablesUtility.Describe(handle.OperationException));
                return;
            }
            var result = handle.Result;
            if (result == null)
            {
                SetAssets(new Object[0]);
                Finish();
                return;
            }
            var assets = new Object[result.Count];
            for (int i = 0; i < result.Count; i++)
                assets[i] = result[i];
            SetAssets(assets);
            Finish();
        }
        protected override void OnRelease()
        {
            if (handle.IsValid())
                AddressablesApi.Release(handle);
        }
    }

    /// <summary>
    /// Addressables 场景加载操作。
    /// <para>释放句柄时调用 AddressablesApi.UnloadSceneAsync，与加载句柄成对出现。</para>
    /// </summary>
    internal sealed class AddressablesSceneOperation : ResourceProviderOperation
    {
        AsyncOperationHandle<SceneInstance> handle;

        public AddressablesSceneOperation(AddressablesProvider provider, ResourceRequest request) : base(provider, request)
        {
        }

        protected override void OnStartLoad()
        {
            handle = AddressablesApi.LoadSceneAsync(AssetInfo.Key, Request.SceneMode, Request.ActivateOnLoad, (int)Priority);
        }
        protected override void OnUpdateLoad()
        {
            if (!handle.IsDone)
            {
                SetProgress(handle.PercentComplete);
                return;
            }
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var scene = handle.Result.Scene;
                SetScene(scene, string.IsNullOrEmpty(AssetInfo.Name) ? scene.name : AssetInfo.Name);
                Finish();
            }
            else
            {
                Finish(AddressablesUtility.Describe(handle.OperationException));
            }
        }
        protected override void OnRelease()
        {
            if (!handle.IsValid())
                return;
            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Scene.IsValid())
                AddressablesApi.UnloadSceneAsync(handle);
            else
                AddressablesApi.Release(handle);
        }
    }

    /// <summary>
    /// Addressables 实例化操作（释放句柄即 ReleaseInstance，实例归还对象池）。
    /// </summary>
    internal sealed class AddressablesInstanceOperation : ResourceProviderOperation
    {
        AsyncOperationHandle<GameObject> handle;

        public AddressablesInstanceOperation(AddressablesProvider provider, ResourceRequest request) : base(provider, request)
        {
        }

        protected override void OnStartLoad()
        {
            handle = AddressablesApi.InstantiateAsync(AssetInfo.Key, Request.InstanceParent);
        }
        protected override void OnUpdateLoad()
        {
            if (!handle.IsDone)
            {
                SetProgress(handle.PercentComplete);
                return;
            }
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                SetInstance(handle.Result);
                Finish();
            }
            else
            {
                Finish(AddressablesUtility.Describe(handle.OperationException));
            }
        }
        protected override void OnRelease()
        {
            if (handle.IsValid())
                AddressablesApi.ReleaseInstance(handle);
        }
    }

    /// <summary>
    /// Addressables 错误信息格式化。
    /// </summary>
    internal static class AddressablesUtility
    {
        public static string Describe(System.Exception exception)
        {
            return exception != null ? exception.Message : "addressables operation failed";
        }

        public static string GetVersion()
        {
            try
            {
                return typeof(AddressablesApi).Assembly.GetName().Version.ToString();
            }
            catch (System.Exception)
            {
                return "unknown";
            }
        }
    }
}
