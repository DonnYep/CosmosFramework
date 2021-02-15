using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System.IO;
public class Upgrade : MonoBehaviour
{
    [SerializeField]
    string folderPath;
    List<DirectoryInfo> dirInfoList = new List<DirectoryInfo>();
    private void Awake()
    {
        CosmosEntry.LaunchHelpers();
        CosmosEntry.LaunchModules();
    }
    void Start()
    {
        CosmosEntry.MonoManager.StartCoroutine(RunUpgrade());
    }
    IEnumerator RunUpgrade()
    {
        var dirInfoRoot= Directory.CreateDirectory(folderPath);
        dirInfoList.Add(dirInfoRoot);
        GetAllDirectory(dirInfoRoot);
        foreach (var dir in dirInfoList)
        {
            var fileInfos = dir.GetFiles();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                if (fileInfos[i].Name.EndsWith(".cs"))
                {
                    var str= Utility.IO.ReadTextFileContent(fileInfos[i].FullName);
                    //str = str.Replace("UILogicTemporary", "UITemporaryForm");
                    //str = str.Replace("GetUIPanel", "GetUIForm");
                    //str = str.Replace("Utility.DebugError", "Utility.Debug.LogError");
                    //str = str.Replace("Facade.DispatchEvent", "CosmosEntry.EventManager.DispatchEvent");
                    //str = str.Replace("Utility.DebugLog", "Utility.Debug.LogInfo");
                    //str = str.Replace("RegisterUIPanel", "GetUIComponents");
                    //str = str.Replace("UILogicResident", "UIResidentForm");
                    //str = str.Replace("Facade.AddEventListener", "CosmosEntry.EventManager.AddListener");
                    //str = str.Replace("AddUIEventListener", "CosmosEntry.EventManager.AddListener");
                    //str = str.Replace("Facade.RemoveEventListener", "CosmosEntry.EventManager.RemoveListener");
                    //str = str.Replace("RemoveUIEventListener", "CosmosEntry.EventManager.RemoveListener");
                    //str = str.Replace("HidePanel()", "HideUIForm()");
                    //str = str.Replace("Facade.CustomeModule", "GameManager.GetModule");
                    //str = str.Replace("Facade.LoadResAsset<Sprite>(", "CosmosEntry.ResourceManager.LoadAsset<Sprite>(new AssetInfo( ");
                    //str = str.Replace("[PrefabUnit", "[PrefabAsset");
                    //str = str.Replace("Facade.GetConfigFloat", "CosmosEntry.ConfigManager.GetConfigFloat");
                    //str = str.Replace("CFDataSet", "DatasetBase");
                    //str = str.Replace("Facade.ShowPanel", "CosmosEntry.UIManager.OpenUIFormAsync");
                    //str = str.Replace("Facade.LoadResAsset<GameObject>(", "CosmosEntry.ResourceManager.LoadPrefab(new AssetInfo(");
                    //str = str.Replace("Facade.GetController", "CosmosEntry.ControllerManager.GetController");
                    //str = str.Replace("AddDefaultEventListener", "CosmosEntry.EventManager.AddListener");
                    //str = str.Replace("RemoveDefaultEventListener", "CosmosEntry.EventManager.RemoveListener");
                    //str = str.Replace("Facade.RefreshHandler", "GameManager.RefreshHandler");
                    //str = str.Replace("[CustomeModule]", "[Module]");
                    //str = str.Replace("Facade.SpawnReference", "CosmosEntry.ReferencePoolManager.Spawn");
                    //str = str.Replace("DispatchUIEvent", "CosmosEntry.EventManager.DispatchEvent");
                    //str = str.Replace("Facade.HidePanel", "CosmosEntry.UIManager.HideUIForm");
                    //str = str.Replace("Facade.LoadSceneAsync", "CosmosEntry.SceneManager.LoadSceneAsync");
                    //str = str.Replace("Utility.Global.TargetLevel", "AscensionUtility.Global.TargetLevel");
                    //str = str.Replace("Facade.LoadResAsset<TextAsset>(", "CosmosEntry.ResourceManager.LoadAsset<TextAsset>(new AssetInfo(");
                    //str = str.Replace("Facade.LoadResPrefab<", "CosmosEntry.ResourceManager.LoadPrefab<");
                    //str = str.Replace("GameManagerAgent", "MonoGameManager");
                    //str = str.Replace("ShowPanel();", "ShowUIForm()");
                    //str = str.Replace("AscensionAscensionAscensionAscensionAscensionUtility.Global.TargetLevel", "AscensionUtility.Global.TargetLevel");
                    //str = str.Replace("Facade.RemovePanel", "CosmosEntry.UIManager.RemoveUIForm");
                    //str = str.Replace("Facade.StartCoroutine", "CosmosEntry.MonoManager.StartCoroutine");
                    //str = str.Replace("Facade.DespawnsReference", "CosmosEntry.ReferencePoolManager.Despawn");
                    Utility.IO.OverwriteTextFile(fileInfos[i].FullName, str);
                }
            }
        }
        yield return null;
    }
    void GetAllDirectory(DirectoryInfo directoryInfo)
    {
        var dirs = directoryInfo.GetDirectories();
        if (dirs.Length > 0)
        {
            foreach (var dir in directoryInfo.GetDirectories())
            {
                dirInfoList.Add(dir);
                GetAllDirectory(dir);
            }
        }
    }
}
