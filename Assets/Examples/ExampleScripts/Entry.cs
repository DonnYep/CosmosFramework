using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cosmos.UI;
using Cosmos.Mvvm;
using Cosmos.Resource;

namespace Cosmos.Test
{
    /// <summary>
    /// 配置测试类，仅用于案例部分；
    /// </summary>
    [DefaultExecutionOrder(2000)]
    public class Entry : CosmosConfig
    {
        [SerializeField] bool loadDefaultHelper=true;
        protected override void Awake()
        {
            if (loadDefaultHelper)
                CosmosEntry.LaunchAppDomainHelpers();
            CosmosEntry.PrintModulePreparatory = printModulePreparatory;
            if (launchAppDomainModules)
            {
                CosmosEntry.LaunchAppDomainModules();
                CosmosEntry.InputManager.SetInputDevice(new StandardInputDevice());
                CosmosEntry.ResourceManager.SwitchBuildInLoadMode(ResourceLoadMode);
            }
        }
    }
}