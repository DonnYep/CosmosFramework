using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.Controller{
    public enum ControlMode
    {
        /// <summary>
        /// 自由控制
        /// </summary>
        FreeControl,
        /// <summary>
        /// 第一人称控制
        /// </summary>
        FirstPerson,
        /// <summary>
        /// 第三人称控制
        /// </summary>
        ThirdPerson
    }
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