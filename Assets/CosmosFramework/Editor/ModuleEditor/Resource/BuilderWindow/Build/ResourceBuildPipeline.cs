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
            var settingsData = new AssetBundleBuildPresetData();
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            options |= BuildAssetBundleOptions.ChunkBasedCompression;
            if (nameByHash)
                settingsData.AssetBundleNameType = AssetBundleNameType.HashInstead;
            else
                settingsData.AssetBundleNameType = AssetBundleNameType.DefaultName;
            var buildParams = new ResourceBuildParams()
            {
                AssetBundleBuildPath = settingsData.AssetBundleBuildPath,
                AssetBundleEncryption = settingsData.AssetBundleEncryption,
                AssetBundleOffsetValue = settingsData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = options,
                AssetBundleNameType = settingsData.AssetBundleNameType,
                EncryptManifest = settingsData.EncryptManifest,
                ManifestEncryptionKey = settingsData.ManifestEncryptionKey,
                BuildTarget = buildTarget,
                BuildVersion = $"{settingsData.BuildVersion}_{settingsData.InternalBuildVersion}",
                CopyToStreamingAssets = settingsData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = settingsData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = settingsData.StreamingAssetsRelativePath,
                ClearStreamingAssetsDestinationPath = true
            };
            ResourceBuildController.BuildAssetBundle(dataset, buildParams);
        }
    }
}