using UnityEngine;
using Cosmos.Resource;
namespace Cosmos
{
    [DefaultExecutionOrder(2000)]
    public class CosmosEntryConfig : MonoBehaviour
    {
        [Header("Cosmos入口配置")]
        public bool LaunchAppDomainModules = true;
        public bool PrintModulePreparatory = false;
        public bool LoadDefaultHelper = true;
        public ResourceLoadMode ResourceLoadMode;
        protected virtual void Awake()
        {
            if (LoadDefaultHelper)
                CosmosEntry.LaunchAppDomainHelpers();
            CosmosEntry.PrintModulePreparatory = PrintModulePreparatory;
            if (LaunchAppDomainModules)
            {
                CosmosEntry.LaunchAppDomainModules();
                CosmosEntry.ResourceManager.SwitchLoadMode(ResourceLoadMode);
            }
        }
    }
}