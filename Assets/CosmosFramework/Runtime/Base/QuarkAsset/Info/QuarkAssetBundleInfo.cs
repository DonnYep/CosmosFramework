using System;
namespace Quark
{
    public struct QuarkAssetBundleInfo : IEquatable<QuarkAssetBundleInfo>
    {
        /// <summary>
        /// AB包的名称；
        /// </summary>
        public string AssetBundleName { get; private set; }
        /// <summary>
        /// 包体对应的引用计数；
        /// </summary>
        public int ReferenceCount { get; private set; }
        public bool Equals(QuarkAssetBundleInfo other)
        {
            return other.AssetBundleName==this.AssetBundleName&&
                other.ReferenceCount==this.ReferenceCount;
        }
        public override bool Equals(object obj)
        {
            return (obj is QuarkAssetBundleInfo) && Equals((QuarkAssetBundleInfo)obj);
        }
        public override string ToString()
        {
            return $"AssetBundleName:{AssetBundleName},ReferenceCount:{ReferenceCount}";
        }
        public static bool operator ==(QuarkAssetBundleInfo a, QuarkAssetBundleInfo b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(QuarkAssetBundleInfo a, QuarkAssetBundleInfo b)
        {
            return !a.Equals(b);
        }
        internal static QuarkAssetBundleInfo Create(string assetBundleName,int referenceCount)
        {
            QuarkAssetBundleInfo info = new QuarkAssetBundleInfo();
            info.AssetBundleName = assetBundleName;
            info.ReferenceCount = referenceCount;
            return info;
        }
    }
}
