using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Mvvm;
namespace Cosmos.Test
{
    public class M_Inventory:Model
    {
        string dataSetPath = "Inventory/Inventory_DataSet";
        InventoryDataSet inventoryDataSet;
        public InventoryDataSet InventoryDataSet { get { return inventoryDataSet; } set { inventoryDataSet = value; } }
        public M_Inventory()
        {
            inventoryDataSet = CosmosEntry.ResourceManager.LoadAsset<InventoryDataSet>(new AssetInfo(dataSetPath));
        }
    }
}
