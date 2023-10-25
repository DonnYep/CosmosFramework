using System;
using System.Collections.Generic;
using System.IO;

namespace Cosmos.Resource
{
    public static partial class ResourceUtility
    {
        public static class Integrity
        {
            /// <summary>
            ///  verify resource bundle integrity 
            /// </summary>
            /// <param name="bundleBuildInfos">bundleBuildInfo in manifest</param>
            /// <param name="path">resource path</param>
            /// <param name="result">verify result</param>
            public static void VerifyResourceIntegrity(IEnumerable<ResourceBundleBuildInfo> bundleBuildInfos, string path, out ResourceIntegrityResult result)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("path is invalid !");
                result = new ResourceIntegrityResult();
                result.ResourceIntegrityInfos = new List<ResourceIntegrityInfo>();
                foreach (var bundleBuildInfo in bundleBuildInfos)
                {

                    var bundleKey = bundleBuildInfo.ResourceBundle.BundleKey;
                    var bundleName = bundleBuildInfo.ResourceBundle.BundleName;
                    var recordedBudleSize = bundleBuildInfo.BundleSize;
                    var bundlePath = Path.Combine(path, bundleKey);
                    long dectededBundleSize = 0;
                    if (File.Exists(bundlePath))
                    {
                        var fileInfo = new FileInfo(bundlePath);
                        dectededBundleSize = fileInfo.Length;
                    }
                    var integrityInfo = new ResourceIntegrityInfo(recordedBudleSize, dectededBundleSize, bundleKey, bundleName);
                    result.ResourceIntegrityInfos.Add(integrityInfo);
                }
            }
            /// <summary>
            /// verify resource bundle integrity 
            /// </summary>
            /// <param name="manifest">target manifest</param>
            /// <param name="path">resource path</param>
            /// <param name="result">verify result</param>
            public static void VerifyResourceIntegrity(ResourceManifest manifest, string path, out ResourceIntegrityResult result)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("path is invalid !");
                result = new ResourceIntegrityResult();
                result.ResourceIntegrityInfos = new List<ResourceIntegrityInfo>();
                foreach (var bundleBuildInfo in manifest.ResourceBundleBuildInfoDict.Values)
                {
                    var bundleKey = bundleBuildInfo.ResourceBundle.BundleKey;
                    var bundleName = bundleBuildInfo.ResourceBundle.BundleName;
                    var recordedBudleSize = bundleBuildInfo.BundleSize;
                    var bundlePath = Path.Combine(path, bundleKey);
                    long dectededBundleSize = 0;
                    if (File.Exists(bundlePath))
                    {
                        var fileInfo = new FileInfo(bundlePath);
                        dectededBundleSize = fileInfo.Length;
                    }
                    var integrityInfo = new ResourceIntegrityInfo(recordedBudleSize, dectededBundleSize, bundleKey, bundleName);
                    result.ResourceIntegrityInfos.Add(integrityInfo);
                }
            }
            public static void VerifyResourceIntegrity(ResourceMergedManifest mergedManifest, string path, out ResourceIntegrityResult result)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("path is invalid !");
                result = new ResourceIntegrityResult();
                result.ResourceIntegrityInfos = new List<ResourceIntegrityInfo>();
                foreach (var bundleBuildInfo in mergedManifest.MergedBundleBuildInfoList)
                {
                    var bundleKey = bundleBuildInfo.ResourceBundleBuildInfo.ResourceBundle.BundleKey;
                    var bundleName = bundleBuildInfo.ResourceBundleBuildInfo.ResourceBundle.BundleName;
                    var recordedBudleSize = bundleBuildInfo.ResourceBundleBuildInfo.BundleSize;
                    var bundlePath = Path.Combine(path, bundleKey);
                    long dectededBundleSize = 0;
                    if (File.Exists(bundlePath))
                    {
                        var fileInfo = new FileInfo(bundlePath);
                        dectededBundleSize = fileInfo.Length;
                    }
                    var integrityInfo = new ResourceIntegrityInfo(recordedBudleSize, dectededBundleSize, bundleKey, bundleName);
                    result.ResourceIntegrityInfos.Add(integrityInfo);
                }
            }
        }
    }
}