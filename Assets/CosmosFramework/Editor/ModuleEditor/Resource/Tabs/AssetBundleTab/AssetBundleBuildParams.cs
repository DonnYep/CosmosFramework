using UnityEditor;
namespace Cosmos.Editor.Resource
{
    public struct AssetBundleBuildParams
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
        /// AB包名称类型
        /// </summary>
        public BuildedAssetNameType BuildedAssetNameType;
        /// <summary>
        /// AB打包输出的绝对路径；
        /// </summary>
        public string AssetBundleBuildPath;
        /// <summary>
        /// 打包的版本；
        /// </summary>
        public string BuildVersion;
        /// <summary>
        /// 打包输出资源加密密钥；
        /// </summary>
        public string BuildIedAssetsEncryptionKey;
        /// <summary>
        /// AB偏移量；
        /// </summary>
        public int AssetBundleOffsetValue;
    }
}
