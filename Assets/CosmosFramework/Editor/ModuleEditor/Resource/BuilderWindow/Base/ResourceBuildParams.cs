using UnityEditor;
namespace Cosmos.Editor.Resource
{
    public struct ResourceBuildParams
    {
        /// <summary>
        /// AB打包到的平台
        /// </summary>
        public BuildTarget BuildTarget;
        /// <summary>
        /// AB打包选项；
        /// </summary>
        public BuildAssetBundleOptions BuildAssetBundleOptions;
        /// <summary>
        /// 强制移除工程中所有的ab标签
        /// </summary>
        public bool ForceRemoveAllAssetBundleNames;
        /// <summary>
        /// AB包名称类型
        /// </summary>
        public AssetBundleNameType AssetBundleNameType;
        /// <summary>
        /// 使用项目相对构建路径
        /// </summary>
        public bool UseProjectRelativeBuildPath;
        /// <summary>
        /// 工程项目的相对构建路径
        /// <see cref="ResourceEditorConstants.DEFAULT_PROJECT_RELATIVE_BUILD_PATH"/>
        /// </summary>
        public string ProjectRelativeBuildPath;
        /// <summary>
        /// AB打包输出的绝对路径；
        /// </summary>
        public string AssetBundleAbsoluteBuildPath;
        /// <summary>
        /// 构建详情输出地址
        /// </summary>
        public string BuildDetailOutputPath;
        /// <summary>
        /// 构建类型，增量或全量
        /// </summary>
        public ResourceBuildType ResourceBuildType;
        /// <summary>
        /// 打包的版本；
        /// </summary>
        public string BuildVersion;
        /// <summary>
        /// 内部版本号
        /// </summary>
        public int InternalBuildVersion;
        /// <summary>
        /// 加密manifest
        /// </summary>
        public bool EncryptManifest;
        /// <summary>
        /// Manifest加密密钥；
        /// </summary>
        public string ManifestEncryptionKey;
        /// <summary>
        /// AB偏移加密；
        /// </summary>
        public bool AssetBundleEncryption;
        /// <summary>
        /// AB偏移量；
        /// </summary>
        public int AssetBundleOffsetValue;
        /// <summary>
        /// 拷贝到streamingAsset文件；
        /// </summary>
        public bool CopyToStreamingAssets;
        /// <summary>
        /// 使用StreamingAsset相对路径；
        /// </summary>
        public bool UseStreamingAssetsRelativePath;
        /// <summary>
        /// 拷贝到的StreamingAssets相对路径；
        /// </summary>
        public string StreamingAssetsRelativePath;
        /// <summary>
        /// 清空拷贝到的StreamingAssets路径
        /// </summary>
        public bool ClearStreamingAssetsDestinationPath;
        /// <summary>
        /// 构建handler的完全限定名
        /// </summary>
        public string BuildHandlerName;
    }
}
