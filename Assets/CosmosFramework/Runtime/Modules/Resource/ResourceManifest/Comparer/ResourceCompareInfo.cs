using System;

namespace Cosmos.Resource.Comparer
{
    public struct ResourceCompareInfo : IEquatable<ResourceCompareInfo>
    {
        public string ResouceBundleName { get; private set; }
        public bool Equals(ResourceCompareInfo other)
        {
            return other.ResouceBundleName == this.ResouceBundleName;
        }
    }
}
