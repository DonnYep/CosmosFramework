namespace Cosmos.Resource
{
    internal static class CFResourceConstants
    {
        /// <summary>
        /// 文件清单名称
        /// </summary>
        public const string PACKAGE_MANIFEST_FILE_NAME = "PackageManifest.json";
        /// <summary>
        /// 默认资源包裹名
        /// </summary>
        public const string DEFAULT_PACKAGE_NAME = "DefaultPackage";
        /// <summary>
        /// 默认文件清单加密密钥
        /// </summary>
        public const string DEFAULT_MANIFEST_ENCRYPTION_KEY = "CosmosBundlesKey";
        /// <summary>
        /// AssetBundle默认后缀
        /// </summary>
        public const string DEFAULT_AB_EXTENSION = "ab";
        /// <summary>
        /// 默认构建相对路径
        /// </summary>
        public const string DEFAULT_BUILD_PATH = "AssetBundles/Resource";
        /// <summary>
        /// 默认StreamingAssets相对路径
        /// </summary>
        public const string DEFAULT_STREAMING_ASSETS_RELATIVE_PATH = "CosmosResources";
        /// <summary>
        /// 共享依赖包名前缀
        /// </summary>
        public const string SHARED_DEPENDENCY_BUNDLE_PREFIX = "~shared_";
    }
}
