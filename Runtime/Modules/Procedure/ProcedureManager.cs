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
        ProcedureProcessor<IProcedureManager> procedureFsm;
        Type procedureNodeType = typeof(ProcedureNode);
        Action<ProcedureNodeChangedEventArgs> procedureNodeChanged;
        /// <inheritdoc/>
        public int ProcedureNodeCount { get { return procedureFsm.NodeCount; } }
        /// <inheritdoc/>
        public ProcedureNode CurrentProcedureNode
        {
            get { return procedureFsm.CurrentNode as ProcedureNode; }
        }
        /// <inheritdoc/>
        public event Action<ProcedureNodeChangedEventArgs> ProcedureNodeChanged
        {
            add { procedureNodeChanged += value; }
            remove { procedureNodeChanged -= value; }
        }
        /// <inheritdoc/>
        public void AddProcedureNodes(params ProcedureNode[] nodes)
        {
            procedureFsm.AddNodes(nodes);
        }
        /// <inheritdoc/>
        public void RunProcedureNode<T>() where T : ProcedureNode
        {
            RunProcedureNode(typeof(T));
        }
        /// <inheritdoc/>
        public void RunProcedureNode(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type is invalid !");
            if (!procedureNodeType.IsAssignableFrom(type))
                throw new NotImplementedException($"Type:{type} is not inherit form ProcedureState");
            procedureFsm.ChangeNode(type);
        }
        /// <inheritdoc/>
        public bool HasProcedureNode<T>() where T : ProcedureNode
        {
            return HasProcedureNode(typeof(T));
        }
        /// <inheritdoc/>
        public bool HasProcedureNode(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type is invalid !");
            if (!procedureNodeType.IsAssignableFrom(type))
                throw new NotImplementedException($"Type:{type} is not inherit form ProcedureState");
            return procedureFsm.HasNode(type);
        }
        /// <inheritdoc/>
        public bool PeekProcedureNode(Type type, out ProcedureNode node)
        {
            if (type == null)
                throw new ArgumentNullException("Type is invalid !");
            if (!procedureNodeType.IsAssignableFrom(type))
                throw new NotImplementedException($"Type:{type} is not inherit form ProcedureState");
            node = null;
            var rst = procedureFsm.PeekNode(type, out var _node);
            if (rst)
            {
                node = _node as ProcedureNode;
            }
            return rst;
        }
        /// <inheritdoc/>
        public bool PeekProcedureNode<T>(out ProcedureNode node) where T : ProcedureNode
        {
            var type = typeof(T);
            return PeekProcedureNode(type, out node);
        }
        /// <inheritdoc/>
        public void RemoveProcedureNodes(params Type[] types)
        {
            var length = types.Length;
            for (int i = 0; i < length; i++)
            {
                RemoveProcedureNode(types[i]);
            }
        }
        /// <inheritdoc/>
        public bool RemoveProcedureNode<T>() where T : ProcedureNode
        {
            return RemoveProcedureNode(typeof(T));
        }
        /// <inheritdoc/>
        public bool RemoveProcedureNode(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Type is invalid !");
            if (!procedureNodeType.IsAssignableFrom(type))
                throw new NotImplementedException($"Type:{type} is not inherit form ProcedureState");
            return procedureFsm.RemoveNode(type);
        }
        protected override void OnInitialization()
        {
            procedureFsm = new ProcedureProcessor<IProcedureManager>(this);
            procedureFsm.ProcedureNodeChanged += ProcedureNodeChangedCallback;
        }
        protected override void OnTermination()
        {
            procedureFsm.ClearAllNode();
            procedureFsm.ProcedureNodeChanged -= ProcedureNodeChangedCallback;
        }
        [TickRefresh]
        void TickRefresh()
        {
            procedureFsm.Refresh();
        }
        void ProcedureNodeChangedCallback(Type exitedNodeType, Type enteredNodeType)
        {
            var changedEventArgs = ProcedureNodeChangedEventArgs.Create(exitedNodeType, enteredNodeType);
            procedureNodeChanged?.Invoke(changedEventArgs);
            ProcedureNodeChangedEventArgs.Release(changedEventArgs);
        }
    }
}
