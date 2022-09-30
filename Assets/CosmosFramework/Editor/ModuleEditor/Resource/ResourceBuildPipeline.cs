using Cosmos.Resource;
using System.Collections;
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
        public static void BuildActivePlatformAssetBundle()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildAssetBundle(buildTarget);
        }
        public static void BuildAndroidAssetBundle()
        {
            BuildAssetBundle(BuildTarget.Android);
        }
        public static void BuildiOSAssetBundle()
        {
            BuildAssetBundle( BuildTarget.iOS);
        }
        public static void BuildAssetBundle(BuildTarget buildTarget)
        {
            dataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>(ResourceDatasetPath);
            AssetBundleBuildParams buildParams = new AssetBundleBuildParams();
            ResourceManifest resourceManifest = new ResourceManifest();
            assetBundleBuilder.PrepareBuildAssetBundle(buildParams, dataset, ref resourceManifest);
            var unityManifest = BuildPipeline.BuildAssetBundles(buildParams.AssetBundleBuildPath, buildParams.BuildAssetBundleOptions, buildParams.BuildTarget);
            assetBundleBuilder.ProcessAssetBundle(buildParams, dataset, unityManifest, ref resourceManifest);
        }
        static IEnumerator EnumBuildDataset()
        {
            if (ResourceWindowDataProxy.ResourceDataset == null)
                yield break;
            var bundles = ResourceWindowDataProxy.ResourceDataset.ResourceBundleList;
            var objects = ResourceWindowDataProxy.ResourceDataset.ResourceObjectList;
            var extensions = ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList;
            var lowerExtensions = extensions.Select(s => s.ToLower()).ToArray();
            extensions.Clear();
            extensions.AddRange(lowerExtensions);
            objects.Clear();
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
                    if (extensions.Contains(srcFileExt))
                    {
                        //统一使用小写的文件后缀名
                        var lowerExtFilePath = srcFilePath.Replace(srcFileExt, lowerFileExt);
                        var resourceObject = new ResourceObject(Path.GetFileNameWithoutExtension(lowerExtFilePath), lowerExtFilePath, bundle.BundleName, lowerFileExt);
                        objects.Add(resourceObject);
                        bundle.ResourceObjectList.Add(resourceObject);
                    }
                }
                long bundleSize = EditorUtil.GetUnityDirectorySize(bundlePath, ResourceWindowDataProxy.ResourceDataset.ResourceAvailableExtenisonList);
                var bundleInfo = new ResourceBundleInfo(bundle.BundleName, bundle.BundlePath, EditorUtility.FormatBytes(bundleSize), bundle.ResourceObjectList.Count);
                validBundleInfo.Add(bundleInfo);

                var bundlePercent = i / (float)bundleLength;
                EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {Mathf.RoundToInt(bundlePercent * 100)}%", bundlePercent);
                yield return null;
            }
            EditorUtility.DisplayProgressBar("BuildDataset building", $"building bundle : {100}%", 1);
            //yield return null;
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
            yield return null;
            EditorUtility.ClearProgressBar();
            EditorUtility.SetDirty(ResourceWindowDataProxy.ResourceDataset);
            AssetDatabase.SaveAssets();

            ResourceWindowDataProxy.ResourceDataset.IsChanged = false;
            yield return null;
        }
    }
}