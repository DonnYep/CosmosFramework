using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    public enum ControlMode:int
    {
        /// <summary>
        /// 自由控制
        /// </summary>
        FreeControl=0,
        /// <summary>
        /// 第一人称控制
        /// </summary>
        FirstPerson=1,
        /// <summary>
        /// 第三人称控制
        /// </summary>
        ThirdPerson=2
    }
}
namespace Cosmos.Controller{

    /// <summary>
    /// 控制器模块，客户端本地玩家的主要控制器。
    /// 从InputManager获取值后，由此控制输入值
    /// </summary>
    public class ControllerManager : Module<ControllerManager>
    {
        Dictionary<Type, CFController> controllerMap = new Dictionary<Type, CFController>();
        /// <summary>
        /// 相机跟随对象
        /// 当操纵Vehicle，Deveice时，通过事件中心由使用者切换跟随对象
        /// </summary>
        ControlMode currentControlMode = ControlMode.ThirdPerson;
        public short ControllerCount { get; private set; }
        protected override void InitModule()
        {
            RegisterModule(CFModule.Controller);
        }
        public void RegisterController<T>(T controller)
            where T : CFController
        {
            if (!controllerMap.ContainsKey(typeof(T)))
            {
                controllerMap.Add(controller.GetType(), controller);
                ControllerCount++;
            }
        }
        public void DeregisterController<T>(T controller)
            where T : CFController
        {
            if (controllerMap.ContainsKey(typeof(T)))
            {
                controllerMap.Remove(controller.GetType());
                ControllerCount--;
            }
            else
                Utility.DebugError("Controller : " + controller.name + "   not  registered");
        }
        public bool HasController<T>(T controller)
            where T : CFController
        {
            return controllerMap.ContainsKey(controller.GetType());
        }
        public T GetController<T>()
            where T:CFController
        {
            Type type = typeof(T);
            if (controllerMap.ContainsKey(type))
                return controllerMap[type] as T;
            else return default(T);
        }
        public void ClearAllController()
        {
            controllerMap.Clear();
            ControllerCount = 0;
        }
        /// <summary>
        /// 更改控制状态
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="callBack">回调函数，与具体mode无关</param>
        public void SetControlMode(ControlMode mode, CFAction callBack = null)
        {
            switch (mode)
            {
                case ControlMode.FirstPerson:
                    break;
                case ControlMode.FreeControl:
                    break;
                case ControlMode.ThirdPerson:
                    break;
            }
            currentControlMode = mode;
            callBack?.Invoke();
        }
        public ControlMode GetControlMode()
        {
            return currentControlMode;
        }
    }
}