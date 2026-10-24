using System;
using Cosmos.Operation;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AddressablesApi = UnityEngine.AddressableAssets.Addressables;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Cosmos.Resource.Addressables
{
    /// <summary>
    /// Addressables 资源 Provider（主推方案）。
    /// <para>映射关系：</para>
    /// <para>· 单资产 → AddressablesApi.LoadAssetAsync&lt;T&gt;；</para>
    /// <para>· 子资产 / 全部资产 → AddressablesApi.LoadAssetsAsync&lt;T&gt;（MergeMode.None）；</para>
    /// <para>· 场景 → AddressablesApi.LoadSceneAsync / UnloadSceneAsync；</para>
    /// <para>· 实例化 → AddressablesApi.InstantiateAsync / ReleaseInstance（带对象池）。</para>
    /// <para>配置：CosmosConfig 的 Provider 填 "Addressables"，settings 可填 "updateCatalog=true"。</para>
    /// </summary>
    public sealed class AddressablesProvider : IResourceProvider
    {
        /// <summary>
        /// 注册名
        /// </summary>
        public const string PROVIDER_NAME = "Addressables";

        ResourceInitParameters initParameters;

        /// <summary>
        /// 程序集加载时自动注册。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            ResourceProviderRegistry.Register(PROVIDER_NAME, () => new AddressablesProvider());
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return PROVIDER_NAME; }
        }
        /// <inheritdoc/>
        public ResourceCapabilities Capabilities
        {
            get
            {
                return ResourceCapabilities.Asset
                    | ResourceCapabilities.SubAssets
                    | ResourceCapabilities.AllAssets
                    | ResourceCapabilities.Scene
                    | ResourceCapabilities.Instance
                    | ResourceCapabilities.Sync
                    | ResourceCapabilities.Priority;
            }
        }
        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }
        /// <inheritdoc/>
        public ResourceInitParameters InitParameters
        {
            get { return initParameters; }
        }

        /// <inheritdoc/>
        public InitializationOperation InitializeAsync(ResourceInitParameters parameters)
        {
            initParameters = parameters ?? new ResourceInitParameters(PROVIDER_NAME);
            var operation = new AddressablesInitializationOperation(this);
            OperationSystem.StartOperation(operation);
            return operation;
        }
        /// <inheritdoc/>
        public ResourceProviderOperation CreateOperation(ResourceRequest request)
        {
            switch (request.LoadMode)
            {
                case AssetLoadMode.Scene:
                    return new AddressablesSceneOperation(this, request);
                case AssetLoadMode.Instance:
                    return new AddressablesInstanceOperation(this, request);
                case AssetLoadMode.SubAssets:
                case AssetLoadMode.AllAssets:
                    return CreateGenericOperation(typeof(AddressablesAssetsOperation<>), request);
                default:
                    return CreateGenericOperation(typeof(AddressablesAssetOperation<>), request);
            }
        }
        /// <inheritdoc/>
        public void UnloadUnusedAssets()
        {
            // Addressables 由引用计数与 AddressablesApi.Release 管理，没有全局 UnloadUnusedAssets
        }
        /// <inheritdoc/>
        public void Destroy()
        {
            IsInitialized = false;
        }

        internal void SetInitialized(bool value)
        {
            IsInitialized = value;
        }

        ResourceProviderOperation CreateGenericOperation(Type openType, ResourceRequest request)
        {
            var assetType = request.AssetType;
            if (assetType == null || assetType == typeof(Scene) || !typeof(Object).IsAssignableFrom(assetType))
                assetType = typeof(Object);
            var closedType = openType.MakeGenericType(assetType);
            return (ResourceProviderOperation)Activator.CreateInstance(closedType, new object[] { this, request });
        }

        /// <summary>
        /// 初始化操作：AddressablesApi.InitializeAsync，可选更新 catalog。
        /// </summary>
        internal sealed class AddressablesInitializationOperation : InitializationOperation
        {
            readonly AddressablesProvider provider;
            AsyncOperationHandle<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator> initializeHandle;
            AsyncOperationHandle<System.Collections.Generic.List<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator>> updateHandle;
            bool needUpdateCatalog;

            public AddressablesInitializationOperation(AddressablesProvider provider) : base(PROVIDER_NAME)
            {
                this.provider = provider;
            }

            protected override void OnStart()
            {
                base.OnStart();
                needUpdateCatalog = provider.InitParameters != null && provider.InitParameters.GetBool("updateCatalog");
                initializeHandle = AddressablesApi.InitializeAsync();
            }

            protected override void OnUpdate()
            {
                if (!initializeHandle.IsDone)
                {
                    Progress = initializeHandle.PercentComplete;
                    return;
                }
                if (initializeHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Fail($"Addressables initialize failed : {initializeHandle.OperationException}");
                    return;
                }
                if (needUpdateCatalog && !updateHandle.IsValid())
                {
                    updateHandle = AddressablesApi.UpdateCatalogs();
                    return;
                }
                if (updateHandle.IsValid() && !updateHandle.IsDone)
                    return;
                provider.SetInitialized(true);
                Complete(AddressablesUtility.GetVersion());
            }
        }
    }
}
