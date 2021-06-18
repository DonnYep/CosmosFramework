using System;

namespace Cosmos
{
    //================================================
    //1、控制器模块用于生成需要执行unity生命周期的对象。被创建的对象可以
    // 被unity的fixedupdate、lateupdate、update方法轮询；
    //
    //2、若控制器需要被轮询，则需要实现无参函数，并为需要被轮询的无参函数
    //挂载 [TickRefresh] [FixedRefresh][LateRefresh] 特性；
    //
    //3、控制器被生成时拥有唯一Id，并可以被赋予组别；所有控制器都可以被
    //命名，且名字可重复。若控制器有重名且无组别，则默认返回第一个被查询
    //到的控制器；
    //
    //4、控制器需要由模块生成，并被模块释放；
    //
    //5、控制器模块已玉unity解耦
    //================================================
    public interface IControllerManager:IModuleManager
    {
        /// <summary>
        /// 控制器组的数量；
        /// </summary>
        int ControllerGroupCount { get; }
        /// <summary>
        /// 创建一个controller；
        /// </summary>
        /// <typeparam name="T">持有者的类型</typeparam>
        /// <param name="controllerName">被创建controller的名字</param>
        /// <param name="owner">持有者对象</param>
        /// <returns>被创建的controller</returns>
        IController CreateController<T>(string controllerName, T owner) where T : class;
        /// <summary>
        /// 创建一个具有组别属性的controller；
        /// </summary>
        /// <typeparam name="T">持有者的类型</typeparam>
        /// <param name="groupName">controller所在的组的名称</param>
        /// <param name="controllerName">被创建controller的名字</param>
        /// <param name="owner">持有者对象</param>
        /// <returns>被创建的controller</returns>
        IController CreateController<T>(string groupName, string controllerName, T owner) where T : class;
        /// <summary>
        /// 是否存在controller组别；
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        bool HasControllerGroup(string groupName);
        /// <summary>
        /// 是否存在指定类型，指定名称的Controller;
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>是否存在</returns>
        bool HasController(int controllerId);
        /// <summary>
        /// 是否存在指定名字的controller；
        /// </summary>
        /// <param name="controllerName">controller name</param>
        /// <returns>是否存在</returns>
        bool HasController(string controllerName);
        /// <summary>
        /// 通过Id获取controller；
        /// </summary>
        /// <param name="controllerId">controller id</param>
        /// <param name="controller">返回的controller</param>
        /// <returns>是否存在</returns>
        bool GetController(int controllerId, out IController controller);
        /// <summary>
        /// 通过Id获取controller；
        /// </summary>
        /// <param name="controllerName">controller name</param>
        /// <param name="controller">返回的controller</param>
        /// <returns>是否存在</returns>
        bool GetController(string controllerName, out IController controller);
        /// <summary>
        ///获得指定tag下所有的controller；
        /// </summary>
        /// <param name="groupName">组的名称</param>
        /// <param name="controllers">返回的controller集合</param>
        /// <returns>是否存在</returns>
        bool GetControllers(string groupName, out IController[] controllers);
        /// <summary>
        /// 条件查找所有符合的Controller
        /// </summary>
        /// <param name="groupName">组的名称</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="controllers">返回的controller集合</param>
        /// <returns>是否存在</returns>
        bool GetControllers(string groupName, Predicate<IController> predicate, out IController[] controllers);
        /// <summary>
        /// 获得指定tag的controller数量；
        /// 若不存在tag，则返回负一；
        /// </summary>
        /// <param name="groupName">组的名称</param>
        /// <returns>数量</returns>
        int GetControllerGroupCount(string groupName);
        /// <summary>
        /// 通过 tag释放controller组；
        /// </summary>
        /// <param name="groupName">需要释放的组</param>
        void ReleaseControllerGroup(string groupName);
        /// <summary>
        /// 释放指定id的controller；
        /// </summary>
        /// <param name="controllerId">controller id</param>
        void ReleaseController(int controllerId);
        /// <summary>
        /// 释放指定名字的控制器；
        /// </summary>
        /// <param name="controllerName">controller name</param>
        void ReleaseController(string controllerName);
        /// <summary>
        /// 释放所有controller；
        /// </summary>
        void ReleaseAllControllers();
        /// <summary>
        /// 暂停组中的所有controller的轮询；
        /// </summary>
        /// <param name="groupName">被暂停的组的名称</param>
        void PauseControllerGroup(string groupName);
        /// <summary>
        /// 恢复组中的所有controller的轮询；
        /// </summary>
        /// <param name="groupName">被恢复的组的名称</param>
        void UnPauseControllerGroup(string groupName);
    }
}
