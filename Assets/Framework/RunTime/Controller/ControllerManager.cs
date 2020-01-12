using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Controller{
    /// <summary>
    /// 控制器模块，客户端本地玩家的主要控制器
    /// </summary>
    public class ControllerManager : Module<ControllerManager>
    {
        protected override void InitModule()
        {
            RegisterModule(CFModule.Controller);
        }
    }
}