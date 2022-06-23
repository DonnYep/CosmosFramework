using System;
namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 资源包信息
    /// </summary>
    public struct ResourceBundleInfo : IEquatable<ResourceBundleInfo>
    {
        public ResourceBundleInfo(string bundleName, string bundleSize)
        {
            BundleName = bundleName;
            BundleSize = bundleSize;
        }
        /// <summary>
        /// 资源包的名称，AsseBundleName；
        /// </summary>
        public string BundleName { get; private set; }
        /// <summary>
        /// 编辑器模式下包体的大小
        /// </summary>
        public string BundleSize { get; private set; }

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
