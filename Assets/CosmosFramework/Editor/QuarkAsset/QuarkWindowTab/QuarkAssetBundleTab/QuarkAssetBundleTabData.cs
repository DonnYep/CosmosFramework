using System;
using UnityEditor;
namespace Quark.Editor
{
    internal enum AssetBundleHashType : byte
    {
        DefaultName = 0,
        AppendHash = 1,
        HashInstead = 2
    }
    [Serializable]
    internal class QuarkAssetBundleTabData
    {
        public BuildTarget BuildTarget;
        public string OutputPath;
        public bool UseDefaultPath;
        public bool ClearOutputFolders;
        public bool CopyToStreamingAssets;
        public string StreamingRelativePath;
        public bool WithoutManifest;
        public AssetBundleHashType NameHashType;
        /// <summary>
        /// 使用偏移加密；
        /// </summary>
        public bool UseOffsetEncryptionForAssetBundle;
        /// <summary>
        /// 加密偏移量；
        /// </summary>
        public int EncryptionOffsetForAssetBundle;

        /// <summary>
        /// 使用对称加密对build信息进行加密;
        /// </summary>
        public bool UseAesEncryptionForBuildInfo;
        /// <summary>
        /// 对称加密的密钥；
        /// </summary>
        public string AesEncryptionKeyForBuildInfo;

        public BuildAssetBundleOptions BuildAssetBundleOptions;

        public QuarkAssetBundleTabData()
        {
            BuildTarget = BuildTarget.StandaloneWindows;
            OutputPath = "AssetBundles/StandaloneWindows";
            UseDefaultPath = true;
            ClearOutputFolders = true;
            CopyToStreamingAssets = false;
            StreamingRelativePath = string.Empty;
            WithoutManifest = true;
            NameHashType = AssetBundleHashType.DefaultName;
            UseOffsetEncryptionForAssetBundle = false;
            EncryptionOffsetForAssetBundle = 0;
            UseAesEncryptionForBuildInfo = false;
            AesEncryptionKeyForBuildInfo = "QuarkAssetAesKey";
            BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        }
    }
}