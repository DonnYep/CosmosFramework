using System;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// bundle build cache info 
    /// </summary>
    public class ResourceBundleCacheInfo : IEquatable<ResourceBundleCacheInfo>
    {
        public string BundleName;
        public string BundlePath;
        public string BundleHash;
        public string[] AssetNames;

        public bool Equals(ResourceBundleCacheInfo other)
        {
            return BundleName == other.BundleName &&
         BundlePath == other.BundlePath &&
         BundleHash == other.BundleHash;
        }
    }
}
