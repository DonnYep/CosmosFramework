using System;
namespace Cosmos
{
    /// <summary>
    /// 简易的抽象有限状态机状态；
    /// </summary>
    /// <typeparam name="T">状态机持有者类型</typeparam>
    public abstract class SimpleFsmState<T>where T:class
    {
        /// <summary>
        /// 当状态被添加时触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnInit(SimpleFsm<T> fsm);
        /// <summary>
        /// 进入状态时触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnEnter(SimpleFsm<T> fsm);
        /// <summary>
        /// 当状态被执行时，轮询触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnUpdate(SimpleFsm<T> fsm);
        /// <summary>
        /// 当离开状态时触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnExit(SimpleFsm<T> fsm);
        /// <summary>
        /// 当状态被移除时触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnDestroy(SimpleFsm<T> fsm);
        /// <summary>
        /// 切换所属状态机的状态；
        /// </summary>
        /// <typeparam name="K">切换的状态类型</typeparam>
        /// <param name="fsm">所属的状态机</param>
        protected void ChangeState<K>(SimpleFsm<T> fsm)
            where K : SimpleFsmState<T>
        {
            fsm.ChangeState(typeof(K));
        }
        /// <summary>
        /// 切换所属状态机的状态；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        /// <param name="stateType">切换的状态类型</param>
        protected void ChangeState(SimpleFsm<T> fsm, Type stateType)
        {
            fsm.ChangeState(stateType);
        }
    }
}
