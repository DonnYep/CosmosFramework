using System.Runtime.InteropServices;

namespace Cosmos
{
    [StructLayout(LayoutKind.Auto)]
    public struct ObjectPoolAssetInfo
    {
        public string AssetName { get; private set; }
        public string PoolName { get; private set; }
        public ObjectPoolAssetInfo(string poolName, string assetName)
        {
            this.PoolName = poolName;
            this.AssetName = assetName;
        }
    }
}
