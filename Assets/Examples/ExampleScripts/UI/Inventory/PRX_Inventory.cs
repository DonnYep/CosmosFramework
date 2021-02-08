using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Mvvm;
using UnityEngine;

namespace Cosmos.Test
{
    public class PRX_Inventory : Proxy
    {
        string dataSetPath = "DataSet/Inventory/Inventory_DataSet";
        string jsonFilePath;
        public InventoryDataSet InventoryDataSet { get; private set; }
        public override string ProxyName { get; protected set; } = MVVMDefine.PRX_Inventory;
        public override void OnRegister()
        {
            InventoryDataSet = CosmosEntry.ResourceManager.LoadAsset<InventoryDataSet>(new AssetInfo(dataSetPath));
            if (InventoryDataSet != null)
                Utility.Debug.LogInfo("InventoryDataSet数据加载成功", MessageColor.ORANGE);
            jsonFilePath = Utility.IO.CombineRelativePath(Application.persistentDataPath, "Inventory");
            Utility.Debug.LogInfo(jsonFilePath, MessageColor.ORANGE);
        }
        public override void OnRemove() { }
        public void SaveJson()
        {
            var json = JsonUtility.ToJson(InventoryDataSet);
            Utility.IO.OverwriteTextFile(jsonFilePath, "InventoryCache.json", json);
            Utility.Debug.LogInfo("SaveJsonDataFromLocal");
        }
        public void LoadJson()
        {
            string json = Utility.IO.ReadTextFileContent(jsonFilePath, "InventoryCache.json");
            JsonUtility.FromJsonOverwrite(json, InventoryDataSet);
            Utility.Debug.LogInfo("LoadJsonDataFromLocal");
        }
    }
}
