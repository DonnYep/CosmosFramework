using System;
using UnityEditor;

namespace Cosmos.Editor.Resource
{
    [Serializable]
    public class AssetBundleTabData
    {
        /// <summary>
        /// AB打包到的平台
        /// </summary>
        public BuildTarget BuildTarget;
        /// <summary>
        /// AB资源压缩类型；
        /// </summary>
        public AssetBundleCompressType AssetBundleCompressType;
        /// <summary>
        /// AB资源名称类型
        /// </summary>
        public BuildedAssetNameType BuildedAssetNameType;
        /// <summary>
        /// 不会在AssetBundle中包含类型信息;
        /// </summary>
        public bool DisableWriteTypeTree;
        /// <summary>
        /// 使用存储在Asset Bundle中的对象的id的哈希构建Asset Bundle;
        /// </summary>
        public bool DeterministicAssetBundle;
        /// <summary>
        /// 强制重建Asset Bundles;
        /// </summary>
        public bool ForceRebuildAssetBundle;
        /// <summary>
        /// 执行增量构建检查时忽略类型树更改;
        /// </summary>
        public bool IgnoreTypeTreeChanges;
        /// <summary>
        /// AB打包输出的绝对路径；
        /// </summary>
        public string AssetBundleBuildPath;
        /// <summary>
        /// AB输出目录；
        /// </summary>
        public string BuildPath;
        /// <summary>
        /// 打包的版本；
        /// </summary>
        public string BuildVersion;
        /// <summary>
        /// AB偏移加密；
        /// </summary>
        public bool AssetBundleEncryption;
        /// <summary>
        /// AB偏移量；
        /// </summary>
        public int AssetBundleOffsetValue;
        /// <summary>
        /// 打包输出资源加密；
        /// </summary>
        public bool BuildedAssetsEncryption;
        /// <summary>
        /// 打包输出资源加密密钥；
        /// </summary>
        public string BuildIedAssetsEncryptionKey;
        public AssetBundleTabData()
        {
            AssetBundleCompressType = AssetBundleCompressType.ChunkBasedCompression_LZ4;
            BuildTarget = BuildTarget.StandaloneWindows;
            BuildPath = Utility.IO.WebPathCombine(EditorUtil.ApplicationPath(), "AssetBundles");
            AssetBundleEncryption = false;
            AssetBundleOffsetValue = 16;
            BuildedAssetsEncryption = false;
            BuildIedAssetsEncryptionKey = "CosmosBundlesKey";
            BuildedAssetNameType = BuildedAssetNameType.HashInstead;
            BuildVersion = "Alpha";
            ForceRebuildAssetBundle = false;
            DisableWriteTypeTree = false;
            DeterministicAssetBundle = false;
            IgnoreTypeTreeChanges = false;
        }
    }
}
