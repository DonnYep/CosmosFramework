using UnityEngine;
using System.Collections;
namespace Cosmos
{
    /// <summary>
    /// 框架所有输入可控抽象基类
    /// 用作PlayerController，aminationController等用途
    /// </summary>
    public abstract class CFController:MonoEventHandler
    {
        string controllerName;
        //返回controller所挂载的对象名称
        public string ControllerName { get { return controllerName; } }
        protected override void OnInitialization()
        {
            controllerName = gameObject.name;
            Facade.Instance.RegisterModule(CFModules.INPUT);
            AddDefaultEventListener(InputEventParam.INPUT_INPUT);
        }
        protected override void OnTermination()
        {
            RemoveDefaultEventListener(InputEventParam.INPUT_INPUT);
        }
        /// <summary>
        /// 事件处理者
        /// </summary>
        protected InputEventArgs inputEventArgs;
        protected ControllerEventArgs controllerEventArgs;
    }
}