using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.Quark 
{
    /// <summary>
    /// Quark基于AssetDatabase的对象；
    /// 在Editor模式用作寻址；
    /// </summary>
    [Serializable]
    public struct QuarkAssetDatabaseObject
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
        /// 资源在unity中的GUID；
        /// </summary>
        public string AssetGuid;
        public static QuarkAssetDatabaseObject None { get { return new QuarkAssetDatabaseObject(); } }
    }
}