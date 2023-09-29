using Cosmos.Resource;
using UnityEditor;

namespace Cosmos.Editor.Resource
{
    public class ResourceBuildPipeline
    {
        static ResourceDataset dataset;
        static AssetBundleBuildProfile buildProfile;
        public static ResourceDataset DefaultResourceDataset
        {
            get
            {
                if (dataset == null)
                    dataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>(ResourceBuilderWindowConstant.DEFAULT_DATASET_PATH);
                return dataset;
            }
        }
        public static AssetBundleBuildProfile DefaultAssetBundleBuildProfile
        {
            get
            {
                if (buildProfile == null)
                    buildProfile = AssetDatabase.LoadAssetAtPath<AssetBundleBuildProfile>(ResourceBuilderWindowConstant.DEFAULT_BUILD_PROFILE_PATH);
                return buildProfile;
            }
        }
        public static void BuildAssetBundle(BuildTarget buildTarget, bool nameByHash = true)
        {
            if (DefaultResourceDataset == null)
            {
                EditorUtil.Debug.LogError($"ResourceDataset : {ResourceBuilderWindowConstant.DEFAULT_DATASET_PATH} not exist !");
                return;
            }
            var presetData = new AssetBundleBuildProfileData();
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            options |= BuildAssetBundleOptions.ChunkBasedCompression;
            if (nameByHash)
                presetData.AssetBundleNameType = AssetBundleNameType.HashInstead;
            else
                presetData.AssetBundleNameType = AssetBundleNameType.DefaultName;
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = presetData.AssetBundleBuildPath,
                AssetBundleEncryption = presetData.AssetBundleEncryption,
                AssetBundleOffsetValue = presetData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = options,
                AssetBundleNameType = presetData.AssetBundleNameType,
                EncryptManifest = presetData.EncryptManifest,
                ManifestEncryptionKey = presetData.ManifestEncryptionKey,
                BuildTarget = buildTarget,
                BuildVersion = $"{presetData.BuildVersion}_{presetData.InternalBuildVersion}",
                CopyToStreamingAssets = presetData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = presetData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = presetData.StreamingAssetsRelativePath,
                ClearStreamingAssetsDestinationPath = true
            };
            ResourceBuildController.BuildAssetBundle(dataset, buildParams);
        }
        public static void BuildAssetBundleByProfile()
        {
            if (DefaultResourceDataset == null)
            {
                EditorUtil.Debug.LogError($"ResourceDataset : {ResourceBuilderWindowConstant.DEFAULT_DATASET_PATH} not exist !");
                return;
            }
            if (DefaultAssetBundleBuildProfile == null)
            {
                EditorUtil.Debug.LogError($"AssetBundleBuildProfile : {ResourceBuilderWindowConstant.DEFAULT_BUILD_PROFILE_PATH} not exist !");
                return;
            }
            var buildParams = DefaultAssetBundleBuildProfile.GetBuildParams();
            ResourceBuildController.BuildAssetBundle(dataset, buildParams);
        }
    }
}