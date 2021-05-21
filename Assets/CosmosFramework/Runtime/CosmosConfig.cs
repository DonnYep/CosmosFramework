using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.QuarkAsset;
using Cosmos.Resource;
namespace Cosmos
{
    [DefaultExecutionOrder(-1000)]
    public class CosmosConfig : MonoBehaviour
    {
        [Header("Cosmos入口配置")]
        [SerializeField]
        ResourceLoadMode ResourceLoadMode;
        private void Awake()
        {
            if (ResourceLoadMode != ResourceLoadMode.QuarkAsset)
                CosmosEntry.ResourceManager.SwitchBuildInLoadMode(ResourceLoadMode);
            else
            {
            }
        }
    }
}