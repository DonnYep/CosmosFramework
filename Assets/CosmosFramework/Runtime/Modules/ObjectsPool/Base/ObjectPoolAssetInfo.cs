using Cosmos.Resource;
namespace Cosmos
{
    public class ObjectPoolAssetInfo : AssetInfo
    {
        public string PoolName { get; private set; }
        public ObjectPoolAssetInfo(string poolName, string assetPath) : this(poolName, string.Empty, assetPath) { }
        public ObjectPoolAssetInfo(string poolName, string assetBundleName, string assetPath)
            : base(assetBundleName, assetPath)
        {
            this.PoolName = poolName;
        }
    }
}
