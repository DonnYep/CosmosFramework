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
        /// <summary>
        /// 包相对Asset目录下的地址；
        /// </summary>
        public string BundlePath;

        public bool Equals(ResourceBundleInfo other)
        {
            return other.BundleName == this.BundleName &&
                other.BundlePath == this.BundlePath;
        }
        public override int GetHashCode()
        {
            return $"{BundleName}{BundlePath}".GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return (obj is ResourceBundleInfo) && Equals((ResourceBundleInfo)obj);
        }
    }
}
