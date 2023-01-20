using Cosmos.Resource;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
            var bundleInfos = ResourceWindowDataProxy.ResourceDataset.ResourceBundleInfoList;
            var extensions = ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
            var lowerExtensions = extensions.Select(s => s.ToLower()).ToArray();
            extensions.Clear();
            extensions.AddRange(lowerExtensions);
            var bundleInfoLength = bundleInfos.Count;

            List<ResourceBundleInfo> invalidBundleInfos = new List<ResourceBundleInfo>();

            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundleInfo = bundleInfos[i];
                var bundlePath = bundleInfo.BundlePath;
                if (!AssetDatabase.IsValidFolder(bundlePath))
                {
                    invalidBundleInfos.Add(bundleInfo);
                    continue;
                }
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                importer.assetBundleName = bundleInfo.BundleName;

                var files = Utility.IO.GetAllFiles(bundlePath);
                var fileLength = files.Length;
                bundleInfo.ResourceObjectInfoList.Clear();
                for (int j = 0; j < fileLength; j++)
                {
                    var srcFilePath = files[j].Replace("\\", "/");
                    var srcFileExt = Path.GetExtension(srcFilePath);
                    var lowerFileExt = srcFileExt.ToLower();
                    if (extensions.Contains(lowerFileExt))
                    {
                        //统一使用小写的文件后缀名
                        var lowerExtFilePath = srcFilePath.Replace(srcFileExt, lowerFileExt);

                        var resourceObjectInfo = new ResourceObjectInfo()
                        {
                            BundleName = bundleInfo.BundleName,
                            Extension = lowerFileExt,
                            ObjectName = Path.GetFileNameWithoutExtension(lowerExtFilePath),
                            ObjectPath = lowerExtFilePath,
                            ObjectSize = EditorUtil.GetAssetFileSizeLength(lowerExtFilePath),
                            ObjectFormatBytes = EditorUtil.GetAssetFileSize(lowerExtFilePath),
                        };
                        var asset = AssetDatabase.LoadMainAssetAtPath(resourceObjectInfo.ObjectPath);
                        if (asset != null)
                        {
                            resourceObjectInfo.ObjectIcon = AssetDatabase.GetCachedIcon(lowerExtFilePath) as Texture2D;
                            resourceObjectInfo.ObjectState = ResourceEditorConstant.ObjectValidState;
                        }
                        else
                        {
                            resourceObjectInfo.ObjectIcon = EditorGUIUtility.FindTexture("console.erroricon");
                            resourceObjectInfo.ObjectState = ResourceEditorConstant.ObjectInvalidState;
                        }
                        bundleInfo.ResourceObjectInfoList.Add(resourceObjectInfo);
                    }
                }
                long bundleSize = EditorUtil.GetUnityDirectorySize(bundlePath, ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                bundleInfo.BundleSize = bundleSize;
                bundleInfo.BundleKey = bundleInfo.BundleName;
                bundleInfo.BundleFormatBytes = EditorUtility.FormatBytes(bundleSize);
            }
            for (int i = 0; i < invalidBundleInfos.Count; i++)
            {
                bundleInfos.Remove(invalidBundleInfos[i]);
            }
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                var bundle = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                bundle.DependentBundleKeyList.Clear();
                bundle.DependentBundleKeyList.AddRange(AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true));
            }
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                var bundle = bundleInfos[i];
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