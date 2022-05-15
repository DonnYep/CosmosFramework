using System;
namespace Quark.Asset
{
    /// <summary>
    /// Quark资源寻址对象；
    /// </summary>
    [Serializable]
    public class QuarkAssetObject : IEquatable<QuarkAssetObject>
    {
        /// <summary>
        ///  资源的名称；
        /// </summary>
        public string AssetName;
        /// <summary>
        /// 资源的后缀名；
        /// </summary>
        public string AssetExtension;
        /// <summary>
        /// 资源在Assets目录下的相对路径；
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// 资源在unity中的类型；
        /// </summary>
        public string AssetType;
        /// <summary>
        /// 资源所在的AB包的名称；
        /// </summary>
        public string AssetBundleName;
        public bool Equals(QuarkAssetObject other)
        {
            return other.AssetName == this.AssetName &&
                other.AssetPath == this.AssetPath &&
                other.AssetBundleName == this.AssetBundleName &&
                other.AssetExtension == this.AssetExtension;
        }
        public override bool Equals(object obj)
        {
            return (obj is QuarkAssetObjectWapper) && Equals((QuarkAssetObjectWapper)obj);
        }
    }
}