using System;
namespace Quark.Asset
{
    /// <summary>
    /// QuarkAssetObject引用计数类
    /// </summary>
    internal class QuarkAssetObjectWapper : IEquatable<QuarkAssetObjectWapper>
    {
        public QuarkAssetObject QuarkAssetObject;
        /// <summary>
        /// 资源的引用计数；
        /// </summary>
        public int AssetReferenceCount;
        public bool Equals(QuarkAssetObjectWapper other)
        {
            return other.QuarkAssetObject==this.QuarkAssetObject&&
                other.AssetReferenceCount == this.AssetReferenceCount;
        }
        public override bool Equals(object obj)
        {
            return (obj is QuarkAssetObjectWapper) && Equals((QuarkAssetObjectWapper)obj);
        }
        public QuarkAssetObjectInfo GetQuarkAssetObjectInfo()
        {
            var assetBundleName = QuarkAssetObject.AssetBundleName;
            var assetPath = QuarkAssetObject.AssetPath;
            var referenceCount = AssetReferenceCount;
            var assetName = QuarkAssetObject.AssetName;
            var assetExtension = QuarkAssetObject.AssetExtension;
            var assetType = QuarkAssetObject.AssetType;
            var info = QuarkAssetObjectInfo.Create(assetName, assetPath, assetBundleName, assetExtension, assetType,referenceCount);
            return info;
        }
        public static QuarkAssetObjectWapper operator --(QuarkAssetObjectWapper quarkAsset)
        {
            quarkAsset.AssetReferenceCount--;
            return quarkAsset;
        }
        public static QuarkAssetObjectWapper operator ++(QuarkAssetObjectWapper quarkAsset)
        {
            quarkAsset.AssetReferenceCount++;
            return quarkAsset;
        }
        public static bool operator ==(QuarkAssetObjectWapper a, QuarkAssetObjectWapper b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(QuarkAssetObjectWapper a, QuarkAssetObjectWapper b)
        {
            return !a.Equals(b);
        }
    }
}
