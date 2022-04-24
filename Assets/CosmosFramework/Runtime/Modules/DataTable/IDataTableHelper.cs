using System;
namespace Cosmos.DataTable
{
    public interface IDataTableHelper
    {
        event Action<DataTableBase> OnReadDataTableSuccess;
        event Action<DataTableBase> OnReadDataTableFailure;
        void LoadDataTableAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable);
        void UnLoadDataTable(DataTableAssetInfo assetInfo);
    }
}
