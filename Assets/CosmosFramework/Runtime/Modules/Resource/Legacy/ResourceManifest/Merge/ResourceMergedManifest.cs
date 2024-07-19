using System;
using System.Collections.Generic;
namespace Cosmos.Resource
{
    [Serializable]
    public class ResourceMergedManifest
    {
        /// <summary>
        /// Build version of assetbundle
        /// </summary>
        public string BuildVersion { get; set; }
        /// <summary>
        /// Unique hash of build
        /// </summary>
        public string BuildHash { get; set; }
        /// <summary>
        /// Merged bundles
        /// </summary>
        public List<ResourceMergedBundleBuildInfo> MergedBundleBuildInfoList;
    }
}
