using System.Runtime.InteropServices;
namespace Cosmos.DataTable
{
    /// <summary>
    /// 数据表资源信息：表名 + 资源名。
    /// <para>默认两者相同（资源名即表名）；需要“表名与资源名不一致”（例如多语言变体）时用双参构造。</para>
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct DataTableAssetInfo
    {
        readonly string dataTableName;
        public string DataTableName { get { return dataTableName; } }
        public string AssetName { get; private set; }
        /// <summary>
        /// 是否携带有效资源名
        /// </summary>
        public bool IsValid { get { return !string.IsNullOrEmpty(AssetName); } }
        public DataTableAssetInfo(string dataTableName) : this(dataTableName, dataTableName) { }
        public DataTableAssetInfo(string dataTableName, string assetName)
        {
            this.dataTableName = dataTableName;
            AssetName = assetName;
        }
        public override string ToString()
        {
            return $"DataTable:{dataTableName} ;Asset:{AssetName}";
        }
    }
}
