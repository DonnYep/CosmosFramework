using Cosmos;
using PureMVC;
using UnityEngine;
public class PRX_Inventory : Proxy
{
    public new const string NAME = "PRX_Inventory";

    string dataSetPath = "DataSet/Inventory/DefaultInventory_Dataset";
    string jsonFilePath;
    public GameObject SlotAsset { get; private set; }
    public PRX_Inventory() : base(NAME) { }

    public InventoryDataset InventoryDataSet { get; private set; }
    public async override void OnRegister()
    {
        InventoryDataSet = await CosmosEntry.ResourceManager.LoadAssetAsync<UnityEngine.Object>(dataSetPath) as InventoryDataset;
        SlotAsset = await CosmosEntry.ResourceManager.LoadPrefabAsync("UI/Slot");
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
        try
        {
            string json = Utility.IO.ReadTextFileContent(jsonFilePath, "InventoryCache.json");
            JsonUtility.FromJsonOverwrite(json, InventoryDataSet);
            Utility.Debug.LogInfo("LoadJsonDataFromLocal");
        }
        catch (System.Exception)
        {
            var path = Utility.IO.WebPathCombine(jsonFilePath, "InventoryCache.json");
            Utility.Debug.LogError($"{path} not exsit !");
        }
    }
}
