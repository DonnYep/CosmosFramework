using System;
namespace Cosmos.Procedure
{
    public abstract class ProcedureNodeBase<T> where T : class
    {
        /// <summary>
        /// 当状态被添加时触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnInit(ProcedureProcessor<T> fsm);
        /// <summary>
        /// 进入状态时触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnEnter(ProcedureProcessor<T> fsm);
        /// <summary>
        /// 当状态被执行时，轮询触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnUpdate(ProcedureProcessor<T> fsm);
        /// <summary>
        /// 当离开状态时触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnExit(ProcedureProcessor<T> fsm);
        /// <summary>
        /// 当状态被移除时触发；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        public abstract void OnDestroy(ProcedureProcessor<T> fsm);
        /// <summary>
        /// 切换所属状态机的状态；
        /// </summary>
        /// <typeparam name="K">切换的状态类型</typeparam>
        /// <param name="fsm">所属的状态机</param>
        protected void ChangeState<K>(ProcedureProcessor<T> fsm)
            where K : ProcedureNodeBase<T>
        {
            fsm.ChangeNode(typeof(K));
        }
        /// <summary>
        /// 切换所属状态机的状态；
        /// </summary>
        /// <param name="fsm">所属的状态机</param>
        /// <param name="stateType">切换的状态类型</param>
        protected void ChangeState(ProcedureProcessor<T> fsm, Type stateType)
        {
            fsm.ChangeNode(stateType);
        }
    }
}
