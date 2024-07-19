using System;
namespace Cosmos.Resource.Verify
{
    /// <summary>
    /// Bundle用于校验时的数据；
    /// </summary>
    public struct ResourceManifestVerifyInfo : IEquatable<ResourceManifestVerifyInfo>
    {
        /// <summary>
        /// 文件的地址；
        /// </summary>
        public string Url;
        /// <summary>
        /// 包的名称；
        /// </summary>
        public string ResourceBundleName;
        /// <summary>
        /// 文件长度是否匹配；
        /// </summary>
        public bool ResourceBundleLengthMatched;
        /// <summary>
        /// 请求到的文件长度
        /// </summary>
        public long RequestedBundleLength;
        public ResourceManifestVerifyInfo(string url, string bundleName, bool matched, long requestedBundleLength)
        {
            Url = url;
            ResourceBundleName = bundleName;
            ResourceBundleLengthMatched = matched;
            RequestedBundleLength = requestedBundleLength;
        }
        public bool Equals(ResourceManifestVerifyInfo other)
        {
            return Url == other.Url &&
                ResourceBundleName == other.ResourceBundleName &&
                ResourceBundleLengthMatched == other.ResourceBundleLengthMatched;
        }
        public override string ToString()
        {
            return $"Url: {Url} ;ResourceBundleName: {ResourceBundleName};ResourceBundleLengthMatched: {ResourceBundleLengthMatched}; RequestedBundleLength: {RequestedBundleLength}";
        }
    }
}
