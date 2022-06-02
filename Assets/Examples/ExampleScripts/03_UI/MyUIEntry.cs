using UnityEngine;
using Cosmos;
public class MyUIEntry : MonoBehaviour
{
    [SerializeField] bool printModulesInitInfo = true;
    void Awake()
    {
        Utility.Debug.SetHelper(new StandaloneDebugHelper());
        Utility.Json.SetHelper(new JsonUtilityHelper());
        CosmosEntry.PrintModulePreparatory = printModulesInitInfo;
        CosmosEntry.LaunchAppDomainModules();
    }
    private void Start()
    {
        var mianUICanvas = CosmosEntry.ResourceManager.LoadPrefabAsync("UI/MainUICanvas", go =>
       {
           CosmosEntry.UIManager.SetUIRoot(go.transform);
       }, null, true);
        PureMVC.MVC.RegisterCommand<CMD_Navigate>(MVCEventDefine.CMD_Navigate);
        PureMVC.MVC.Dispatch(MVCEventDefine.CMD_Navigate);
    }
}
