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
        protected virtual void Awake()
        {
            Facade.Instance.AddEventListener(ApplicationConst._InputEventKey, Handler);
            OnInitialization();
        }
        protected virtual  void OnDestroy()
        {
            Facade.Instance.RemoveEventListener(ApplicationConst._InputEventKey, Handler);
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
        protected abstract void Handler(object sender, GameEventArgs arg);
        protected InputEventArgs inputEventArg;
    }
}