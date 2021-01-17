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
        public virtual string ControllerName { get { return typeof(T).Name; } protected set { } }
        public virtual Type ControllerType { get { return typeof(T); } }
        public bool IsPause { get; set; }
        /// <summary>
        /// 空虚函数;
        /// 逻辑激活；
        /// 对象被激活;
        /// </summary>
        public virtual void OnActive() { }
        /// <summary>
        /// 空虚函数；
        /// 逻辑失活；
        /// 对象被失活；
        /// </summary>
        public virtual void OnDeactive() { }
        public void OnPause() { IsPause = true; }
        public void OnUnPause() { IsPause = false; }
        public void OnRefresh() { if (IsPause) return; RefreshHandler(); }
        /// <summary>
        ///  空虚函数
        ///  覆写时可以根据需要保留父类方法；
        /// </summary>
        protected override void Awake() { }
        /// <summary>
        ///  空虚函数
        ///  覆写时需要保留父类方法
        /// </summary>
        protected virtual void RefreshHandler() { }
        /// <summary>
        ///  非空虚函数
        ///  覆写时需要保留父类方法
        /// </summary>
        protected virtual  void OnEnable(){
            Facade.RegisterController(ControllerType, this);
        }
        /// <summary>
        ///  非空虚函数
        ///  覆写时需要保留父类方法
        /// </summary>
        protected virtual void OnDisable() { 
            Facade.DeregisterController(ControllerType, this);
        }
    }
}