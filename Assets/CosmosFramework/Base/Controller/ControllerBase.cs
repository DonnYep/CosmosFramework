using UnityEngine;
using System.Collections;
namespace Cosmos
{
    /// <summary>
    /// 框架所有输入可控抽象基类
    /// 用作PlayerController，aminationController等用途
    /// </summary>
    public abstract class ControllerBase:MonoEventHandler
    {
        string controllerName;
        //返回controller所挂载的对象名称
        public string ControllerName { get { return controllerName; } }
        protected short monoPoolID;
        /// <summary>
        /// 非空虚函数，覆写时建议保留父类方法
        /// </summary>
        protected override void Awake()
        {
            controllerName = gameObject.name;
        }
        /// <summary>
        ///  非空虚函数
        ///  覆写时需要保留父类方法
        /// </summary>
        protected virtual void OnEnable()
        {
            Facade.AddMonoListener(UpdateHandler, UpdateType.Update, out monoPoolID);
        }
        /// <summary>
        ///  非空虚函数
        ///  覆写时需要保留父类方法
        /// </summary>
        protected virtual void OnDisable()
        {
            Facade.RemoveMonoListener(UpdateHandler, UpdateType.Update, monoPoolID);
        }
        /// <summary>
        /// 空虚函数
        /// </summary>
        protected virtual void UpdateHandler(){}
    }
}