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
        /// <summary>
        /// Build info of resource;
        /// </summary>
        [Serializable]
        public class ResourceBundleBuildInfo
        {
            /// <summary>
            /// Size of assetbundle;
            /// </summary>
            public long BundleSize;
            /// <summary>
            /// Hash of assetbundle
            /// </summary>
            public string BundleHash;
            /// <summary>
            /// Resource bundle pack;
            /// </summary>
            public ResourceBundle ResourceBundle;
        }
        Dictionary<string, ResourceBundleBuildInfo> resourceBundleBuildInfoDict;
        /// <summary>
        /// Build version of assetbundle
        /// </summary>
        public string BuildVersion { get; set; }
        /// <summary>
        /// BundleName===ResourceBundleBuildInfo；
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
