using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using Cosmos;
namespace Cosmos.Config
{
    /// <summary>
    /// 载入时候读取配置，例如声音大小，角色等
    /// </summary>
    public sealed class ConfigManager : Module<ConfigManager>
    {
        protected override void InitModule()
        {
            RegisterModule(CFModule.Config);
        }
    }
}