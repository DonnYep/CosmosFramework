using System;
namespace Cosmos.Procedure
{
    //================================================
    /*
     * 1、流程管理模块。
     * 
     * 2、流程节点的生命周期按照顺序依次为:OnInit>OnEnter>OnUpdate
     * >OnExit>OnDestroy。
     * 
     * 3、OnInit函数在ProcedureNode被添加到ProcedureManager时触发。
     * 
     * 4、OnEnter函数在进入ProcedureNode状态时触发。
     * 
     * 5、OnUpdate函数在ProcedureNode状态中轮询触发。
     * 
     * 6、OnExit函数在离开ProcedureNode状态时触发。
     * 
     * 7、OnDestroy函数在ProcedureNode被从ProcedureManager移除时触发。
     */
    //================================================
    [Module]
    internal class ProcedureManager : Module, IProcedureManager
    {
        SimpleFsm<IProcedureManager> procedureFsm;
        Type procedureStateType = typeof(ProcedureState);
        /// <inheritdoc/>
        public int ProcedureCount { get { return procedureFsm.StateCount; } }
        /// <inheritdoc/>
        public ProcedureState CurrentProcedureNode { get { return procedureFsm.CurrentState as ProcedureState; } }
        /// <inheritdoc/>
        public void AddProcedures(params ProcedureState[] nodes)
        {
            procedureFsm.AddStates(nodes);
        }
        /// <inheritdoc/>
        public void RunProcedure<T>() where T : ProcedureState
        {
            RunProcedure(typeof(T));
        }
        /// <inheritdoc/>
        public void RunProcedure(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type is invalid !");
            if (procedureStateType.IsAssignableFrom(type))
                throw new NotImplementedException($"Type:{type} is not inherit form ProcedureState");
            procedureFsm.SetDefaultState(type);
        }
        /// <inheritdoc/>
        public bool HasProcedure<T>() where T : ProcedureState
        {
            return HasProcedure(typeof(T));
        }
        /// <inheritdoc/>
        public bool HasProcedure(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type is invalid !");
            if (procedureStateType.IsAssignableFrom(type))
                throw new NotImplementedException($"Type:{type} is not inherit form ProcedureState");
            return procedureFsm.HasState(type);
        }
        /// <inheritdoc/>
        public bool PeekProcedure(Type type, out ProcedureState node)
        {
            if (type == null)
                throw new ArgumentNullException("Type is invalid !");
            if (procedureStateType.IsAssignableFrom(type))
                throw new NotImplementedException($"Type:{type} is not inherit form ProcedureState");
            node = null;
            var rst = procedureFsm.PeekState(type, out var state);
            if (rst)
            {
                node = state as ProcedureState;
            }
            return rst;
        }
        /// <inheritdoc/>
        public bool PeekProcedure<T>(out ProcedureState node) where T : ProcedureState
        {
            var type = typeof(T);
            return PeekProcedure(type, out node);
        }
        /// <inheritdoc/>
        public void RemoveProcedures(params Type[] types)
        {
            var length = types.Length;
            for (int i = 0; i < length; i++)
            {
                RemoveProcedure(types[i]);
            }
        }
        /// <inheritdoc/>
        public bool RemoveProcedure<T>() where T : ProcedureState
        {
            return RemoveProcedure(typeof(T));
        }
        /// <inheritdoc/>
        public bool RemoveProcedure(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type is invalid !");
            if (procedureStateType.IsAssignableFrom(type))
                throw new NotImplementedException($"Type:{type} is not inherit form ProcedureState");
            return procedureFsm.RemoveState(type);
        }
        protected override void OnInitialization()
        {
            procedureFsm = new SimpleFsm<IProcedureManager>(this);
        }
        protected override void OnTermination()
        {
            procedureFsm.ClearAllState();
        }
        [TickRefresh]
        void TickRefresh()
        {
            procedureFsm.Refresh();
        }
    }
}
