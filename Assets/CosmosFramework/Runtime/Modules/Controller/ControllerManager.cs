using System.Collections;
using System.Collections.Generic;
using System;

namespace Cosmos.Controller
{

    /// <summary>
    /// 控制器模块，主要针对于unity的生命周期；
    /// </summary>
    [Module]
    internal sealed class ControllerManager : Module // , IControllerManager
    {
        #region Properties
        Dictionary<Type, ControllerGroup> ctrlGroupDict;

        ControllerIndividuals ctrlIndividuals;

        Type controllerType = typeof(IController);

        Action tickRefresh;
        Action lateRefresh;
        Action fixedRefresh;

        public int ControllerTypeCount
        {
            get { return ctrlGroupDict.Count; }
        }
        #endregion

        #region Methods
        public override void OnInitialization()
        {
            ctrlGroupDict = new Dictionary<Type, ControllerGroup>();
            tickRefresh += ctrlIndividuals.TickRefresh;
            lateRefresh += ctrlIndividuals.LateRefresh;
            fixedRefresh += ctrlIndividuals.FixedRefresh;
        }
        public Controller<T> CreateController<T>(string controllerName, T owner, bool cluster)
            where T : class
        {
            var type = typeof(Controller<T>);
            Controller<T> controller = null;
            if (!cluster)
            {
                if (!ctrlIndividuals.HasController(type))
                {
                    var ctrl = Controller<T>.Create(controllerName, owner);
                    ctrl.OnInitialization();
                    ctrlIndividuals.AddController(type, ctrl);
                }
            }
            else
            {
                if (!ctrlGroupDict.TryGetValue(type, out var group))
                {
                    group = new ControllerGroup();
                    ctrlGroupDict.Add(type, group);

                    tickRefresh += group.TickRefresh;
                    lateRefresh += group.LateRefresh;
                    fixedRefresh += group.FixedRefresh;
                }
                group.AddController(controller);
            }
            return controller;
        }
        public Controller<T> CreateController<T>(T owner, bool individual)
    where T : class
        {
            return CreateController(string.Empty, owner, individual);
        }
        /// <summary>
        /// 是否存在指定类型，指定名称的Controller;
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>是否存在</returns>
        public bool HasController<T>(int controllerId)
           where T : class
        {
            if (ctrlGroupDict.TryGetValue(typeof(Controller<T>), out var ctrlPool))
            {
                return ctrlPool.HasController(controllerId);
            }
            return false;
        }
        /// <summary>
        /// 是否存在指定类型，指定名称的Controller;
        /// </summary>
        /// <param name="type">Controller类型</param>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>是否存在</returns>
        public bool HasController(Type type, int controllerId)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented from IController");
            if (!HasControllerType(type))
                return false;
            else
                return ctrlGroupDict[type].HasController(controllerId);
        }
        /// <summary>
        /// 是否存在指定类型的Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <returns>是否存在</returns>
        public bool HasControllerSet<T>()
           where T : class
        {
            return ctrlGroupDict.ContainsKey(typeof(Controller<T>));
        }
        public bool HasControllerType(Type type)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            return ctrlGroupDict.ContainsKey(type);
        }
        /// <summary>
        /// 获得目标类型的指定名称的Controller;
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="controllerName">Controller名称</param>
        /// <returns>Controller对象</returns>
        public IController GetElementController<T>(int controllerId)
           where T : class
        {
            return GetElementController(typeof(Controller<T>), controllerId);
        }
        public IController GetElementController(Type type, int controllerId)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            bool result = ctrlGroupDict.TryGetValue(type, out var ctrlGroup);
            if (!result)
                throw new ArgumentNullException("ControllerManager " + "Controller Type : " + type.FullName + " has not registered");
            return ctrlGroup.GetController(controllerId);
        }
        /// <summary>
        /// 条件查找Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="predicate">条件委托</param>
        /// <returns>Controller对象</returns>
        public IController GetElementController<T>(Predicate<IController> predicate)
           where T : class
        {
            var key = typeof(Controller<T>);
            bool result = ctrlGroupDict.TryGetValue(key, out var ctrlGroup);
            if (!result)
                throw new ArgumentNullException("ControllerManager " + "Controller Type : " + key.FullName + " has not registered");
            return ctrlGroup.GetController(predicate);
        }
        /// <summary>
        /// 条件查找所有符合的Controller
        /// </summary>
        /// <typeparam name="T">Controller类型</typeparam>
        /// <param name="predicate">条件委托</param>
        /// <returns>Controller数组</returns>
        public IController[] GetControllers<T>(Predicate<IController> predicate)
           where T : class
        {
            var key = typeof(Controller<T>);
            bool result = ctrlGroupDict.TryGetValue(key, out var ctrlGroup);
            if (!result)
                throw new ArgumentNullException("ControllerManager " + "Controller Type : " + key.FullName + " has not registered");
            return ctrlGroup.GetControllers(predicate);
        }
        public int GetControllerCount<T>()
           where T : class
        {
            return GetControllerCount(typeof(Controller<T>));
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
            if (ctrlGroupDict.ContainsKey(type))
                return ctrlGroupDict[type].ControllerCount;
            else
                throw new ArgumentNullException("ControllerManager-->>" + "Controller Type: " + type.ToString() + "  has not  registered");
        }
        public void ReleaseAllControllers()
        {
            foreach (var ctrlGroup in ctrlGroupDict)
            {
                ctrlGroup.Value.ClearControllers();
            }
            ctrlGroupDict.Clear();
            ctrlIndividuals.ClearControllers();
        }
        /// <summary>
        /// 清理并销毁指定类型的Controller类型;
        /// </summary>
        /// <typeparam name="T">Controller名称</typeparam>
        public void ReleaseControllerSet<T>()
           where T : class
        {
            ReleaseControllerSet(typeof(Controller<T>));
        }
        public void ReleaseControllerSet(Type type)
        {
            if (!controllerType.IsAssignableFrom(type))
                throw new NotImplementedException(type.ToString() + " : Not Implemented IController");
            if (ctrlGroupDict.TryRemove(type, out var ctrlGroup))
            {
                fixedRefresh -= ctrlGroup.FixedRefresh;
                tickRefresh -= ctrlGroup.TickRefresh;
                lateRefresh -= ctrlGroup.LateRefresh;
            }
            else
                throw new ArgumentNullException("ControllerManager-->>" + "Controller Type: " + type.ToString() + "  has not  registered");
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
    }
}