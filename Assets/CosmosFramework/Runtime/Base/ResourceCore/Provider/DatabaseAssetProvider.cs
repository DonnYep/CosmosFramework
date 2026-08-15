using Cosmos.Operation;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    /// <summary>
    /// 编辑器资源加载器，通过AssetDatabase同步加载资产。
    /// <para>仅用于编辑器模拟模式下免构建AB包直接加载。</para>
    /// </summary>
    internal class DatabaseAssetProvider : ProviderBase
    {
        readonly ResourcePackage package;

        internal DatabaseAssetProvider(ResourcePackage package, AssetInfo assetInfo, Type assetType, AssetLoadMode loadMode)
        {
            this.package = package;
            this.MainAssetInfo = assetInfo;
            this.AssetType = assetType;
            this.LoadMode = loadMode;
        }

        protected override void OnStart()
        {
        }

        protected override void OnUpdate()
        {
#if UNITY_EDITOR
            if (IsDone || IsDestroyed)
                return;
            if (currentStep == ProviderStep.None)
            {
                switch (LoadMode)
                {
                    case AssetLoadMode.SingleAsset:
                        AssetObject = LoadMainAsset();
                        break;
                    case AssetLoadMode.SubAssets:
                        AllAssetObjects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(MainAssetInfo.AssetPath);
                        break;
                    case AssetLoadMode.AllAssets:
                        AllAssetObjects = LoadBundleAllAssets();
                        break;
                }
                currentStep = ProviderStep.Checking;
            }
            if (currentStep == ProviderStep.Checking)
            {
                if (LoadMode == AssetLoadMode.SingleAsset)
                {
                    if (AssetObject == null)
                    {
                        InvokeCompletion($"Failed to load asset : {MainAssetInfo.AssetPath} AssetType : {AssetType}", OperationStatus.Failed);
                    }
                    else
                    {
                        InvokeCompletion(string.Empty, OperationStatus.Succeeded);
                    }
                }
                else
                {
                    if (AllAssetObjects == null || AllAssetObjects.Length == 0)
                    {
                        InvokeCompletion($"Failed to load assets : {MainAssetInfo.AssetPath}", OperationStatus.Failed);
                    }
                    else
                    {
                        InvokeCompletion(string.Empty, OperationStatus.Succeeded);
                    }
                }
            }
#endif
        }

        Object LoadMainAsset()
        {
            string guid = UnityEditor.AssetDatabase.AssetPathToGUID(MainAssetInfo.AssetPath);
            if (string.IsNullOrEmpty(guid))
            {
                InvokeCompletion($"Asset not found : {MainAssetInfo.AssetPath}", OperationStatus.Failed);
                return null;
            }
            return UnityEditor.AssetDatabase.LoadAssetAtPath(MainAssetInfo.AssetPath, AssetType ?? typeof(Object));
        }

        Object[] LoadBundleAllAssets()
        {
            var manifest = package.GetManifest();
            if (manifest == null)
                return null;
            var bundleName = MainAssetInfo.BundleName;
            var result = new List<Object>();
            var assetList = manifest.AssetList;
            var count = assetList.Count;
            for (int i = 0; i < count; i++)
            {
                var asset = assetList[i];
                if (asset.BundleName != bundleName)
                    continue;
                var obj = UnityEditor.AssetDatabase.LoadAssetAtPath(asset.AssetPath, AssetType ?? typeof(Object));
                if (obj != null)
                    result.Add(obj);
            }
            return result.ToArray();
        }

        internal override void WaitForCompletion()
        {
            // 编辑器同步加载，立即完成
            int frame = 100;
            while (!IsDone && frame-- > 0)
            {
                OperationSystem.Update();
                OnUpdate();
            }
        }
    }
}
