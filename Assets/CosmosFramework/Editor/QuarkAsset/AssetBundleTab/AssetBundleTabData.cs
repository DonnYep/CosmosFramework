using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
namespace Cosmos.CosmosEditor
{
    internal enum AssetBundleHashType:byte
    {
        None=0,
        AppendHash=1,
        HashInstead = 2
    }
    [Serializable]
   internal  class AssetBundleTabData 
    {
        public BuildTarget BuildTarget= BuildTarget.StandaloneWindows;
        public string OutputPath= "AssetBundles/StandaloneWindows";
        public bool UseDefaultPath=true;
        public bool ClearFolders;
        public bool CopyToStreamingAssets;
        public AssetBundleHashType  NameHashType;
        public string EncryptionKey="QuarkAsset";
        public BuildAssetBundleOptions BuildAssetBundleOptions= BuildAssetBundleOptions.ChunkBasedCompression;
    }
}