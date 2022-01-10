using Cosmos.Resource;

namespace Cosmos.Entity
{
    /// <summary>
    /// 实体资源信息；
    /// </summary>
    public class EntityAssetInfo : AssetInfo
    {
        readonly string entityGroupName;
        public string EntityGroupName { get { return entityGroupName; } }
        readonly bool useObjectPool;
        public bool UseObjectPool { get { return useObjectPool; } }
        public EntityAssetInfo(string entityGroupName, string assetBundleName, string assetPath, bool useObjectPool=false) :
            base(assetBundleName, assetPath)
        {
            this.entityGroupName = entityGroupName;
            this.useObjectPool = useObjectPool;
        }
        public EntityAssetInfo(string entityGroupName, string assetPath, bool useObjectPool=false) : base(assetPath)
        {
            this.entityGroupName = entityGroupName;
            this.useObjectPool = useObjectPool;
        }
    }
}
