using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public class EntityAssetAttribute : AssetAttribute
    {
        public EntityAssetAttribute(string assetBundleName, string assetPath, string resourcePath, bool useObjectPool):base(assetBundleName,assetPath,resourcePath)
        {
            UseObjectPool = useObjectPool;
        }
        /// <summary>
        /// 是否使用对象池进行生成
        /// </summary>
        public bool UseObjectPool{ get; private set; }
    }
}