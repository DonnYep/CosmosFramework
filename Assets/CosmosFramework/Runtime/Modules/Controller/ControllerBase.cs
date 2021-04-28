using UnityEngine;
using System.Collections;
using System;

namespace Cosmos
{
    /// <summary>
    /// 框架所有输入可控抽象基类
    /// 用作PlayerController，aminationController等用途
    /// </summary>
    public abstract class ControllerBase<T> : MonoBehaviour, IController
        where T : ControllerBase<T>
    {
        public virtual Type ControllerType { get { return controllerType; } }
        public virtual string ControllerName
        {
            get
            {
                if (string.IsNullOrEmpty(controllerName))
                    return controllerType.Name;
                else
                    return controllerName;
            }
            set { controllerName = value; }
        }
        protected string controllerName;
        public bool IsPause { get; protected set; }
        IControllerManager ControllerManager { get { return GameManager.GetModule<IControllerManager>(); } }
        Type controllerType = typeof(T);
        /// <summary>
        /// 空虚函数;
        /// 逻辑激活；
        /// 对象被激活;
        /// </summary>
        public virtual void OnActive() { }
        /// <summary>
        /// 空虚函数；
        /// 逻辑失活；
        /// 对象被激活；
        /// </summary>
        public virtual void OnDeactive() { }
        public void OnPause() { IsPause = true; }
        public void OnUnPause() { IsPause = false; }
        public void OnRefresh() { if (IsPause) return; RefreshHandler(); }
        /// <summary>
        /// 空虚函数；
        /// </summary>
        protected virtual void RefreshHandler() { }
        /// <summary>
        ///  非空虚函数；
        /// 覆写时需要保留基类方法； 
        /// </summary>
        protected  void Awake()
        {
            ControllerManager.RegisterController(ControllerType, this);
            OnInitialization();
        }
        /// <summary>
        /// 初始化；
        /// 空虚函数；
        /// </summary>
        protected virtual void OnInitialization(){}
        /// <summary>
        /// 终结；
        /// 空虚函数；
        /// </summary>
        protected virtual void OnTermination(){}
        protected void OnDestroy()
        {
            try
            {
                ControllerManager.DeregisterController(ControllerType, this);
                OnTermination();
            }
            catch { }
        }


    }
}