using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =true,Inherited =false)]
    public class EntityAssetAttribute : AssetAttribute
    {
        public EntityAssetAttribute(string entityGroupName, string assetBundleName, string assetPath, string resourcePath)
            :base(assetBundleName,assetPath,resourcePath)
        {
            this.EntityGroupName = entityGroupName;
        }
        public EntityAssetAttribute(string entityGroupName, string resourcePath)
    : base(string.Empty,string.Empty, resourcePath)
        {
            this.EntityGroupName = entityGroupName;
        }
        /// <summary>
        /// 是否使用对象池进行生成
        /// </summary>
        public bool UseObjectPool{ get;  set; }
        /// <summary>
        /// 实体组名称；
        /// </summary>
        public string EntityGroupName { get; private set; }
    }
}