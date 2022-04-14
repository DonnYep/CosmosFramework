using System;
namespace Quark
{
    /// <summary>
    /// 资源体的信息
    /// </summary>
    public struct QuarkAssetObjectInfo : IEquatable<QuarkAssetObjectInfo>
    {
        /// <summary>
        /// 资源名称；
        /// </summary>
        public string AssetName { get; private set; }
        /// <summary>
        /// 资源的相对路径；
        /// </summary>
        public string AssetPath { get; private set; }
        /// <summary>
        /// 资源所属AB包名；
        /// </summary>
        public string AssetBundleName { get; private set; }
        /// <summary>
        /// 资源在unity中的类型；
        /// </summary>
        public string AssetType { get; private set; }
        /// <summary>
        /// 引用计数；
        /// </summary>
        public int ReferenceCount { get; private set; }
        /// <summary>
        /// 源文件的后缀名；
        /// </summary>
        public string AssetExtension { get; private set; }
        public QuarkAssetObjectInfo Clone()
        {
             return Create(this.AssetName, this.AssetPath, this.AssetBundleName, this.AssetExtension,this.AssetType, this.ReferenceCount);
        }
        public bool Equals(QuarkAssetObjectInfo other)
        {
            return other.AssetName == this.AssetName &&
                other.AssetPath == this.AssetPath &&
                other.AssetBundleName == this.AssetBundleName &&
                other.AssetExtension == this.AssetExtension &&
                other.ReferenceCount == this.ReferenceCount&&
                other.AssetType==this.AssetType;
        }
        public override bool Equals(object obj)
        {
            return (obj is QuarkAssetObjectInfo) && Equals((QuarkAssetObjectInfo)obj);
        }
        public static QuarkAssetObjectInfo None { get { return new QuarkAssetObjectInfo(); } }
        public static QuarkAssetObjectInfo operator --(QuarkAssetObjectInfo quarkObjectInfo)
        {
            var latesetReferenceCount = quarkObjectInfo.ReferenceCount;
            var newInfo = QuarkAssetObjectInfo.Create(quarkObjectInfo.AssetName, quarkObjectInfo.AssetPath, quarkObjectInfo.AssetBundleName, quarkObjectInfo.AssetExtension, quarkObjectInfo.AssetType,latesetReferenceCount--);
            return newInfo;
        }
        public static QuarkAssetObjectInfo operator ++(QuarkAssetObjectInfo quarkObjectInfo)
        {
            var latesetReferenceCount = quarkObjectInfo.ReferenceCount;
            var newInfo = QuarkAssetObjectInfo.Create(quarkObjectInfo.AssetName, quarkObjectInfo.AssetPath, quarkObjectInfo.AssetBundleName, quarkObjectInfo.AssetExtension, quarkObjectInfo.AssetType,latesetReferenceCount++);
            return newInfo;
        }
        public static bool operator ==(QuarkAssetObjectInfo a, QuarkAssetObjectInfo b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(QuarkAssetObjectInfo a, QuarkAssetObjectInfo b)
        {
            return !a.Equals(b);
        }
        internal static QuarkAssetObjectInfo Create(string assetName, string assetPath, string assetBundleName, string assetExtension,string assetType, int referenceCount)
        {
            QuarkAssetObjectInfo info = new QuarkAssetObjectInfo();
            info.AssetName = assetName;
            info.AssetPath = assetPath;
            info.AssetBundleName = assetBundleName;
            info.ReferenceCount = referenceCount;
            info.AssetExtension = assetExtension;
            info.AssetType= assetType;
            return info;
        }
    }
}
