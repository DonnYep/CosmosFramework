using System;
namespace Cosmos.Resource
{
    public struct ResourceVersion : IEquatable<ResourceVersion>
    {
        /// <summary>
        /// Build version of assetbundle
        /// </summary>
        public string ResourceBuildVersion;
        /// <summary>
        /// Desc for build version.
        /// </summary>
        public string ResourceDescription;

        public ResourceVersion(string resourceBuildVersion, string resourceDescription)
        {
            ResourceBuildVersion = resourceBuildVersion;
            ResourceDescription = resourceDescription;
        }

        public bool Equals(ResourceVersion other)
        {
            return other.ResourceBuildVersion == this.ResourceBuildVersion&&
                other.ResourceDescription==this.ResourceDescription;
        }
        public override string ToString()
        {
            return $"ResourceBuildVersion: {ResourceBuildVersion}\nResourceDescription: {ResourceDescription}";
        }
    }
}
