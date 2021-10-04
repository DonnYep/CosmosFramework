using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.Controller
{
    //================================================
    /*
    *1、控制器模块用于生成需要执行unity生命周期的对象。被创建的对象可以
    * 被unity的fixedupdate、lateupdate、update方法轮询；
    * 
    * 2、若控制器需要被轮询，则需要实现无参函数，并为需要被轮询的无参函数
    * 挂载 [TickRefresh] [FixedRefresh][LateRefresh] 特性；
    * 
    * 3、控制器被生成时拥有唯一Id，并可以被赋予组别；所有控制器都可以被
    * 命名，且名字可重复。若控制器有重名且无组别，则默认返回第一个被查询
    * 到的控制器；
    * 
    * 4、控制器需要由模块生成，并被模块释放；
    * 
    * 5、控制器模块已与unity解耦
    */
    //================================================
    [Module]
    internal sealed class ControllerManager : Module, IControllerManager
    {
        #region Properties
        /// <summary>
        /// key---标签名；value---组；
        /// </summary>
        Dictionary<string, ControllerGroup> controllerGroupDict;
        /// <summary>
        /// Controller在被创建时都会被分配一个唯一的ID；
        /// 此字典为唯一ID对应Controller本身；
        /// </summary>
        Dictionary<int, IController> controllerIdDict;

        Dictionary<int, Action> tickActionDict;
        Dictionary<int, Action> lateActionDict;
        Dictionary<int, Action> fixedActionDict;

        Action tickRefresh;
        Action lateRefresh;
        Action fixedRefresh;
        /// <summary>
        /// 控制器组的数量；
        /// </summary>
        public int ControllerGroupCount
        {
            get { return controllerGroupDict.Count; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 创建一个controller；
        /// </summary>
        /// <param name="controllerName">被创建controller的名字</param>
        /// <param name="handle">持有者对象</param>
        /// <returns>被创建的controller</returns>
        public IController CreateController(string controllerName, object handle)
        {
            return CreateController(controllerName, string.Empty, handle);
        }
        /// <summary>
        /// 创建一个具有组别属性的controller；
        /// </summary>
        /// <param name="controllerGroupName">controller所在的组的名称</param>
        /// <param name="controllerName">被创建controller的名字</param>
        /// <param name="handle">持有者对象</param>
        /// <returns>被创建的controller</returns>
        public IController CreateController(string controllerName, string controllerGroupName, object handle)
        {
            Utility.Text.IsStringValid(controllerName, "ControllerName is invalid !");
            if (!IsReferenceType(handle))
                throw new ArgumentException($"{handle} is not reference type");
            var controller = Controller.Create(controllerName, controllerGroupName, handle);
            controllerIdDict.Add(controller.Id, controller);
            AddRefresh(controller);
            if (!string.IsNullOrEmpty(controllerGroupName))
            {
                if (!controllerGroupDict.TryGetValue(controllerGroupName, out var group))
                {
                    group = new ControllerGroup();
                    controllerGroupDict.Add(controllerGroupName, group);
                }
                group.AddController(controller);
            }
            return controller;
        }
        /// <summary>
        /// 是否存在controller组别；
        /// </summary>
        /// <param name="controllerGroupName"></param>
        /// <returns></returns>
        public bool HasControllerGroup(string controllerGroupName)
        {
            return controllerGroupDict.ContainsKey(controllerGroupName);
        }
        /// <summary>
        /// 是否存在指定类型，指定名称的Controller;
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>是否存在</returns>
        public bool HasController(int controllerId)
        {
            return controllerIdDict.ContainsKey(controllerId);
        }
        /// <summary>
        /// 是否存在指定名字的controller；
        /// </summary>
        /// <param name="controllerName">controller name</param>
        /// <returns>是否存在</returns>
        public bool HasController(string controllerName)
        {
            Utility.Text.IsStringValid(controllerName, "ControllerName is invalid !");
            foreach (var controller in controllerIdDict)
            {
                if (controller.Value.ControllerName == controllerName)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 通过Id获取controller；
        /// </summary>
        /// <param name="controllerId">controller id</param>
        /// <param name="controller">返回的controller</param>
        /// <returns>是否存在</returns>
        public bool GetController(int controllerId, out IController controller)
        {
            return controllerIdDict.TryGetValue(controllerId, out controller);
        }
        /// <summary>
        /// 通过名称获取controller；
        /// </summary>
        /// <param name="controllerName">controller name</param>
        /// <param name="controller">返回的controller</param>
        /// <returns>是否存在</returns>
        public bool GetController(string controllerName, out IController controller)
        {
            Utility.Text.IsStringValid(controllerName, "ControllerName is invalid !");
            controller = null;
            foreach (var c in controllerIdDict)
            {
                if (c.Value.ControllerName == controllerName)
                {
                    controller = c.Value;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        ///获得指定tag下所有的controller；
        /// </summary>
        /// <param name="controllerGroupName">组的名称</param>
        /// <param name="controllers">返回的controller集合</param>
        /// <returns>是否存在</returns>
        public bool GetControllers(string controllerGroupName, out IController[] controllers)
        {
            Utility.Text.IsStringValid(controllerGroupName, "ControllerGroupName is invalid !");
            controllers = null;
            if (controllerGroupDict.TryGetValue(controllerGroupName, out var controllerGroup))
            {
                controllers = controllerGroup.GetAllControllers();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 条件查找所有符合的Controller
        /// </summary>
        /// <param name="controllerGroupName">组的名称</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="controllers">返回的controller集合</param>
        /// <returns>是否存在</returns>
        public bool GetControllers(string controllerGroupName, Predicate<IController> predicate, out IController[] controllers)
        {
            Utility.Text.IsStringValid(controllerGroupName, "ControllerGroupName is invalid !");
            controllers = null;
            bool result = controllerGroupDict.TryGetValue(controllerGroupName, out var controllerGroup);
            if (result)
                controllers = controllerGroup.GetControllers(predicate);
            return result;
        }
        /// <summary>
        /// 获得指定tag的controller数量；
        /// 若不存在tag，则返回负一；
        /// </summary>
        /// <param name="controllerGroupName">组的名称</param>
        /// <returns>数量</returns>
        public int GetControllerGroupCount(string controllerGroupName)
        {
            Utility.Text.IsStringValid(controllerGroupName, "ControllerGroupName is invalid !");
            if (controllerGroupDict.TryGetValue(controllerGroupName, out var controllerGroup))
                return controllerGroup.ControllerCount;
            return -1;
        }
        /// <summary>
        /// 通过 tag释放controller组；
        /// </summary>
        /// <param name="controllerGroupName">需要释放的组</param>
        public void ReleaseControllerGroup(string controllerGroupName)
        {
            Utility.Text.IsStringValid(controllerGroupName, "ControllerGroupName is invalid !");
            if (controllerGroupDict.Remove(controllerGroupName, out var controllerGroup))
            {
                var ids = controllerGroup.ControllerIds;
                controllerGroup.ClearControllers();
                var length = ids.Length;
                for (int i = 0; i < length; i++)
                {
                    controllerIdDict.Remove(ids[i], out var controller);
                    RemoveRefresh(ids[i]);
                    Controller.Release(controller);
                }
            }
        }
        /// <summary>
        /// 释放指定id的controller；
        /// </summary>
        /// <param name="controllerId">controller id</param>
        public void ReleaseController(int controllerId)
        {
            if (controllerIdDict.Remove(controllerId, out var controller))
            {
                var tag = controller.GroupName;
                if (!string.IsNullOrEmpty(tag))
                {
                    if (controllerGroupDict.TryGetValue(tag, out var controllerGroup))
                        controllerGroup.RemoveController(controllerId);
                    if (controllerGroup.ControllerCount <= 0)
                        controllerGroupDict.Remove(tag);
                }
                RemoveRefresh(controllerId);
                Controller.Release(controller);
            }
        }
        /// <summary>
        /// 释放controller；
        /// </summary>
        /// <param name="controller">需要释放的controller对象</param>
        public void ReleaseController(IController controller)
        {
            if (controller == null)
                throw new ArgumentNullException("Controller is invaild !");
            var controllerId = controller.Id;
            if (controllerIdDict.TryGetValue(controllerId, out var srcCtrl))
            {
                if (controller != srcCtrl)
                    throw new ArgumentException($"{controller}'s ptr is not equal !");//指针不一致
                controllerIdDict.Remove(controllerId);
                var tag = controller.GroupName;
                if (!string.IsNullOrEmpty(tag))
                {
                    if (controllerGroupDict.TryGetValue(tag, out var controllerGroup))
                        controllerGroup.RemoveController(controllerId);
                    if (controllerGroup.ControllerCount <= 0)
                        controllerGroupDict.Remove(tag);
                }
                RemoveRefresh(controllerId);
                Controller.Release(controller);
            }
        }
        /// <summary>
        /// 释放指定名字的控制器；
        /// </summary>
        /// <param name="controllerName">controller name</param>
        public void ReleaseController(string controllerName)
        {
            Utility.Text.IsStringValid(controllerName, "ControllerName is invalid !");
            IController controller = null;
            foreach (var c in controllerIdDict)
            {
                if (c.Value.ControllerName == controllerName)
                {
                    controller = c.Value;
                    return;
                }
            }
            if (controller != null)
                ReleaseController(controller.Id);
        }
        /// <summary>
        /// 释放所有controller；
        /// </summary>
        public void ReleaseAllControllers()
        {
            foreach (var controllerGroup in controllerGroupDict)
            {
                controllerGroup.Value.ClearControllers();
            }
            foreach (var controller in controllerIdDict)
            {
                Controller.Release(controller.Value);
            }
            controllerGroupDict.Clear();
            controllerIdDict.Clear();

            tickRefresh = null;
            fixedRefresh = null;
            lateRefresh = null;
        }
        /// <summary>
        /// 暂停组中的所有controller的轮询；
        /// </summary>
        /// <param name="controllerGroupName">被暂停的组的名称</param>
        public void PauseControllerGroup(string controllerGroupName)
        {
            Utility.Text.IsStringValid(controllerGroupName, "ControllerGroupName is invalid !");
            if (controllerGroupDict.TryGetValue(controllerGroupName, out var controllerGroup))
            {
                controllerGroup.PauseControllers();
            }
        }
        /// <summary>
        /// 恢复组中的所有controller的轮询；
        /// </summary>
        /// <param name="controllerGroupName">被恢复的组的名称</param>
        public void UnPauseControllerGroup(string controllerGroupName)
        {
            Utility.Text.IsStringValid(controllerGroupName, "ControllerGroupName is invalid !");
            if (controllerGroupDict.TryGetValue(controllerGroupName, out var controllerGroup))
            {
                controllerGroup.UnPauseControllers();
            }
        }
        protected override void OnInitialization()
        {
            controllerGroupDict = new Dictionary<string, ControllerGroup>();
            controllerIdDict = new Dictionary<int, IController>();

            tickActionDict = new Dictionary<int, Action>();
            lateActionDict = new Dictionary<int, Action>();
            fixedActionDict = new Dictionary<int, Action>();
        }
        void AddRefresh(IController controller)
        {
            TickRefreshAttribute.GetRefreshAction(controller, true, out var tickAction);
            LateRefreshAttribute.GetRefreshAction(controller, true, out var lateAction);
            FixedRefreshAttribute.GetRefreshAction(controller, true, out var fixedAction);
            if (tickAction != null)
            {
                tickActionDict.Add(controller.Id, tickAction);
                tickRefresh += tickAction;
            }
            if (lateAction != null)
            {
                lateActionDict.Add(controller.Id, lateAction);
                lateRefresh += lateAction;
            }
            if (fixedAction != null)
            {
                fixedActionDict.Add(controller.Id, fixedAction);
                fixedRefresh += fixedAction;
            }
        }
        void RemoveRefresh(int controllerId)
        {
            if (tickActionDict.Remove(controllerId, out var tickAction))
                tickRefresh -= tickAction;
            if (lateActionDict.Remove(controllerId, out var lateAction))
                lateRefresh += lateAction;
            if (fixedActionDict.Remove(controllerId, out var fixedAction))
                fixedRefresh += fixedAction;
        }
        [TickRefresh]
        void TickRefresh()
        {
            if (IsPause)
                return;
            tickRefresh?.Invoke();
        }
        [LateRefresh]
        void LateRefresh()
        {
            if (IsPause)
                return;
            lateRefresh?.Invoke();
        }
        [FixedRefresh]
        void FixedRefresh()
        {
            if (IsPause)
                return;
            fixedRefresh?.Invoke();
        }
        #endregion
        bool IsReferenceType(object handle)
        {
            var type = handle.GetType();
            return type.IsClass || type.IsInterface;
        }
    }
}