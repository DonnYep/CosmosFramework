using System.Runtime.InteropServices;
using System;
namespace Cosmos.Resource.Verifiy
{
    /// <summary>
    /// Bundle用于校验时的数据；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct ResourceManifestVerifiyInfo : IEquatable<ResourceManifestVerifiyInfo>
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
        public ResourceManifestVerifiyInfo(string url, string bundleName, bool matched)
        {
            Url = url;
            ResourceBundleName = bundleName;
            ResourceBundleLengthMatched = matched;
        }
        public bool Equals(ResourceManifestVerifiyInfo other)
        {
            return Url == other.Url &&
                ResourceBundleName == other.ResourceBundleName &&
                ResourceBundleLengthMatched == other.ResourceBundleLengthMatched;
        }
        public override string ToString()
        {
            return $"Url: {Url} ;ResourceBundleName: {ResourceBundleName};ResourceBundleLengthMatched: {ResourceBundleLengthMatched}";
        }
    }
}
