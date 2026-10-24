using System;
using Cosmos.Operation;
using Quark;
using Quark.Asset;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos.Resource.QuarkAsset
{
    /// <summary>
    /// QuarkAsset 资源 Provider。
    /// <para>Quark 本身就是“按名字加载 + 名字卸载”的引用计数模型，与本框架资源桥的 name-keyed 兼容层天然对应。</para>
    /// <para>映射关系：</para>
    /// <para>· 单资产 → QuarkResources.LoadAssetAsync&lt;T&gt;；释放 → QuarkResources.UnloadAsset(key)；</para>
    /// <para>· 子资产 → LoadMainAndSubAssetsAsync&lt;T&gt;；全部资产 → LoadAllAssetAsync(bundleName)；</para>
    /// <para>· 场景 → LoadSceneAsync / UnloadSceneAsync。</para>
    /// <para>配置：Provider 填 "QuarkAsset"，settings 示例：</para>
    /// <para>· AssetBundle：mode=assetbundle;path=&lt;目录&gt;;aesKey=xxx;offset=0</para>
    /// <para>· AssetDatabase（编辑器）：mode=assetdatabase;dataset=QuarkAssetDataset（Resources 路径）</para>
    /// </summary>
    public sealed class QuarkAssetProvider : IResourceProvider
    {
        /// <summary>
        /// 注册名
        /// </summary>
        public const string PROVIDER_NAME = "QuarkAsset";

        ResourceInitParameters initParameters;

        /// <summary>
        /// 程序集加载时自动注册。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            ResourceProviderRegistry.Register(PROVIDER_NAME, () => new QuarkAssetProvider());
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
                    | ResourceCapabilities.Scene;
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
            var operation = new QuarkAssetInitializationOperation(this);
            OperationSystem.StartOperation(operation);
            return operation;
        }
        /// <inheritdoc/>
        public ResourceProviderOperation CreateOperation(ResourceRequest request)
        {
            switch (request.LoadMode)
            {
                case AssetLoadMode.Scene:
                    return new QuarkSceneOperation(this, request);
                case AssetLoadMode.AllAssets:
                    return new QuarkAllAssetsOperation(this, request);
                case AssetLoadMode.SubAssets:
                    return CreateGenericOperation(typeof(QuarkSubAssetsOperation<>), request);
                case AssetLoadMode.Instance:
                    return null;
                default:
                    return CreateGenericOperation(typeof(QuarkAssetOperation<>), request);
            }
        }
        /// <inheritdoc/>
        public void UnloadUnusedAssets()
        {
            QuarkResources.UnloadAllAssetBundle(false);
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
            if (assetType == null || !typeof(Object).IsAssignableFrom(assetType))
                assetType = typeof(Object);
            var closedType = openType.MakeGenericType(assetType);
            return (ResourceProviderOperation)Activator.CreateInstance(closedType, new object[] { this, request });
        }

        /// <summary>
        /// 初始化操作：按 mode 启动 Quark 的 AssetBundle 或 AssetDatabase 模式。
        /// </summary>
        internal sealed class QuarkAssetInitializationOperation : InitializationOperation
        {
            readonly QuarkAssetProvider provider;
            bool done;
            string error;

            public QuarkAssetInitializationOperation(QuarkAssetProvider provider) : base(PROVIDER_NAME)
            {
                this.provider = provider;
            }

            protected override void OnStart()
            {
                base.OnStart();
                var parameters = provider.InitParameters;
                var mode = (parameters != null ? parameters.GetString("mode", "assetbundle") : "assetbundle").ToLowerInvariant();
                try
                {
                    if (mode == "assetdatabase" || mode == "editor")
                        LaunchAssetDatabaseMode(parameters);
                    else
                        LaunchAssetBundleMode(parameters);
                }
                catch (Exception e)
                {
                    Fail($"QuarkAsset initialize exception : {e.Message}");
                }
            }

            void LaunchAssetDatabaseMode(ResourceInitParameters parameters)
            {
                var datasetPath = parameters.GetString("dataset", "QuarkAssetDataset");
                var dataset = Resources.Load<QuarkDataset>(datasetPath);
                if (dataset == null)
                {
                    Fail($"QuarkDataset not found in Resources : {datasetPath}");
                    return;
                }
                QuarkResources.LaunchAssetDatabaseMode(dataset, OnLaunchSuccess, OnLaunchFailure);
            }

            void LaunchAssetBundleMode(ResourceInitParameters parameters)
            {
                var path = parameters.GetString("path", Application.streamingAssetsPath);
                var aesKey = parameters.GetString("aesKey", string.Empty);
                var offset = (ulong)parameters.GetLong("offset", 0);
                QuarkResources.LaunchAssetBundleMode(path, OnLaunchSuccess, OnLaunchFailure, aesKey, offset);
            }

            void OnLaunchSuccess()
            {
                done = true;
            }
            void OnLaunchFailure(string message)
            {
                error = message;
                done = true;
            }

            protected override void OnUpdate()
            {
                if (!done)
                    return;
                if (!string.IsNullOrEmpty(error))
                {
                    Fail($"QuarkAsset initialize failed : {error}");
                    return;
                }
                provider.SetInitialized(true);
                Complete(GetVersion());
            }

            static string GetVersion()
            {
                try
                {
                    return QuarkResources.GetBuildVersion();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
