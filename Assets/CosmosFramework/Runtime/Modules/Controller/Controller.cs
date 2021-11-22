using UnityEngine;
using System.Collections;
using System;

namespace Cosmos.Controller
{
    /// <summary>
    /// 框架所有输入可控抽象基类
    /// 无参轮询特性支持；
    /// <see cref="TickRefreshAttribute"/>
    /// <see cref="LateRefreshAttribute"/>
    /// <see cref="FixedRefreshAttribute"/>
    /// </summary>
    internal class Controller : IController
    {
        static int controllerIndex = 0;

        Action tickRefreshHandler;
        Action lateRefreshHandler;
        Action fixedRefreshHandler;
        object handle;
        Type handleType;
        string controllerName;
        public object Handle { get { return handle; } }
        public Type HandleType { get { return handleType; } }
        public string ControllerName
        {
            get
            {
                if (string.IsNullOrEmpty(controllerName))
                    return handleType.Name;
                else
                    return controllerName;
            }
        }
        public bool Pause { get; set; }
        public int Id { get; private set; }
        public string GroupName { get; private set; }
        public void Release()
        {
            Id = 0;
            handle = null;
            Pause = false;
            handleType = null;
            controllerName = null;
            tickRefreshHandler = null;
            lateRefreshHandler = null;
            fixedRefreshHandler = null;
        }
        internal static void Release(IController controller)
        {
            ReferencePool.Release(controller);
        }
        internal static IController Create(string controllerName, string groupName, object handle)
        {
            var controller = ReferencePool.Acquire<Controller>();
            controller.handle = handle;
            controller.controllerName = controllerName;
            controller.handleType = handle.GetType();
            controller.GroupName = groupName;
            TickRefreshAttribute.GetRefreshAction(handle, true, out var tickAction);
            LateRefreshAttribute.GetRefreshAction(handle, true, out var lateAction);
            FixedRefreshAttribute.GetRefreshAction(handle, true, out var fixedAction);
            controller.tickRefreshHandler = tickAction;
            controller.lateRefreshHandler = lateAction;
            controller.fixedRefreshHandler = fixedAction;
            controller.Id = controllerIndex++;
            return controller;
        }
        [FixedRefresh]
        void FixesRefrsh()
        {
            if (Pause)
                return;
            fixedRefreshHandler?.Invoke();
        }
        [LateRefresh]
        void LateRefrsh()
        {
            if (Pause)
                return;
            lateRefreshHandler?.Invoke();
        }
        [TickRefresh]
        void TickRefrsh()
        {
            if (Pause)
                return;
            tickRefreshHandler?.Invoke();
        }
    }
}