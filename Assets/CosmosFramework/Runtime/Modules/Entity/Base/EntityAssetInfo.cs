using System.Runtime.InteropServices;
namespace Cosmos.Entity
{
    /// <summary>
    /// 实体资源信息；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct EntityAssetInfo
    {
        public string AssetName { get; private set; }
        public string EntityGroupName { get; private set; }
        public bool UseObjectPool { get; private set; }
        public EntityAssetInfo(string entityGroupName, string assetName, bool useObjectPool = false)
        {
            this.EntityGroupName = entityGroupName;
            this.AssetName = assetName;
            this.UseObjectPool = useObjectPool;
        }
    }
}
