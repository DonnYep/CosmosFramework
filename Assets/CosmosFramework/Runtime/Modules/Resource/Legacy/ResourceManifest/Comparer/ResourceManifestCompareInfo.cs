using System;
namespace Cosmos.Resource.Compare
{
    [Serializable]
    public struct ResourceManifestCompareInfo : IEquatable<ResourceManifestCompareInfo>
    {
        public string ResouceBundleName;
        /// <summary>
        /// 用于寻址的文件名；
        /// </summary>
        public string ResouceBundleKey;
        public long ResouceBundleSize;
        public string ResourceBundleHash;

        public ResourceManifestCompareInfo(string resouceBundleName, string resouceBundleKey, long resouceBundleSize, string resourceBundleHash)
        {
            ResouceBundleName = resouceBundleName;
            ResouceBundleSize = resouceBundleSize;
            ResouceBundleKey = resouceBundleKey;
            ResourceBundleHash = resourceBundleHash;
        }
        public bool Equals(ResourceManifestCompareInfo other)
        {
            return other.ResouceBundleName == this.ResouceBundleName &&
                other.ResouceBundleKey == this.ResouceBundleKey &&
                other.ResouceBundleSize == this.ResouceBundleSize &&
                other.ResourceBundleHash == this.ResourceBundleHash;
        }
        public override string ToString()
        {
            return $"ResouceBundleName: {ResouceBundleName}; ResouceBundleKey: {ResouceBundleKey}; ResouceBundleSize: {ResouceBundleSize}; ResourceBundleHash :{ResourceBundleHash}";
        }
    }
}
