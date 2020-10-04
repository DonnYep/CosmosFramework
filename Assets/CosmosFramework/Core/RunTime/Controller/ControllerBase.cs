using UnityEngine;
using System.Collections;
namespace Cosmos
{
    /// <summary>
    /// 框架所有输入可控抽象基类
    /// 用作PlayerController，aminationController等用途
    /// </summary>
    public abstract class ControllerBase : MonoEventHandler, IController
    {
        protected string controllerName;
        //返回controller所挂载的对象名称
        public virtual string ControllerName { get { return controllerName; } protected set { controllerName = value; } }
        public bool IsPause { get; protected set; }
        protected short monoPoolID;
        /// <summary>
        /// 空虚函数
        /// 对象被激活
        /// </summary>
        public virtual void OnActive() { }
        /// <summary>
        /// 空虚函数
        /// 对象被激活
        /// </summary>
        public virtual void OnDeactive() { }
        public void OnPause() { IsPause = true; }
        public void OnUnPause() { IsPause = false; }
        /// <summary>
        /// 非空虚函数
        /// 调用了UpdateHandler()
        /// </summary>
        public virtual void OnRefresh() { UpdateHandler(); }
        /// <summary>
        ///  非空虚函数
        ///  覆写时需要保留父类方法
        /// </summary>
        protected override void Awake() 
        {
            controllerName = this.GetType().Name;
        }
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void UpdateHandler() { }
        /// <summary>
        /// 非空虚函数
        /// </summary>
        protected virtual  void OnEnable(){
            Facade.RegisterController(this);
        }
        /// <summary>
        /// 非空虚函数
        /// </summary>
        protected virtual void OnDisable() {
            Facade.DeregisterController(this);
        }
    }
}