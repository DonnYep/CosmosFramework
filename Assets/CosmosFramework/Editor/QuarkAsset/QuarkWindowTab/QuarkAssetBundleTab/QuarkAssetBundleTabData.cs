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
        public BuildTarget BuildTarget { get; set; }
        public string OutputPath { get; set; }
        public bool UseDefaultPath { get; set; }
        public bool ClearOutputFolders { get; set; }
        public bool CopyToStreamingAssets { get; set; }
        public string StreamingRelativePath { get; set; }
        public bool WithoutManifest { get; set; }
        public AssetBundleHashType NameHashType { get; set; }
        /// <summary>
        /// 使用偏移加密；
        /// </summary>
        public bool UseOffsetEncryption { get; set; }
        /// <summary>
        /// 加密偏移量；
        /// </summary>
        public int EncryptionOffset { get; set; }
        public BuildAssetBundleOptions BuildAssetBundleOptions { get; set; }

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