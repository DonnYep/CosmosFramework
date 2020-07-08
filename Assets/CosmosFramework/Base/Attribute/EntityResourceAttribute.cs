using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public class EntityResourceAttribute : Attribute
    {
        public EntityResourceAttribute(string assetBundleName, string assetPath, string resourcePath, string useObjectPool)
        {
            AssetBundleName = assetBundleName;
            AssetPath = assetPath;
            ResourcePath = resourcePath;
            UseObjectPool = useObjectPool;
        }
        /// <summary>
        /// AB包名
        /// </summary>
        public string AssetBundleName { get; private set; }
        /// <summary>
        /// 基于AB包的资源的路径
        /// </summary>
        public string AssetPath{ get; private set; }
        /// <summary>
        /// 基于Resource的资源路径
        /// </summary>
        public string ResourcePath{ get; private set; }
        /// <summary>
        /// 是否使用对象池进行生成
        /// </summary>
        public string UseObjectPool{ get; private set; }
    }
}