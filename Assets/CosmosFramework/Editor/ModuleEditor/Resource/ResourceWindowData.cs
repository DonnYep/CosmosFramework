using System;
using UnityEditor;
namespace Cosmos.Editor.Resource
{
    [Serializable]
    public class ResourceWindowData
    {
        /// <summary>
        /// AB打包选项；
        /// </summary>
        public BuildAssetBundleOptions BuildAssetBundleOptions;
        /// <summary>
        /// AB打包到的平台
        /// </summary>
        public BuildTarget BuildTarget;
        /// <summary>
        /// AB输出目录；
        /// </summary>
        public string OutputPath;
        /// <summary>
        /// Assets目录下，ResourceDataset的文件地址；
        /// </summary>
        public string ResourceDatasetPath;
        /// <summary>
        /// 被选择的标签序号；
        /// </summary>
        public int SelectedTabIndex;
        public ResourceWindowData()
        {
            BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
            BuildTarget = BuildTarget.StandaloneWindows;
            OutputPath = "AssetBundles/StandaloneWindows";
            ResourceDatasetPath = "Assets/ResourceDataset";
        }
    }
}
