using System;
namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public abstract class AssetAttribute : Attribute
    {
        /// <summary>
        /// AB包名
        /// </summary>
        public string AssetBundleName { get; private set; }
        /// <summary>
        /// AB包地址、Resource地址或其他路径地址；
        /// </summary>
        public string AssetPath { get; private set; }
        public AssetAttribute(string assetBundleName, string assetPath)
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
        }
        public AssetAttribute(string assetPath) : this(null,assetPath){}
    }
}