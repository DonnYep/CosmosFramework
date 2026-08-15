using Cosmos.Operation;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    /// <summary>
    /// AssetBundle资源提供器。
    /// <para>通过共享的BundleLoader加载包体，再异步加载包内资产。同一包体的多路并发加载共享同一个异步请求。</para>
    /// </summary>
    internal class AssetBundleAssetProvider : ProviderBase
    {
        readonly ResourcePackage package;
        BundleLoader bundleLoader;
        AssetBundleRequest assetRequest;

        internal AssetBundleAssetProvider(ResourcePackage package, AssetInfo assetInfo, Type assetType, AssetLoadMode loadMode)
        {
            this.package = package;
            this.MainAssetInfo = assetInfo;
            this.AssetType = assetType;
            this.LoadMode = loadMode;
        }

        protected override void OnStart()
        {
            var bundleName = MainAssetInfo.BundleName;
            if (string.IsNullOrEmpty(bundleName))
            {
                InvokeCompletion($"Bundle name is empty for asset : {MainAssetInfo.AssetPath}", OperationStatus.Failed);
                return;
            }
            bundleLoader = package.GetBundles().LoadBundle(bundleName);
            if (bundleLoader == null)
            {
                InvokeCompletion($"Bundle not found : {bundleName}", OperationStatus.Failed);
            }
        }

        protected override void OnUpdate()
        {
            if (IsDone || IsDestroyed)
                return;
            if (bundleLoader == null)
                return;
            // 等待包体加载完成
            if (!bundleLoader.IsDone)
            {
                SetWaitForAsyncComplete();
                Progress = bundleLoader.Progress * 0.5f;
                return;
            }
            if (bundleLoader.Status != OperationStatus.Succeeded || bundleLoader.AssetBundle == null)
            {
                InvokeCompletion($"Failed to load bundle : {bundleLoader.BundleName}", OperationStatus.Failed);
                return;
            }
            if (currentStep == ProviderStep.None)
            {
                var assetBundle = bundleLoader.AssetBundle;
                switch (LoadMode)
                {
                    case AssetLoadMode.SingleAsset:
                        assetRequest = assetBundle.LoadAssetAsync(MainAssetInfo.AssetPath, AssetType ?? typeof(Object));
                        break;
                    case AssetLoadMode.SubAssets:
                        assetRequest = assetBundle.LoadAssetWithSubAssetsAsync(MainAssetInfo.AssetPath, AssetType ?? typeof(Object));
                        break;
                    case AssetLoadMode.AllAssets:
                        assetRequest = assetBundle.LoadAllAssetsAsync(AssetType ?? typeof(Object));
                        break;
                }
                if (assetRequest == null)
                {
                    InvokeCompletion($"Failed to start asset request : {MainAssetInfo.AssetPath}", OperationStatus.Failed);
                    return;
                }
                currentStep = ProviderStep.Loading;
                SetWaitForAsyncComplete();
            }
            if (currentStep == ProviderStep.Loading)
            {
                if (!assetRequest.isDone)
                {
                    Progress = 0.5f + assetRequest.progress * 0.5f;
                    return;
                }
                Progress = 1;
                if (LoadMode == AssetLoadMode.SingleAsset)
                    AssetObject = assetRequest.asset;
                else
                    AllAssetObjects = assetRequest.allAssets;
                currentStep = ProviderStep.Checking;
            }
            if (currentStep == ProviderStep.Checking)
            {
                if (LoadMode == AssetLoadMode.SingleAsset)
                {
                    if (AssetObject == null)
                        InvokeCompletion($"Failed to load asset : {MainAssetInfo.AssetPath}", OperationStatus.Failed);
                    else
                        InvokeCompletion(string.Empty, OperationStatus.Succeeded);
                }
                else
                {
                    if (AllAssetObjects == null || AllAssetObjects.Length == 0)
                        InvokeCompletion($"Failed to load assets : {MainAssetInfo.AssetPath}", OperationStatus.Failed);
                    else
                        InvokeCompletion(string.Empty, OperationStatus.Succeeded);
                }
            }
        }

        protected override void OnDestroy()
        {
            if (bundleLoader != null)
            {
                package.GetBundles().UnloadBundle(bundleLoader);
                bundleLoader = null;
            }
        }

        internal override void WaitForCompletion()
        {
            int frame = 1000;
            while (!IsDone && frame-- > 0)
            {
                // 主线程阻塞状态下手动驱动操作链
                OperationSystem.Update();
                OnUpdate();
            }
        }
    }
}
