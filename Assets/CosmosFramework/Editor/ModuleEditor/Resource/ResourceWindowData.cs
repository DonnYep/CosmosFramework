using System;
using UnityEditor;
namespace Cosmos.Editor
{
    [Serializable]
    public class ResourceWindowData
    {
        public BuildAssetBundleOptions BuildAssetBundleOptions;
        public BuildTarget BuildTarget;
        public string OutputPath;
        public ResourceWindowData()
        {
            BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
            BuildTarget = BuildTarget.StandaloneWindows;
            OutputPath = "AssetBundles/StandaloneWindows";
        }
    }
}
