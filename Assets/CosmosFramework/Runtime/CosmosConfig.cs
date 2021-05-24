using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.QuarkAsset;
using Cosmos.Resource;
namespace Cosmos
{
    [DefaultExecutionOrder(2000)]
    public class CosmosConfig : MonoBehaviour
    {
        [Header("Cosmos入口配置")]
        [SerializeField] protected bool launchAppDomainModules=true;
        [SerializeField] protected bool printModulePreparatory = true;
        [SerializeField] protected ResourceLoadMode ResourceLoadMode;
        protected virtual void Awake()
        {
            CosmosEntry.PrintModulePreparatory = printModulePreparatory;
            if (launchAppDomainModules)
            {
                CosmosEntry.LaunchAppDomainModules();
                CosmosEntry.ResourceManager.SwitchBuildInLoadMode(ResourceLoadMode);
            }
        }
    }
}