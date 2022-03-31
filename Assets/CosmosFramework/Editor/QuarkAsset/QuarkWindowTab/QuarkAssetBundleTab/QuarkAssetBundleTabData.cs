using System;
using UnityEditor;
namespace Cosmos.Editor.Quark
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
        public bool UseOffsetEncryption;
        /// <summary>
        /// 加密偏移量；
        /// </summary>
        public int EncryptionOffset;
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
            EncryptionOffset = 0;
            BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        }
    }
}