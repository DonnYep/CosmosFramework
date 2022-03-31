using System;
namespace Cosmos
{
    /// <summary>
    /// 简易的抽象有限状态机状态；
    /// </summary>
    public abstract class SimpleFsmState<T>where T:class
    {
        public abstract string StateName { get; }
        public virtual void OnInit(SimpleFsm<T> fsm) { }
        public virtual void OnEnter(SimpleFsm<T> fsm) { }
        public virtual void OnExit(SimpleFsm<T> fsm) { }
        public abstract void OnUpdate(SimpleFsm<T> fsm);
        public virtual void OnDestroy(SimpleFsm<T> fsm) { }

        protected void ChangeState<K>(SimpleFsm<T> fsm)
            where K : SimpleFsmState<T>
        {
            fsm.ChangeState(typeof(T));
        }
        protected void ChangeState(SimpleFsm<T> fsm, Type stateType)
        {
            fsm.ChangeState(stateType);
        }
    }
}
