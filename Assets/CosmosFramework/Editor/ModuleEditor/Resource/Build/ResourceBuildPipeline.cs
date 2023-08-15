using Cosmos.Resource;
using UnityEditor;

namespace Cosmos.Editor.Resource
{
    public class ResourceBuildPipeline
    {
        static ResourceDataset dataset;
        static string ResourceDatasetPath = "Assets/ResourceDataset.asset";
        public static void BuildActivePlatformAssetBundleNameByDefault()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildAssetBundle(buildTarget, false);
        }
        public static void BuildActivePlatformAssetBundleNameByHash()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildAssetBundle(buildTarget);
        }
        public static void BuildStandaloneWindowsAssetBundleNameByDefault()
        {
            BuildAssetBundle(BuildTarget.StandaloneWindows, false);
        }
        public static void BuildStandaloneWindowsAssetBundleNameByHash()
        {
            BuildAssetBundle(BuildTarget.StandaloneWindows);
        }
        public static void BuildAndroidAssetBundleNameByDefault()
        {
            BuildAssetBundle(BuildTarget.Android, false);
        }
        public static void BuildAndroidAssetBundleNamedByHash()
        {
            BuildAssetBundle(BuildTarget.Android);
        }
        public static void BuildIOSAssetBundleNameByDefault()
        {
            BuildAssetBundle(BuildTarget.iOS, false);
        }
        public static void BuildIOSAssetBundleNameByHash()
        {
            BuildAssetBundle(BuildTarget.iOS);
        }
        public static void BuildAssetBundle(BuildTarget buildTarget, bool nameByHash = true)
        {
            dataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>(ResourceDatasetPath);
            var tabData = new AssetBundleTabData();
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            options |= BuildAssetBundleOptions.ChunkBasedCompression;
            if (nameByHash)
                tabData.AssetBundleNameType = AssetBundleNameType.HashInstead;
            else
                tabData.AssetBundleNameType = AssetBundleNameType.DefaultName;
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = tabData.AssetBundleBuildPath,
                AssetBundleEncryption = tabData.AssetBundleEncryption,
                AssetBundleOffsetValue = tabData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = options,
                AssetBundleNameType = tabData.AssetBundleNameType,
                EncryptManifest = tabData.EncryptManifest,
                ManifestEncryptionKey = tabData.ManifestEncryptionKey,
                BuildTarget = buildTarget,
                BuildVersion = $"{tabData.BuildVersion}_{tabData.InternalBuildVersion}",
                CopyToStreamingAssets = tabData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = tabData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = tabData.StreamingAssetsRelativePath,
                ClearStreamingAssetsDestinationPath = true
            };
            ResourceBuildController.BuildAssetBundle(dataset, buildParams);
        }
    }
}