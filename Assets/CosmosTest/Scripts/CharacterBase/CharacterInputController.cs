using UnityEngine;
using System.Collections;
namespace Cosmos
{
    public abstract class CharacterInputController:MonoBehaviour
    {
        protected virtual void Awake()
        {
            Facade.Instance.InitInputManager();
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
        protected  Input.InputEventArgs inputEventArg;
    }
}