using System;
namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 资源包信息
    /// </summary>
    public struct ResourceBundleInfo : IEquatable<ResourceBundleInfo>
    {
        /// <summary>
        /// 资源包的名称，AsseBundleName；
        /// </summary>
        public string BundleName;

        public bool Equals(ResourceBundleInfo other)
        {
            return other.BundleName == this.BundleName;
        }
        public override int GetHashCode()
        {
            return $"{BundleName}".GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return (obj is ResourceBundleInfo) && Equals((ResourceBundleInfo)obj);
        }
    }
}
