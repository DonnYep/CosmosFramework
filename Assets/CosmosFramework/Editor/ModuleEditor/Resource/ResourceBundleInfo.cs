using System;
namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 资源包信息
    /// </summary>
    public struct ResourceBundleInfo : IEquatable<ResourceBundleInfo>
    {
        public ResourceBundleInfo(string bundleName, string bundlePath, long bundleSize, int objectCount)
        {
            BundleName = bundleName;
            BundleSize = bundleSize;
            BundlePath = bundlePath;
            ObjectCount = objectCount;
        }
        /// <summary>
        /// 资源包内对象的数量；
        /// </summary>
        public int ObjectCount { get; private set; }
        /// <summary>
        /// 资源包的名称，AsseBundleName；
        /// </summary>
        public string BundleName { get; private set; }
        /// <summary>
        /// 资源包在Assets目录下的地址
        /// </summary>
        public string BundlePath { get; private set; }
        /// <summary>
        /// 编辑器模式下包体的大小
        /// </summary>
        public long BundleSize { get; private set; }
        public bool Equals(ResourceBundleInfo other)
        {
            return other.BundleName == this.BundleName;
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
