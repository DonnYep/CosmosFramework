using System;
using UnityEditor;
namespace Cosmos.Editor
{
    [Serializable]
    public class AssetBundleBuilderData
    {
        public BuildAssetBundleOptions BuildAssetBundleOptions;
        public BuildTarget BuildTarget;
        public string OutputPath;
        public AssetBundleBuilderData()
        {
            BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
            BuildTarget = BuildTarget.StandaloneWindows;
            OutputPath = "AssetBundles/StandaloneWindows";
        }
    }
}
