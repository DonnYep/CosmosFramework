using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
namespace Cosmos.CosmosEditor
{
    internal enum AssetBundleHashType:byte
    {
        DefaultName=0,
        AppendHash=1,
        HashInstead = 2
    }
    [Serializable]
   internal  class AssetBundleBuildTabData 
    {
        public BuildTarget BuildTarget= BuildTarget.StandaloneWindows;
        public string OutputPath= "AssetBundles/StandaloneWindows";
        public bool UseDefaultPath=true;
        public bool ClearFolders;
        public bool CopyToStreamingAssets;
        public string StreamingAssetsPath = "Assets/StreamingAssets";
        public bool WithoutManifest;
        public AssetBundleHashType  NameHashType;
        public bool UseAESEncryption = false;
        public string AESEncryptionKey= "QuarkAssetBundle";
        public BuildAssetBundleOptions BuildAssetBundleOptions= BuildAssetBundleOptions.ChunkBasedCompression;
    }
}