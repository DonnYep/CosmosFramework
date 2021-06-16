using UnityEngine;
using System.Collections;
using System;

namespace Cosmos
{
    //================================================
    //1、WebRequest用于加载AssetBundle资源。资源状态可以是Remote的，
    // 也可以是Local下persistentDataPath的；
    //2、内置已经实现了一个默认的WebRequest帮助类对象；模块初始化时会
    // 自动加载并将默认的helper设置为此模块的默认加载helper；
    //3、helper可以自行实现并且切换，切换模块的状态是异步的，内部由
    // FutureTask进行异步状态的检测。
    //================================================
    /// <summary>
    /// 框架所有输入可控抽象基类
    /// 特性TickRefreshAttribute支持类；需要轮询的无参方法在挂载TickRefreshAttribute后即可获得Update轮询；
    /// <see cref="TickRefreshAttribute"/>
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