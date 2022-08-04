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
    internal sealed partial class ControllerManager : Module, IControllerManager
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
        ///<inheritdoc/>
        public int ControllerGroupCount
        {
            get { return controllerGroupDict.Count; }
        }
        #endregion

        #region Methods
        ///<inheritdoc/>
        public IController CreateController(string controllerName, object handle)
        {
            return CreateController(controllerName, string.Empty, handle);
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public bool HasControllerGroup(string controllerGroupName)
        {
            return controllerGroupDict.ContainsKey(controllerGroupName);
        }
        ///<inheritdoc/>
        public bool HasController(int controllerId)
        {
            return controllerIdDict.ContainsKey(controllerId);
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public bool GetController(int controllerId, out IController controller)
        {
            return controllerIdDict.TryGetValue(controllerId, out controller);
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public bool GetControllers(string controllerGroupName, Predicate<IController> condition, out IController[] controllers)
        {
            Utility.Text.IsStringValid(controllerGroupName, "ControllerGroupName is invalid !");
            controllers = null;
            bool result = controllerGroupDict.TryGetValue(controllerGroupName, out var controllerGroup);
            if (result)
                controllers = controllerGroup.GetControllers(condition);
            return result;
        }
        ///<inheritdoc/>
        public int GetControllerGroupCount(string controllerGroupName)
        {
            Utility.Text.IsStringValid(controllerGroupName, "ControllerGroupName is invalid !");
            if (controllerGroupDict.TryGetValue(controllerGroupName, out var controllerGroup))
                return controllerGroup.ControllerCount;
            return -1;
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public void PauseControllerGroup(string controllerGroupName)
        {
            Utility.Text.IsStringValid(controllerGroupName, "ControllerGroupName is invalid !");
            if (controllerGroupDict.TryGetValue(controllerGroupName, out var controllerGroup))
            {
                controllerGroup.PauseControllers();
            }
        }
        ///<inheritdoc/>
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