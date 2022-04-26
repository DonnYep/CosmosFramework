using UnityEngine;
using Cosmos;
using Cosmos.Resource;

public class MyEntry : MonoBehaviour
{
    [SerializeField] bool printModulesInitInfo = true;
    [SerializeField] bool useQuark=false;
    void Awake()
    {
        Utility.Debug.SetHelper(new StandaloneDebugHelper());
        Utility.Json.SetHelper(new JsonUtilityHelper());
        CosmosEntry.PrintModulePreparatory = printModulesInitInfo;
        CosmosEntry.LaunchAppDomainModules();
        if (useQuark)
            CosmosEntry.ResourceManager.AddOrUpdateBuildInLoadHelper(ResourceLoadMode.Resource, new QuarkLoader());
    }
}
