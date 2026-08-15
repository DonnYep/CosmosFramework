using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源内部工具类：Provider工厂与失败句柄生成。
    /// </summary>
    internal static class ResourceHelper
    {
        /// <summary>
        /// 根据运行模式创建对应Provider
        /// </summary>
        public static ProviderBase CreateProvider(ResourcePackage package, AssetInfo assetInfo, Type assetType, AssetLoadMode loadMode)
        {
            var playMode = CFResources.ResourcePlayMode;
            switch (playMode)
            {
                case CFResourcePlayMode.AssetDatabase:
                    return new DatabaseAssetProvider(package, assetInfo, assetType, loadMode);
                case CFResourcePlayMode.AssetBundle:
                case CFResourcePlayMode.EditorAssetBundle:
                case CFResourcePlayMode.Offline:
                    return new AssetBundleAssetProvider(package, assetInfo, assetType, loadMode);
                default:
                    throw new ArgumentException($"Unsupported resource play mode : {playMode}");
            }
        }

        /// <summary>
        /// 创建立即失败的资产句柄
        /// </summary>
        public static AssetHandle<T> CreateFailedHandle<T>(ResourcePackage package, string assetKey, string error)
            where T : UnityEngine.Object
        {
            var assetInfo = CreateFakeAssetInfo(package.PackageName, assetKey);
            var provider = new FailedAssetProvider(assetInfo, typeof(T), error);
            package.InternalStartOperation(provider);
            return new AssetHandle<T>(provider);
        }

        /// <summary>
        /// 创建立即失败的子资产句柄
        /// </summary>
        public static SubAssetsHandle<T> CreateFailedSubAssetsHandle<T>(ResourcePackage package, string assetKey, string error)
            where T : UnityEngine.Object
        {
            var assetInfo = CreateFakeAssetInfo(package.PackageName, assetKey);
            var provider = new FailedAssetProvider(assetInfo, typeof(T), error);
            package.InternalStartOperation(provider);
            return new SubAssetsHandle<T>(provider);
        }

        /// <summary>
        /// 创建立即失败的全部资产句柄
        /// </summary>
        public static AllAssetsHandle<T> CreateFailedAllAssetsHandle<T>(ResourcePackage package, string bundleName, string error)
            where T : UnityEngine.Object
        {
            var assetInfo = CreateFakeAssetInfo(package.PackageName, bundleName);
            var provider = new FailedAssetProvider(assetInfo, typeof(T), error);
            package.InternalStartOperation(provider);
            return new AllAssetsHandle<T>(provider);
        }

        /// <summary>
        /// 创建立即失败的场景句柄
        /// </summary>
        public static SceneHandle CreateFailedSceneHandle(ResourcePackage package, string sceneKey, string error, bool additive, bool activateOnLoad)
        {
            var assetInfo = CreateFakeAssetInfo(package.PackageName, sceneKey);
            var provider = new FailedAssetProvider(assetInfo, typeof(UnityEngine.Object), error);
            package.InternalStartOperation(provider);
            return new SceneHandle(provider);
        }

        static AssetInfo CreateFakeAssetInfo(string packageName, string assetKey)
        {
            var packageAsset = new PackageAsset()
            {
                Name = assetKey,
                AssetPath = assetKey,
                AssetGuid = assetKey,
            };
            return new AssetInfo(packageName, packageAsset);
        }
    }
}

