using System.Runtime.InteropServices;
using System;
namespace Cosmos.Resource.Verifiy
{
    /// <summary>
    /// Bundle用于校验时的数据；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct ResourceVerifiyInfo : IEquatable<ResourceVerifiyInfo>
    {
        /// <summary>
        /// 文件的地址；
        /// </summary>
        public string Url;
        /// <summary>
        /// 包的名称；
        /// </summary>
        public string BundleName;
        /// <summary>
        /// 是否被校验；
        /// </summary>
        public bool Verified;
        /// <summary>
        /// 是否与清单上的信息相等；
        /// </summary>
        public bool IsEqual;
        public ResourceVerifiyInfo(string url, string bundleName, bool verified, bool isEqual)
        {
            Url = url;
            BundleName = bundleName;
            Verified = verified;
            IsEqual = isEqual;
        }
        public bool Equals(ResourceVerifiyInfo other)
        {
            return Url == other.Url &&
                BundleName == other.BundleName &&
                Verified == other.Verified &&
                IsEqual == other.IsEqual;
        }
    }
}
