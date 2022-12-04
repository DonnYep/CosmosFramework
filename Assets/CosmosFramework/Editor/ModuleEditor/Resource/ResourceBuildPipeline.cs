using Cosmos.Resource;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
namespace Cosmos.Editor.Resource
{
    public class ResourceBuildPipeline
    {
        static AssetBundleBuilder assetBundleBuilder = new AssetBundleBuilder();
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
            ResourceWindowDataProxy.ResourceDataset = dataset;
            BuildDataset();
            var tabData = new AssetBundleTabData();
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            options |= BuildAssetBundleOptions.ChunkBasedCompression;
            if (nameByHash)
                tabData.AssetBundleNameType = AssetBundleNameType.HashInstead;
            else
                tabData.AssetBundleNameType = AssetBundleNameType.DefaultName;
            var buildParams = new AssetBundleBuildParams()
            {
                AssetBundleBuildPath = tabData.AssetBundleBuildPath,
                AssetBundleEncryption = tabData.AssetBundleEncryption,
                AssetBundleOffsetValue = tabData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = options,
                AssetBundleNameType = tabData.AssetBundleNameType,
                BuildedAssetsEncryption = tabData.BuildedAssetsEncryption,
                BuildIedAssetsEncryptionKey = tabData.BuildIedAssetsEncryptionKey,
                BuildTarget = buildTarget,
                BuildVersion = tabData.BuildVersion,
                CopyToStreamingAssets = tabData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = tabData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = tabData.StreamingAssetsRelativePath
            };
            ResourceManifest resourceManifest = new ResourceManifest();
            assetBundleBuilder.PrepareBuildAssetBundle(buildParams, dataset, ref resourceManifest);
            var unityManifest = BuildPipeline.BuildAssetBundles(buildParams.AssetBundleBuildPath, buildParams.BuildAssetBundleOptions, buildParams.BuildTarget);
            assetBundleBuilder.ProcessAssetBundle(buildParams, dataset, unityManifest, ref resourceManifest);
        }
        static void BuildDataset()
        {
            if (ResourceWindowDataProxy.ResourceDataset == null)
                return;
            var bundles = ResourceWindowDataProxy.ResourceDataset.ResourceBundleList;
            var objects = ResourceWindowDataProxy.ResourceDataset.ResourceObjectList;
            var extensions = ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
            var scenes = ResourceWindowDataProxy.ResourceDataset.ResourceSceneList;
            var lowerExtensions = extensions.Select(s => s.ToLower()).ToArray();
            extensions.Clear();
            extensions.AddRange(lowerExtensions);
            objects.Clear();
            scenes.Clear();
            var bundleLength = bundles.Count;

            List<ResourceBundleInfo> validBundleInfo = new List<ResourceBundleInfo>();
            List<ResourceBundle> invalidBundles = new List<ResourceBundle>();

            for (int i = 0; i < bundleLength; i++)
            {
                var bundle = bundles[i];
                var bundlePath = bundle.BundlePath;
                if (!AssetDatabase.IsValidFolder(bundlePath))
                {
                    invalidBundles.Add(bundle);
                    continue;
                }
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                importer.assetBundleName = bundle.BundleName;

                var files = Utility.IO.GetAllFiles(bundlePath);
                var fileLength = files.Length;
                bundle.ResourceObjectList.Clear();
                for (int j = 0; j < fileLength; j++)
                {
                    var srcFilePath = files[j].Replace("\\", "/");
                    var srcFileExt = Path.GetExtension(srcFilePath);
                    var lowerFileExt = srcFileExt.ToLower();
                    if (extensions.Contains(lowerFileExt))
                    {
                        //统一使用小写的文件后缀名
                        var lowerExtFilePath = srcFilePath.Replace(srcFileExt, lowerFileExt);
                        var resourceObject = new ResourceObject(Path.GetFileNameWithoutExtension(lowerExtFilePath), lowerExtFilePath, bundle.BundleName, lowerFileExt);
                        objects.Add(resourceObject);
                        bundle.ResourceObjectList.Add(resourceObject);
                        if (lowerFileExt == ResourceConstants.UNITY_SCENE_EXTENSION)//表示为场景资源
                        {
                            scenes.Add(resourceObject);
                        }
                    }
                }
                long bundleSize = EditorUtil.GetUnityDirectorySize(bundlePath, ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                var bundleInfo = new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, bundleSize, bundle.ResourceObjectList.Count);
                validBundleInfo.Add(bundleInfo);
            }
            for (int i = 0; i < invalidBundles.Count; i++)
            {
                bundles.Remove(invalidBundles[i]);
            }
            for (int i = 0; i < bundles.Count; i++)
            {
                var bundle = bundles[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                bundle.DependentList.Clear();
                bundle.DependentList.AddRange(AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true));
            }
            for (int i = 0; i < bundles.Count; i++)
            {
                var bundle = bundles[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                importer.assetBundleName = string.Empty;
            }
            EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
#if UNITY_2021_1_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(ResourceWindowDataProxy.ResourceDataset);
#elif UNITY_2019_1_OR_NEWER
            AssetDatabase.SaveAssets();
#endif
            ResourceWindowDataProxy.ResourceDataset.IsChanged = false;
        }
    }
}