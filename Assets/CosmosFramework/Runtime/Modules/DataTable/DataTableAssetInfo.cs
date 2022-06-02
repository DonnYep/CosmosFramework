using System.Runtime.InteropServices;
namespace Cosmos.DataTable
{
    [StructLayout(LayoutKind.Auto)]
    public struct DataTableAssetInfo
    {
        readonly string dataTableName;
        public string DataTableName { get { return dataTableName; } }
        public string AssetName { get; private set; }
        public DataTableAssetInfo(string dataTableName) : this(dataTableName, dataTableName) { }
        public DataTableAssetInfo(string dataTableName, string assetName)
        {
            this.dataTableName = dataTableName;
            AssetName = assetName;
        }
    }
}
