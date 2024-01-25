using System.Collections.Generic;
using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// AssetBundle mode use only;
    /// Manifest of resource;
    /// </summary>
    [Serializable]
    public class ResourceManifest
    {
        Dictionary<string, ResourceBundleBuildInfo> resourceBundleBuildInfoDict;
        /// <summary>
        /// Build version of assetbundle
        /// </summary>
        public string BuildVersion { get; set; }
        /// <summary>
        /// Unique hash of build
        /// </summary>
        public string BuildHash { get; set; }
        /// <summary>
        /// AssetBundle offset
        /// </summary>
        public ulong BundleOffset { get; set; }
        /// <summary>
        /// BundleName===ResourceBundleBuildInfo
        /// </summary>
        public Dictionary<string, ResourceBundleBuildInfo> ResourceBundleBuildInfoDict
        {
            get
            {
                if (resourceBundleBuildInfoDict == null)
                    resourceBundleBuildInfoDict = new Dictionary<string, ResourceBundleBuildInfo>();
                return resourceBundleBuildInfoDict;
            }
            set { resourceBundleBuildInfoDict = value; }
        }
    }
}
