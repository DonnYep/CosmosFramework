using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
namespace Cosmos.CosmosEditor
{
    [Serializable]
   internal  class AssetBundleTabData 
    {
        public BuildTarget BuildTarget= BuildTarget.StandaloneWindows;
        public string OutputPath= "AssetBundles/StandaloneWindows";
        public bool UseDefaultPath=true;
        public bool ClearFolders;
        public bool CopyToStreamingAssets;
        public bool AppendHash;
        public string EncryptionKey="QuarkAsset";
        public BuildAssetBundleOptions BuildAssetBundleOptions= BuildAssetBundleOptions.ChunkBasedCompression;
    }
}