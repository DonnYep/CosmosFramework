namespace Cosmos.Entity
{
    /// <summary>
    /// 实体资源信息；
    /// </summary>
    public struct EntityAssetInfo
    {
        public string AssetName { get; private set; }
        public string EntityName { get; private set; }
        public string EntityGroupName { get; private set; }
        public EntityAssetInfo(string assetName) : this(assetName, assetName, string.Empty) { }
        public EntityAssetInfo(string assetName, string entityName) : this(assetName, entityName, string.Empty) { }
        public EntityAssetInfo(string assetName, string entityName, string entityGroupName)
        {
            this.AssetName = assetName;
            this.EntityName = entityName;
            this.EntityGroupName = entityGroupName;
        }
    }
}
