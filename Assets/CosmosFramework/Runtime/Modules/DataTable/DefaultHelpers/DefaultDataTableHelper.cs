using Cosmos.DataTable;
using System;
using UnityEngine;
namespace Cosmos
{
    public class DefaultDataTableHelper : IDataTableHelper
    {
        Action<DataTableBase> onReadDataTableSuccess;
        Action<DataTableBase> onReadDataTableFailure;

        public event Action<DataTableBase> OnReadDataTableSuccess
        {
            add { onReadDataTableSuccess += value; }
            remove { onReadDataTableSuccess -= value; }
        }
        public event Action<DataTableBase> OnReadDataTableFailure
        {
            add { onReadDataTableFailure += value; }
            remove { onReadDataTableFailure -= value; }
        }
        public void LoadDataTableAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable)
        {
            CosmosEntry.ResourceManager.LoadAssetAsync<TextAsset>(assetInfo, (asset) => { OnLoadDone(asset, dataTable); });
        }
        public void UnLoadDataTable(DataTableAssetInfo assetInfo)
        {
            CosmosEntry.ResourceManager.UnLoadAsset(assetInfo);
        }
        void OnLoadDone(TextAsset asset, DataTableBase dataTable)
        {
            if (asset != null)
            {
                dataTable.ReadDataTable(asset.bytes);
                onReadDataTableSuccess?.Invoke(dataTable);
            }
            else
            {
                onReadDataTableFailure?.Invoke(dataTable);
            }
        }
    }
}
