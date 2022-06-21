using System;
using System.Runtime.InteropServices;
using UnityEngine;
namespace Cosmos.Resource
{
    /// <summary>
    /// 资源对象
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    [Serializable]
    public struct ResourceObject : IEquatable<ResourceObject>
    {
        [SerializeField]
        string assetName;
        [SerializeField]
        string assetPath;
        [SerializeField]
        string bundleName;
        [SerializeField]
        string extension;
        /// <summary>
        /// 资源名
        /// </summary>
        public string AssetName { get { return assetName; } }
        /// <summary>
        /// 资源在asset目录下的地址
        /// </summary>
        public string AssetPath { get { return assetPath; } }
        /// <summary>
        /// 资源Bundle的名称
        /// </summary>
        public string BundleName { get { return bundleName; } }
        /// <summary>
        /// 后缀名；
        /// </summary>
        public string Extension { get { return extension; } }
        public ResourceObject(string assetName, string assetPath, string bundleName, string extension)
        {
            this.assetName = assetName;
            this.assetPath = assetPath;
            this.bundleName = bundleName;
            this.extension = extension;
        }
        public bool Equals(ResourceObject other)
        {
            return other.Extension == this.Extension &&
                other.BundleName == this.BundleName &&
                other.AssetName == this.AssetName &&
                other.AssetPath == this.AssetPath;
        }
        public override bool Equals(object obj)
        {
            return (obj is ResourceObject) && Equals((ResourceObject)obj);
        }
        public override int GetHashCode()
        {
            return $"{assetPath},{bundleName}".GetHashCode();
        }
        public override string ToString()
        {
            return $"AssetPath: {assetPath} , BundleName: {bundleName}";
        }
        public ResourceObject Clone()
        {
            return new ResourceObject(this.AssetName, this.assetPath, this.BundleName, this.Extension);
        }
    }
}
