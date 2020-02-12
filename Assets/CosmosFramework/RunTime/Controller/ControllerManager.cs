using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    /// 控制器模块，客户端本地玩家的主要控制器。
    /// 从InputManager获取值后，由此控制输入值
    /// </summary>
    public class ControllerManager : Module<ControllerManager>
    {
        Dictionary<CFController, CFController> controllerMap = new Dictionary<CFController, CFController>();
        public short ControllerCount { get; private set; }
        protected override void InitModule()
        {
            RegisterModule(CFModule.Controller);
        }
        public void RegisterController<T>(T controller)
            where T:CFController
        {
            if (!controllerMap.ContainsKey(controller))
            {
                controllerMap.Add(controller, null);
                ControllerCount++;
            }
        }
        public void DeregisterController<T>(T controller)
            where T : CFController
        {
            ControllerCount--;
        }
        public bool HasController<T>(T controller)
            where T : CFController
        {
            return false;
        }
        public void ClearAllController()
        {
            ControllerCount = 0;
        }
    }
}