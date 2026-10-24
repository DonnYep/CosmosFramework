using System;
using Cosmos.Operation;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace Cosmos.Resource.YooAsset
{
    /// <summary>
    /// YooAsset 资源 Provider。
    /// <para>映射关系：</para>
    /// <para>· 单资产 → ResourcePackage.LoadAssetAsync&lt;T&gt;（AssetOperationHandle）；</para>
    /// <para>· 子资产 → LoadSubAssetsAsync&lt;T&gt;；全部资产 → LoadAllAssetsAsync&lt;T&gt;；</para>
    /// <para>· 场景 → LoadSceneAsync / SceneOperationHandle.UnloadAsync；</para>
    /// <para>· 卸载 → handle.Release()。</para>
    /// <para>配置：Provider 填 "YooAsset"，settings 示例：package=DefaultPackage;mode=offline</para>
    /// <para>mode 支持：editor（模拟）、offline（单机）；host 模式需要自定义 IRemoteServices，请自行扩展。</para>
    /// <para>注意：本适配器按 ResourcePackage + *OperationHandle 的 2.x 风格 API 编写。</para>
    /// </summary>
    public sealed class YooAssetProvider : IResourceProvider
    {
        /// <summary>
        /// 注册名
        /// </summary>
        public const string PROVIDER_NAME = "YooAsset";
        /// <summary>
        /// 默认包名
        /// </summary>
        public const string DEFAULT_PACKAGE_NAME = "DefaultPackage";

        ResourceInitParameters initParameters;
        ResourcePackage package;
        string packageName = DEFAULT_PACKAGE_NAME;

        /// <summary>
        /// 程序集加载时自动注册。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            ResourceProviderRegistry.Register(PROVIDER_NAME, () => new YooAssetProvider());
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
        /// <summary>
        /// 当前资源包。
        /// </summary>
        public ResourcePackage Package
        {
            get { return package; }
        }
        /// <summary>
        /// 当前包名。
        /// </summary>
        public string PackageName
        {
            get { return packageName; }
        }

        /// <inheritdoc/>
        public InitializationOperation InitializeAsync(ResourceInitParameters parameters)
        {
            initParameters = parameters ?? new ResourceInitParameters(PROVIDER_NAME);
            packageName = initParameters.GetString("package", initParameters.PackageName);
            if (string.IsNullOrEmpty(packageName))
                packageName = DEFAULT_PACKAGE_NAME;
            var operation = new YooAssetInitializationOperation(this);
            OperationSystem.StartOperation(operation);
            return operation;
        }
        /// <inheritdoc/>
        public ResourceProviderOperation CreateOperation(ResourceRequest request)
        {
            if (package == null)
            {
                Debug.LogError("[Resource][YooAsset] package is not initialized !");
                return null;
            }
            switch (request.LoadMode)
            {
                case AssetLoadMode.Scene:
                    return new YooAssetSceneOperation(this, request);
                case AssetLoadMode.SubAssets:
                case AssetLoadMode.AllAssets:
                    return CreateGenericOperation(typeof(YooAssetAssetsOperation<>), request);
                default:
                    return CreateGenericOperation(typeof(YooAssetAssetOperation<>), request);
            }
        }
        /// <inheritdoc/>
        public void UnloadUnusedAssets()
        {
            package?.UnloadUnusedAssets();
        }
        /// <inheritdoc/>
        public void Destroy()
        {
            if (package != null && YooAssets.HasPackage(packageName))
                YooAssets.DestroyPackage(packageName);
            package = null;
            IsInitialized = false;
        }

        internal static InitializeParameters BuildInitializeParameters(ResourceInitParameters parameters)
        {
            var mode = (parameters != null ? parameters.GetString("mode", "offline") : "offline").ToLowerInvariant();
            switch (mode)
            {
                case "editor":
                case "editorsimulate":
                case "simulate":
                    return new EditorSimulateModeParameters()
                    {
                        SimulateManifestFilePath = parameters.GetString("manifest", "Assets/StreamingAssets/yoo/SimulateManifest.json"),
                    };
                case "host":
                    throw new NotSupportedException("YooAsset host mode requires a custom IRemoteServices implementation, please extend this adapter.");
                default:
                    return new OfflinePlayModeParameters();
            }
        }

        internal void SetPackage(ResourcePackage value)
        {
            package = value;
        }
        internal void SetInitialized(bool value)
        {
            IsInitialized = value;
        }

        ResourceProviderOperation CreateGenericOperation(Type openType, ResourceRequest request)
        {
            var assetType = request.AssetType;
            if (assetType == null || !typeof(Object).IsAssignableFrom(assetType))
                assetType = typeof(Object);
            var closedType = openType.MakeGenericType(assetType);
            return (ResourceProviderOperation)Activator.CreateInstance(closedType, new object[] { this, request });
        }

        /// <summary>
        /// 初始化操作。
        /// </summary>
        internal sealed class YooAssetInitializationOperation : InitializationOperation
        {
            readonly YooAssetProvider provider;
            global::YooAsset.InitializationOperation initializeOperation;

            public YooAssetInitializationOperation(YooAssetProvider provider) : base(PROVIDER_NAME)
            {
                this.provider = provider;
            }

            protected override void OnStart()
            {
                base.OnStart();
                try
                {
                    YooAssets.Initialize();
                    if (!YooAssets.HasPackage(provider.PackageName))
                        YooAssets.CreatePackage(provider.PackageName);
                    provider.SetPackage(YooAssets.GetPackage(provider.PackageName));
                    initializeOperation = provider.Package.InitializeAsync(YooAssetProvider.BuildInitializeParameters(provider.InitParameters));
                }
                catch (Exception e)
                {
                    Fail($"YooAsset initialize exception : {e.Message}");
                }
            }

            protected override void OnUpdate()
            {
                if (initializeOperation == null)
                    return;
                if (!initializeOperation.IsDone)
                {
                    Progress = initializeOperation.Progress;
                    return;
                }
                if (initializeOperation.Status != EOperationStatus.Succeed)
                {
                    Fail($"YooAsset initialize failed : {initializeOperation.Error}");
                    return;
                }
                provider.SetInitialized(true);
                Complete();
            }
        }
    }
}
