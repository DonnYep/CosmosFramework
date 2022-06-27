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
        /// AB打包选项；
        /// </summary>
        public BuildAssetBundleOptions BuildAssetBundleOptions;
        /// <summary>
        /// AB包名称类型
        /// </summary>
        public AssetBundleNameType AssetBundleNameType;
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
        /// 打包信息加密；
        /// </summary>
        public bool BuildInfoEncryption;
        /// <summary>
        /// 打包信息加密密钥；
        /// </summary>
        public string BuildInfoEncryptionKey;
        public AssetBundleTabData()
        {
            BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
            BuildTarget = BuildTarget.StandaloneWindows;
            BuildPath = Utility.IO.WebPathCombine(EditorUtil.ApplicationPath(), "AssetBundles");
            AssetBundleEncryption = false;
            AssetBundleOffsetValue = 16;
            BuildInfoEncryption = false;
            BuildInfoEncryptionKey = "CosmosBundlesKey";
            AssetBundleNameType = AssetBundleNameType.DefaultName;
            BuildVersion = "0.0.1";
        }
    }
}
