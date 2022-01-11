using System;
namespace Quark.Asset
{
    /// <summary>
    /// Quark基于AB的对象；
    /// 用于Built后AB资源的寻址；
    /// </summary>
    [Serializable]
    internal class QuarkAssetBundleObject
    {
        /// <summary>
        ///  资源的名称；
        /// </summary>
        public string AssetName { get; set; }
        /// <summary>
        /// 资源的后缀名；
        /// </summary>
        public string AssetExtension { get; set; }
        /// <summary>
        /// 资源在Assets目录下的相对路径；
        /// </summary>
        public string AssetPath { get; set; }
        /// <summary>
        /// 资源在unity中的类型；
        /// </summary>
        public string AssetType { get; set; }
        /// <summary>
        /// 资源所在的AB包的名称；
        /// </summary>
        public string AssetBundleName { get; set; }
    }
}
