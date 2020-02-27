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
        Dictionary<Type, HashSet<CFController>> controllerMap = new Dictionary<Type, HashSet<CFController>>();
        /// <summary>
        /// 相机跟随对象
        /// 当操纵Vehicle，Deveice时，通过事件中心由使用者切换跟随对象
        /// </summary>
        ControlMode currentControlMode = ControlMode.ThirdPerson;
        //Controller类型的数量，根据type计算
        short controllerTypeCount = 0;
        protected override void InitModule()
        {
            RegisterModule(CFModules.CONTROLLER);
        }
        public void RegisterController<T>(T controller)
            where T : CFController
        {
            var key = typeof(T);
            if (!controllerMap.ContainsKey(key))
            {
                controllerMap.Add(key, new HashSet<CFController>());
                controllerMap[key].Add(controller);
                controllerTypeCount++;
            }
            else
            {
                if( HasControllerItem(controller))
                    controllerMap[key].Add(controller);
                else
                    Utility.DebugError("ControllerManager\n" + "Controller : " + controller.ControllerName + "is  already registered");
            }
        }
        public void DeregisterController<T>(T controller)
            where T : CFController
        {
            var key = typeof(T);
            if (controllerMap.ContainsKey(key))
            {
                if (controllerMap[key].Contains(controller))
                {
                    controllerMap[key].Remove(controller);
                    if (controllerMap[key].Count == 0)
                    {
                        controllerMap.Remove(key);
                        controllerTypeCount--;
                    }
                }
            }
            else
                Utility.DebugError("ControllerManager\n"+"Controller : " + controller.ControllerName + "is  unregistered");
        }
        public bool HasController<T>()
            where T : CFController
        {
            return controllerMap.ContainsKey(typeof(T));
        }
        public bool HasControllerItem<T>(T controller)
            where T : CFController
        {
            if (!HasController<T>())
                return false;
            else
                return  controllerMap[typeof(T)].Contains(controller);
        }
        public T GetController<T>(CFPredicateAction<T> predicate)
            where T:CFController
        {
            var key = typeof(T);
            T temp = default(T);
            if (controllerMap.ContainsKey(key))
            {
                foreach (var  item in controllerMap[key])
                {
                    if (predicate(item as T))
                        return item as T;
                }
                return temp;
            }
            else
            {
                Utility.DebugError("ControllerManager"+"Controller : " + key.FullName + "is  unregistered");
                return temp;
            }
        }
        public T[] GetControllers<T>(CFPredicateAction<T> predicate)
            where T : CFController
        {
            var key = typeof(T);
            if (controllerMap.ContainsKey(key))
            {
                List<T> list = new List<T>();
                foreach (var item in controllerMap[key])
                {
                    if (predicate(item as T))
                    {
                        list.Add(item as T);
                    }
                }
                return list.ToArray();
            }
            else
            {
                Utility.DebugError("ControllerManager" + "Controller : " + key.FullName + "is  unregistered");
                return default(T[]);
            } 
        }
        public short GetControllerItemCount<T>()
        {
            var key = typeof(T);
            if (controllerMap.ContainsKey(key))
                return (short)controllerMap[key].Count;
            else
            {
                Utility.DebugError("ControllerManager"+"Controller : " + key.FullName + "is  unregistered");
                return -1;
            }
        }
        public short GetControllerTypeCount()
        {
            return controllerTypeCount;
        }
        public void ClearAllController()
        {
            controllerMap.Clear();
            controllerTypeCount = 0;
        }
        public void ClearControllerItem<T>()
            where T : CFController
        {
            if (HasController<T>())
                controllerMap[typeof(T)].Clear();
            else
                Utility.DebugError("ControllerManager\n"+"Controller : " + typeof(T).FullName + "is  unregistered");
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