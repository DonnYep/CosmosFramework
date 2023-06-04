using System;
using System.Collections.Generic;
using System.IO;
using static Cosmos.Resource.ResourceManifest;

namespace Cosmos.Resource
{
    public class ResourceIntegrityVerifier
    {
        /// <summary>
        ///  verify resource bundle integrity 
        /// </summary>
        /// <param name="bundleBuildInfos">bundleBuildInfo in manifest</param>
        /// <param name="path">resource path</param>
        /// <param name="result">verify result</param>
        public static void VerifyResourceIntegrity(IList<ResourceBundleBuildInfo> bundleBuildInfos, string path, out ResourceIntegrityResult result)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path is invalid !");
            result = new ResourceIntegrityResult();
            result.ResourceIntegrityInfos = new ResourceIntegrityInfo[bundleBuildInfos.Count];
            int bundleIndex = 0;
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
                result.ResourceIntegrityInfos[bundleIndex] = integrityInfo;
                bundleIndex++;
            }
        }
    }
}
