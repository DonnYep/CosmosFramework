using UnityEngine;
using System.Collections;
namespace Cosmos
{
    /// <summary>
    /// 框架所有输入可控抽象基类
    /// 用作PlayerController，aminationController等用途
    /// </summary>
    public abstract class CFController:MonoBehaviour
    {
        string controllerName;
        //返回controller所挂载的对象名称
        public string ControllerName { get { return controllerName; } }
        protected virtual void Awake()
        {
            controllerName = gameObject.name;
            Facade.Instance.RegisterModule(CFModules.INPUT);
            Facade.Instance.AddEventListener(InputEventParam.INPUTEVENT_INPUT, Handler);
            OnInitialization();
        }
        protected virtual  void OnDestroy()
        {
            Facade.Instance.RemoveEventListener(InputEventParam.INPUTEVENT_INPUT, Handler);
            OnTermination();
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        protected abstract void OnInitialization();
        /// <summary>
        /// 终止组件
        /// </summary>
        protected virtual void OnTermination() { }
        /// <summary>
        /// 事件处理者
        /// </summary>
        protected abstract void Handler(object sender, GameEventArgs arg);
        protected InputEventArgs inputEventArgs;
        protected ControllerEventArgs controllerEventArgs;
    }
}