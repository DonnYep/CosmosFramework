using UnityEngine;
using System.Collections;
using System;

namespace Cosmos
{
    /// <summary>
    /// 框架所有输入可控抽象基类
    /// 用作PlayerController，aminationController等用途
    /// </summary>
    public abstract class ControllerBase<T> : MonoEventHandler, IController
        where T: ControllerBase<T>
    {
        public Type ControllerType { get { return controllerType; } }
        public virtual string ControllerName { get; protected set; }
        public bool IsPause { get; protected set; }
        Type controllerType = typeof(T);
        /// <summary>
        /// 空虚函数;
        /// 逻辑激活；
        /// 对象被激活;
        /// </summary>
        public virtual void OnActive() {}
        /// <summary>
        /// 空虚函数；
        /// 逻辑失活；
        /// 对象被激活；
        /// </summary>
        public virtual void OnDeactive() {}
        public void OnPause() { IsPause = true; }
        public void OnUnPause() { IsPause = false; }
        public void OnRefresh() { if (IsPause) return;RefreshHandler(); }
        /// <summary>
        /// 空虚函数；
        /// </summary>
        protected virtual void RefreshHandler() { }
        /// <summary>
        ///  非空虚函数
        ///  覆写时需要保留父类方法
        /// </summary>
        protected override void Awake()
        {
            ControllerName = ControllerType.Name;
        }
        /// <summary>
        /// 非空虚函数
        /// </summary>
        protected virtual void OnEnable()
        {
            GameManager.GetModule<IControllerManager>().RegisterController(ControllerType, this);
        }
        /// <summary>
        /// 非空虚函数
        /// </summary>
        protected virtual void OnDisable()
        {
            GameManager.GetModule<IControllerManager>().DeregisterController(ControllerType, this);
        }
    }
}