using Cosmos.Resource;
namespace Cosmos.DataTable
{
    public class DataTableAssetInfo : AssetInfo
    {
        readonly string dataTableName;
        public string DataTableName { get { return dataTableName; } }
        public DataTableAssetInfo(string dataTableName) : this(dataTableName, string.Empty, dataTableName) { }
        public DataTableAssetInfo(string dataTableName, string assetPath) : this(dataTableName, string.Empty, assetPath) { }
        public DataTableAssetInfo(string dataTableName, string assetBundleName, string assetPath) : base(assetBundleName, assetPath)
        {
            this.dataTableName = dataTableName;
        }
    }
}
