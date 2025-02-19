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
    public interface IProcedureManager : IModuleManager
    {
        /// <summary>
        /// 流程节点数量。
        /// </summary>
        int ProcedureNodeCount { get; }
        /// <summary>
        /// 当前节点。
        /// </summary>
        ProcedureNode CurrentProcedureNode { get; }
        /// <summary>
        /// Procedure node add event
        /// </summary>
        event Action<ProcedureNodeAddedEventArgs> OnProcedureNodeAdd;
        /// <summary>
        /// Procedure node remove event
        /// </summary>
        event Action<PorcedureNodeRemovedEventArgs> OnProcedureNodeRemove;
        /// <summary>
        /// Procedure node change event
        /// </summary>
        event Action<ProcedureNodeChangedEventArgs> OnProcedureNodeChange;
        /// <summary>
        /// 添加多个流程。
        /// </summary>
        /// <param name="nodes">流程集合</param>
        void AddProcedureNodes(params ProcedureNode[] nodes);
        /// <summary>
        /// 运行流程。
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        void RunProcedureNode<T>() where T : ProcedureNode;
        /// <summary>
        ///  运行流程。
        /// </summary>
        /// <param name="type">流程节点类型</param>
        void RunProcedureNode(Type type);
        /// <summary>
        /// 是否存在节点。
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        /// <returns>存在结果</returns>
        bool HasProcedureNode<T>() where T : ProcedureNode;
        /// <summary>
        /// 是否存在节点。
        /// </summary>
        /// <param name="type">流程节点类型</param>
        /// <returns>存在结果</returns>
        bool HasProcedureNode(Type type);
        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <param name="type">流程节点类型</param>
        /// <param name="node">获得的节点</param>
        /// <returns>获得结果</returns>
        bool PeekProcedureNode(Type type, out ProcedureNode node);
        /// <summary>
        ///  获取流程。
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        /// <param name="node">获得的节点</param>
        /// <returns>获得结果</returns>
        bool PeekProcedureNode<T>(out T node) where T : ProcedureNode;
        /// <summary>
        /// 移除多个流程。
        /// </summary>
        /// <param name="types">流程类型集合</param>
        void RemoveProcedureNodes(params Type[] types);
        /// <summary>
        /// 移除一个流程。
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        /// <returns>移除结果</returns>
        bool RemoveProcedureNode<T>() where T : ProcedureNode;
        /// <summary>
        /// 移除一个流程。
        /// </summary>
        /// <param name="type">流程节点类型</param>
        /// <returns>移除结果</returns>
        bool RemoveProcedureNode(Type type);
    }
}
