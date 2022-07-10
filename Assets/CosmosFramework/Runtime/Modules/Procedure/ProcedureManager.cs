using System;
namespace Cosmos.Procedure
{
    //================================================
    /*
     * 1、流程管理模块
     */
    //================================================
    internal class ProcedureManager : Module,IProcedureManager
    {
        SimpleFsm<IProcedureManager> procedureFsm;

        /// <inheritdoc/>
        public int ProcedureNodeCount { get; }
        /// <inheritdoc/>
        public ProcedureNode CurrentProcedureNode { get; private set; }
        /// <inheritdoc/>
        public void AddProcedures(params ProcedureNode[]  nodes)
        {

        }
        /// <inheritdoc/>
        public void RunProcedure<T>() where T : ProcedureNode
        {
            RunProcedure(typeof(T));
        }
        /// <inheritdoc/>
        public void RunProcedure(Type type)
        {
            procedureFsm.SetDefaultState(type);
        }
        /// <inheritdoc/>
        public bool HasProcedure<T>() where T : ProcedureNode
        {
            return HasProcedure(typeof(T));
        }
        /// <inheritdoc/>
        public  bool HasProcedure(Type type)
        {
            return procedureFsm.HasState(type);
        }
        /// <inheritdoc/>
        public bool PeekProcedure(Type type, out ProcedureNode node)
        {
            node = null;
            var rst= procedureFsm.PeekState(type, out var state);
            if (rst)
            {
                node = state as ProcedureNode;
            }
            return rst;
        }
        /// <inheritdoc/>
        public bool PeekProcedure<T>(out ProcedureNode node) where T : ProcedureNode
        {
            var type = typeof(T);
            return PeekProcedure(type, out node);
        }
        /// <inheritdoc/>
        public void RemoveProcedures(params Type[] types)
        {

        }
        protected override void OnInitialization()
        {
            procedureFsm = new SimpleFsm<IProcedureManager>(this);
        }
    }
}
