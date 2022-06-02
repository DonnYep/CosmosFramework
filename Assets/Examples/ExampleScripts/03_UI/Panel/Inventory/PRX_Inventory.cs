using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.Resource;
using PureMVC;
using UnityEngine;
public class PRX_Inventory : Proxy
{
    public new const string NAME = "PRX_Inventory";

    string dataSetPath = "DataSet/Inventory/DefaultInventory_Dataset";
    string jsonFilePath;

    public PRX_Inventory() : base(NAME) { }

    public InventoryDataset InventoryDataSet { get; private set; }
    public override void OnRegister()
    {
        InventoryDataSet = CosmosEntry.ResourceManager.LoadAsset<InventoryDataset>(new AssetInfo(dataSetPath));
        //if (InventoryDataSet != null)
        //    Utility.Debug.LogInfo("InventoryDataset数据加载成功", MessageColor.ORANGE);
        jsonFilePath = Utility.IO.WebPathCombine(Application.persistentDataPath, "Inventory");
    }
    public void SaveJson()
    {
        var json = JsonUtility.ToJson(InventoryDataSet);
        Utility.IO.OverwriteTextFile(jsonFilePath, "InventoryCache.json", json);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(InventoryDataSet);
#endif
        Utility.Debug.LogInfo("SaveJsonDataFromLocal");
    }
    public void LoadJson()
    {
        string json = Utility.IO.ReadTextFileContent(jsonFilePath, "InventoryCache.json");
        JsonUtility.FromJsonOverwrite(json, InventoryDataSet);
        Utility.Debug.LogInfo("LoadJsonDataFromLocal");
    }
}
