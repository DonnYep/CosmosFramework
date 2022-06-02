using Cosmos.DataTable;
using System;
using UnityEngine;
namespace Cosmos
{
    public class DefaultDataTableHelper : IDataTableHelper
    {
        Action<DataTableBase,byte[]> onReadDataTableSuccess;
        Action<DataTableBase,string> onReadDataTableFailure;

        public event Action<DataTableBase, byte[]> OnReadDataTableSuccess
        {
            add { onReadDataTableSuccess += value; }
            remove { onReadDataTableSuccess -= value; }
        }
        public event Action<DataTableBase, string> OnReadDataTableFailure
        {
            add { onReadDataTableFailure += value; }
            remove { onReadDataTableFailure -= value; }
        }
        public void LoadDataTableAsync(DataTableAssetInfo assetInfo, DataTableBase dataTable)
        {
            CosmosEntry.ResourceManager.LoadAssetAsync<TextAsset>(assetInfo.AssetName, (asset) => { OnLoadDone(asset, dataTable); });
        }
        public void UnLoadDataTable(DataTableAssetInfo assetInfo)
        {
            CosmosEntry.ResourceManager.UnloadAsset(assetInfo.AssetName);
        }
        void OnLoadDone(TextAsset asset, DataTableBase dataTable)
        {
            if (asset != null)
            {
                onReadDataTableSuccess?.Invoke(dataTable,asset.bytes);
            }
            else
            {
                onReadDataTableFailure?.Invoke(dataTable,$"{dataTable.Name} read data failure ");
            }
        }
    }
}
