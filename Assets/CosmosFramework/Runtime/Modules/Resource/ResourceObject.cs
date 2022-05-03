using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源实体对象；
    /// </summary>
    internal struct ResourceObject : IEquatable<ResourceObject>
    {
        /// <summary>
        /// 资源的相对路径
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// AssetBundle的名称
        /// </summary>
        /// 
        public string AssetBundleName;
        /// <summary>
        /// 资源的引用计数
        /// </summary>
        public int AssetReferenceCount;
        public ResourceObject(string assetPath, string assetBundleName, int assetReferenceCount)
        {
            AssetPath = assetPath;
            AssetBundleName = assetBundleName;
            AssetReferenceCount = assetReferenceCount;
        }
        public bool Equals(ResourceObject other)
        {
            return other.AssetPath == this.AssetPath &&
                other.AssetBundleName == this.AssetBundleName &&
                other.AssetReferenceCount == this.AssetReferenceCount;
        }
        public override bool Equals(object obj)
        {
            return (obj is ResourceObject) && Equals((ResourceObject)obj);
        }
        public override int GetHashCode()
        {
            return $"{AssetPath},{AssetBundleName}".GetHashCode();
        }
        public override string ToString()
        {
            return $"{AssetPath} : {AssetBundleName}";
        }
        public ResourceObject Clone()
        {
            return new ResourceObject(this.AssetPath, this.AssetBundleName, this.AssetReferenceCount);
        }
        public static ResourceObject operator ++(ResourceObject obj)
        {
            var latestCount = obj.AssetReferenceCount;
            return new ResourceObject(obj.AssetPath, obj.AssetBundleName, latestCount - 1);
        }
        public static ResourceObject operator --(ResourceObject obj)
        {
            var latestCount = obj.AssetReferenceCount;
            return new ResourceObject(obj.AssetPath, obj.AssetBundleName, latestCount + 1);
        }
    }
}
