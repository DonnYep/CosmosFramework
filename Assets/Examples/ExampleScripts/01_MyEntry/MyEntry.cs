using UnityEngine;
using Cosmos;
public class MyEntry : MonoBehaviour
{
    [SerializeField] bool printModulesInitInfo = true;
    void Awake()
    {
        Utility.Debug.SetHelper(new StandaloneDebugHelper());
        Utility.Json.SetHelper(new JsonUtilityHelper());
        CosmosEntry.PrintModulePreparatory = printModulesInitInfo;
        CosmosEntry.LaunchAppDomainModules();
    }
}
