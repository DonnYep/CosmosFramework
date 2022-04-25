using System;
namespace Cosmos.DataTable
{
    public interface IDataTableHelper
    {
        /// <summary>
        /// DataTableBase===DataTableBytes
        /// </summary>
        event Action<DataTableBase, byte[]> OnReadDataTableSuccess;
        /// <summary>
        /// DataTableBase===ErrorMessage
        /// </summary>
        event Action<DataTableBase,string> OnReadDataTableFailure;
        void LoadDataTableAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable);
        void UnLoadDataTable(DataTableAssetInfo assetInfo);
    }
}
