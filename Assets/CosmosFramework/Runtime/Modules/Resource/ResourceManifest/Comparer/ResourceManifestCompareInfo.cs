using System;

namespace Cosmos.Resource.Comparer
{
    public struct ResourceManifestCompareInfo : IEquatable<ResourceManifestCompareInfo>
    {
        public string ResouceBundleName { get; private set; }
        /// <summary>
        /// 是否已经过期；
        /// </summary>
        public bool Expired{ get; private set; }
        public bool Equals(ResourceManifestCompareInfo other)
        {
            return other.ResouceBundleName == this.ResouceBundleName;
        }
    }
}
