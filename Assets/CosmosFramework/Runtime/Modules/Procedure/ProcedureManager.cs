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
        ProcedureProcessor procedureProcessor;
        Type procedureNodeType = typeof(ProcedureNode);
        Action<ProcedureNodeAddedEventArgs> onProcedureNodeAdd;
        Action<PorcedureNodeRemovedEventArgs> onProcedureNodeRemove;
        Action<ProcedureNodeChangedEventArgs> onProcedureNodeChange;
        /// <inheritdoc/>
        public int ProcedureNodeCount { get { return procedureProcessor.NodeCount; } }
        /// <inheritdoc/>
        public ProcedureNode CurrentProcedureNode
        {
            get { return procedureProcessor.CurrentNode; }
        }
        /// <inheritdoc/>
        public event Action<ProcedureNodeAddedEventArgs> OnProcedureNodeAdd
        {
            add { onProcedureNodeAdd += value; }
            remove { onProcedureNodeAdd -= value; }
        }
        /// <inheritdoc/>
        public event Action<PorcedureNodeRemovedEventArgs> OnProcedureNodeRemove
        {
            add { onProcedureNodeRemove += value; }
            remove { onProcedureNodeRemove -= value; }
        }
        /// <inheritdoc/>
        public event Action<ProcedureNodeChangedEventArgs> OnProcedureNodeChange
        {
            add { onProcedureNodeChange += value; }
            remove { onProcedureNodeChange -= value; }
        }
        /// <inheritdoc/>
        public void AddProcedureNodes(params ProcedureNode[] nodes)
        {
            procedureProcessor.AddNodes(nodes);
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
            procedureProcessor.ChangeNode(type);
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
            return procedureProcessor.HasNode(type);
        }
        /// <inheritdoc/>
        public bool PeekProcedureNode(Type type, out ProcedureNode node)
        {
            if (type == null)
                throw new ArgumentNullException("Type is invalid !");
            if (!procedureNodeType.IsAssignableFrom(type))
                throw new NotImplementedException($"Type:{type} is not inherit form ProcedureState");
            node = null;
            var rst = procedureProcessor.PeekNode(type, out var _node);
            if (rst)
            {
                node = _node as ProcedureNode;
            }
            return rst;
        }
        /// <inheritdoc/>
        public bool PeekProcedureNode<T>(out T node) where T : ProcedureNode
        {
            node = default;
            var type = typeof(T);
            var hasNode = PeekProcedureNode(type, out var procedureNode);
            if (hasNode)
                node = (T)procedureNode;
            return hasNode;
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
            return procedureProcessor.RemoveNode(type);
        }
        protected override void OnInitialization()
        {
            procedureProcessor = new ProcedureProcessor(this);
            procedureProcessor.OnProcedureNodeAdd += ProcedureNodeAddCallback;
            procedureProcessor.OnProcedureNodeRemove += ProcedureNodeRemoveCallback;
            procedureProcessor.OnProcedureNodeChange += ProcedureNodeChangedCallback;
        }
        protected override void OnTermination()
        {
            procedureProcessor.ClearAllNode();
            procedureProcessor.OnProcedureNodeAdd -= ProcedureNodeAddCallback;
            procedureProcessor.OnProcedureNodeRemove -= ProcedureNodeRemoveCallback;
            procedureProcessor.OnProcedureNodeChange -= ProcedureNodeChangedCallback;
        }
        [TickRefresh]
        void TickRefresh()
        {
            procedureProcessor.Refresh();
        }
        void ProcedureNodeAddCallback(Type type)
        {
            var eventArgs = ProcedureNodeAddedEventArgs.Create(type);
            onProcedureNodeAdd?.Invoke(eventArgs);
            ProcedureNodeAddedEventArgs.Release(eventArgs);
        }
        void ProcedureNodeRemoveCallback(Type type)
        {
            var eventArgs = PorcedureNodeRemovedEventArgs.Create(type);
            onProcedureNodeRemove?.Invoke(eventArgs);
            PorcedureNodeRemovedEventArgs.Release(eventArgs);
        }
        void ProcedureNodeChangedCallback(Type exitedNodeType, Type enteredNodeType)
        {
            var eventArgs = ProcedureNodeChangedEventArgs.Create(exitedNodeType, enteredNodeType);
            onProcedureNodeChange?.Invoke(eventArgs);
            ProcedureNodeChangedEventArgs.Release(eventArgs);
        }
    }
}
