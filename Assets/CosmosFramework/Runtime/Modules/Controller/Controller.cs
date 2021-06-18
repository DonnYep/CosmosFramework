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
    internal class Controller<T> : IController
        where T : class
    {
        static int controllerIndex = 0;
        T owner;
        Action tickRefreshHandler;
        Action lateRefreshHandler;
        Action fixedRefreshHandler;
        Type ownerType;
        string controllerName;
        public object Handle { get { return owner; } }
        public Type HandleType { get { return ownerType; } }
        public string ControllerName
        {
            get
            {
                if (string.IsNullOrEmpty(controllerName))
                    return ownerType.Name;
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
            owner = null;
            Pause = false;
            ownerType = null;
            controllerName = null;
            tickRefreshHandler = null;
            lateRefreshHandler = null;
            fixedRefreshHandler = null;
        }
        internal static Controller<T> Create(string groupName, string controllerName, T owner)
        {
            var controller = ReferencePool.Accquire<Controller<T>>();
            controller.owner = owner;
            controller.controllerName = controllerName;
            controller.ownerType = typeof(T);
            controller.GroupName = groupName;
            TickRefreshAttribute.GetRefreshAction(owner, true, out var tickAction);
            LateRefreshAttribute.GetRefreshAction(owner, true, out var lateAction);
            FixedRefreshAttribute.GetRefreshAction(owner, true, out var fixedAction);
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