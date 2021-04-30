using System;
using UnityEngine;

namespace Cosmos
{
    public interface IControllerManager:IModuleManager
    {
        /// <summary>
        /// 当前控制模式；
        /// </summary>
        ControlMode CurrentControlMode { get; }
        int ControllerTypeCount { get; }

        /// <summary>
        /// 创建一个Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="go">挂载的目标点</param>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>创建后的对象</returns>
        T CreateController<T>(GameObject go, string controllerName = null) where T : Component, IController;
        /// <summary>
        /// 销毁Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controllerName">Controller名称</param>
        void DestoryController<T>(string controllerName) where T : Component, IController;
        void DestoryController(Type type, string controllerName);
        /// <summary>
        /// 注册Controller；
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controller">Controller对象</param>
        /// <returns>是否注册成功</returns>
        bool RegisterController<T>(T controller) where T : Component, IController;
        /// <summary>
        /// 注册Controller；
        /// </summary>
        /// <param name="type">Controller类型</param>
        /// <param name="controller">Controller对象</param>
        /// <returns>是否注册成功</returns>
        bool RegisterController(Type type, IController controller);
        /// <summary>
        /// 注销Controller，此方法不会销毁Controller；
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controller">Controller对象</param>
        /// <returns>是否注销成功</returns>
        bool DeregisterController<T>(T controller) where T : Component, IController;
        /// <summary>
        /// 注销Controller，此方法不会销毁Controller；
        /// </summary>
        /// <param name="type">Controller类型</param>
        /// <param name="controller">Controller对象</param>
        /// <returns>是否注销成功</returns>
        bool DeregisterController(Type type, IController controller);
        /// <summary>
        /// 是否存在指定类型，指定名称的Controller;
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>是否存在</returns>
        bool HasController<T>(string controllerName) where T : Component, IController;
        /// <summary>
        /// 是否存在指定类型，指定名称的Controller;
        /// </summary>
        /// <param name="type">Controller类型</param>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>是否存在</returns>
        bool HasController(Type type, string controllerName);
        /// <summary>
        /// 是否存在指定类型的Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <returns>是否存在</returns>
        bool HasControllerType<T>() where T : Component, IController;
        bool HasControllerType(Type type);
        /// <summary>
        /// 获得目标类型的指定名称的Controller;
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>Controller对象</returns>
        T GetController<T>(string controllerName) where T : Component, IController;
        IController GetController(Type type, string controllerName);
        /// <summary>
        /// 条件查找Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="predicate">条件委托</param>
        /// <returns>Controller对象</returns>
        T GetController<T>(Predicate<T> predicate) where T : Component, IController;
        /// <summary>
        /// 条件查找所有符合的Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="predicate">条件委托</param>
        /// <returns>Controller数组</returns>
        T[] GetControllers<T>(Predicate<T> predicate) where T : Component, IController;
        int GetControllerCount<T>() where T : Component, IController;
        /// <summary>
        /// 获取指定类型ctrl的数量；
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>数量</returns>
        int GetControllerCount(Type type);
        void ClearAllController();
        /// <summary>
        /// 清理并销毁指定类型的Controller类型;
        /// </summary>
        /// <typeparam name="T">Controller名称</typeparam>
        void DestroyControllers<T>() where T : Component, IController;
        void DestroyControllers(Type type);
        /// <summary>
        /// 更改控制状态
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="callback">回调函数，与具体mode无关</param>
        void SetControlMode(ControlMode mode, Action callback = null);
    }
}
