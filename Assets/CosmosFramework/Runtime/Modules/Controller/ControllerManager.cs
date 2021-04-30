using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Cosmos
{
    public enum ControlMode : int
    {
        /// <summary>
        /// 自由控制
        /// </summary>
        FreeControl = 0,
        /// <summary>
        /// 第一人称控制
        /// </summary>
        FirstPerson = 1,
        /// <summary>
        /// 第三人称控制
        /// </summary>
        ThirdPerson = 2
    }
}
namespace Cosmos.Controller
{

    /// <summary>
    /// 控制器模块，客户端本地玩家的主要控制器。
    /// 从InputManager获取值后，由此控制输入值
    /// </summary>
    [Module]
    internal sealed class ControllerManager : Module, IControllerManager
    {
        #region Properties
        Dictionary<Type, ControllerPool> controllerDict;
        Type controllerType = typeof(IController);
        Action ctrlPoolRefresh;
        event Action CtrlPoolRefresh
        {
            add { ctrlPoolRefresh += value; }
            remove { ctrlPoolRefresh -= value; }
        }
        /// <summary>
        /// 相机跟随对象
        /// 当操纵Vehicle，Deveice时，通过事件中心由使用者切换跟随对象
        /// </summary>
        ControlMode currentControlMode = ControlMode.ThirdPerson;
        /// <summary>
        /// 当前控制模式；
        /// </summary>
        public ControlMode CurrentControlMode
        {
            get { return currentControlMode; }
        }
        public int ControllerTypeCount
        {
            get { return controllerDict.Count; }
        }
        #endregion

        #region Methods
        public override void OnInitialization()
        {
            controllerDict = new Dictionary<Type, ControllerPool>();
        }
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            ctrlPoolRefresh?.Invoke();
        }
        /// <summary>
        /// 创建一个Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="go">挂载的目标点</param>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>创建后的对象</returns>
        public T CreateController<T>(GameObject go, string controllerName = null)
    where T : Component, IController
        {
            ControllerPool ctrlPool;
            var key = typeof(T);
            if (!controllerDict.TryGetValue(key, out ctrlPool))
            {
                ctrlPool = new ControllerPool();
                controllerDict.Add(key, ctrlPool);
                CtrlPoolRefresh+= ctrlPool.OnRefresh;
            }
            return ctrlPool.CreateController<T>(go, controllerName);
        }
        /// <summary>
        /// 销毁Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controllerName">Controller名称</param>
        public void DestoryController<T>(string controllerName)
where T : Component, IController
        {
            DestoryController(typeof(T),controllerName);
        }
        public void DestoryController(Type type, string controllerName)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            if (controllerDict.TryGetValue(type, out var ctrlPool))
            {
                ctrlPool.DestoryController(controllerName);
                if (ctrlPool.ControllerCount <= 0)
                {
                    controllerDict.Remove(type);
                    CtrlPoolRefresh -= ctrlPool.OnRefresh;
                }
            }
        }
        /// <summary>
        /// 注册Controller；
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controller">Controller对象</param>
        /// <returns>是否注册成功</returns>
        public bool RegisterController<T>(T controller)
           where T : Component, IController
        {
            return RegisterController(typeof(T), controller);
        }
        /// <summary>
        /// 注册Controller；
        /// </summary>
        /// <param name="type">Controller类型</param>
        /// <param name="controller">Controller对象</param>
        /// <returns>是否注册成功</returns>
        public bool RegisterController(Type type, IController controller)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            ControllerPool ctrlPool = null; ;
            if (!controllerDict.ContainsKey(type))
            {
                ctrlPool = new ControllerPool();
                controllerDict.Add(type, ctrlPool);
                CtrlPoolRefresh += ctrlPool.OnRefresh;
            }
            return ctrlPool.AddController(controller);
        }
        /// <summary>
        /// 注销Controller，此方法不会销毁Controller；
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controller">Controller对象</param>
        /// <returns>是否注销成功</returns>
        public bool DeregisterController<T>(T controller)
           where T : Component , IController
        {
            return DeregisterController(typeof(T), controller);
        }
        /// <summary>
        /// 注销Controller，此方法不会销毁Controller；
        /// </summary>
        /// <param name="type">Controller类型</param>
        /// <param name="controller">Controller对象</param>
        /// <returns>是否注销成功</returns>
        public bool DeregisterController(Type type, IController controller)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            bool result = true;
            if (controllerDict.TryGetValue(type,out var ctrlPool))
            {
                ctrlPool.RemoveController(controller);
                if (ctrlPool.ControllerCount <= 0)
                {
                    controllerDict.Remove(type);
                    CtrlPoolRefresh -= ctrlPool.OnRefresh;
                }
            }
            return result;
        }
        /// <summary>
        /// 是否存在指定类型，指定名称的Controller;
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>是否存在</returns>
        public bool HasController<T>(string controllerName)
           where T : Component, IController
        {
            if (controllerDict.TryGetValue(typeof(T),out var ctrlPool))
            {
               return ctrlPool.HasController(controllerName);
            }
            return false;
        }
        /// <summary>
        /// 是否存在指定类型，指定名称的Controller;
        /// </summary>
        /// <param name="type">Controller类型</param>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>是否存在</returns>
        public bool HasController(Type type,  string controllerName)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            if (!HasControllerType(type))
                return false;
            else
                return controllerDict[type].HasController(controllerName);
        }
        /// <summary>
        /// 是否存在指定类型的Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <returns>是否存在</returns>
        public bool HasControllerType<T>()
           where T : Component, IController
        {
            return controllerDict.ContainsKey(typeof(T));
        }
        public bool HasControllerType(Type type)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            return controllerDict.ContainsKey(type);
        }
        /// <summary>
        /// 获得目标类型的指定名称的Controller;
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>Controller对象</returns>
        public T GetController<T>(string controllerName)
           where T : Component, IController
        {
            var key = typeof(T);
            bool result = controllerDict.TryGetValue(key, out var ctrlPool);
            if (!result)
                throw new ArgumentNullException("ControllerManager " + "Controller Type : " + key.FullName + " has not registered");
            return ctrlPool.GetController(controllerName) as T;
        }
        public IController GetController(Type type, string controllerName)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            bool result = controllerDict.TryGetValue(type, out var ctrlPool);
            if (!result)
                throw new ArgumentNullException("ControllerManager " + "Controller Type : " + type.FullName + " has not registered");
            return ctrlPool.GetController(controllerName) ;
        }
        /// <summary>
        /// 条件查找Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="predicate">条件委托</param>
        /// <returns>Controller对象</returns>
        public T GetController<T>(Predicate<T> predicate)
           where T : Component, IController
        {
            var key = typeof(T);
            bool result = controllerDict.TryGetValue(key, out var ctrlPool);
            if (!result)
                throw new ArgumentNullException("ControllerManager " + "Controller Type : " + key.FullName + " has not registered");
            return ctrlPool.GetController<T>(predicate);
        }
        /// <summary>
        /// 条件查找所有符合的Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="predicate">条件委托</param>
        /// <returns>Controller数组</returns>
        public T[] GetControllers<T>(Predicate<T> predicate)
           where T : Component, IController
        {
            var key = typeof(T);
            bool result = controllerDict.TryGetValue(key, out var ctrlPool);
            if (!result)
                throw new ArgumentNullException("ControllerManager " + "Controller Type : " + key.FullName + " has not registered");
            return ctrlPool.GetControllers<T>(predicate);
        }
        public int GetControllerCount<T>()
           where T : Component, IController
        {
            return GetControllerCount(typeof(T));
        }
        /// <summary>
        /// 获取指定类型ctrl的数量；
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>数量</returns>
        public int GetControllerCount(Type type)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            if (controllerDict.ContainsKey(type))
                return controllerDict[type].ControllerCount;
            else
                throw new ArgumentNullException("ControllerManager-->>" + "Controller Type: " + type.ToString() + "  has not  registered");
        }
        public void ClearAllController()
        {
            foreach (var ctrlPool in controllerDict)
            {
                ctrlPool.Value.DestroyControllers();
            }
            controllerDict.Clear();
        }
        /// <summary>
        /// 清理并销毁指定类型的Controller类型;
        /// </summary>
        /// <typeparam name="T">Controller名称</typeparam>
        public void DestroyControllers<T>()
           where T : Component, IController
        {
            DestroyControllers(typeof(T));
        }
        public void DestroyControllers(Type type)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            if (HasControllerType(type))
            {
                controllerDict.TryRemove(type, out var ctrlPool);
                CtrlPoolRefresh -= ctrlPool.OnRefresh;
                ctrlPool.DestroyControllers();
            }
            else
                throw new ArgumentNullException("ControllerManager-->>" + "Controller Type: " + type.ToString() + "  has not  registered");
        }
        /// <summary>
        /// 更改控制状态
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="callback">回调函数，与具体mode无关</param>
        public void SetControlMode(ControlMode mode, Action callback = null)
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
            callback?.Invoke();
        }
        #endregion
    }
}