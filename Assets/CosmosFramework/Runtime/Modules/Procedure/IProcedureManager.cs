using System;

namespace Cosmos.Procedure
{
    //================================================
    /*
     * 1、流程管理模块
     */
    //================================================
    public interface IProcedureManager: IModuleManager
    {
        int ProcedureNodeCount { get; }
        /// <summary>
        /// 当前节点；
        /// </summary>
        ProcedureNode CurrentProcedureNode { get; }
        /// <summary>
        /// 添加多个流程；
        /// </summary>
        /// <param name="nodes">流程集合</param>
        void AddProcedures(params ProcedureNode[] nodes);
        /// <summary>
        /// 运行流程；
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        void RunProcedure<T>() where T : ProcedureNode;
        /// <summary>
        ///  运行流程；
        /// </summary>
        /// <param name="type">流程节点类型</param>
        void RunProcedure(Type type);
        /// <summary>
        /// 是否存在节点；
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        /// <returns>存在结果</returns>
        bool HasProcedure<T>() where T : ProcedureNode;
        /// <summary>
        /// 是否存在节点；
        /// </summary>
        /// <param name="type">流程节点类型</param>
        /// <returns>存在结果</returns>
        bool HasProcedure(Type type);
        /// <summary>
        /// 获取流程；
        /// </summary>
        /// <param name="type">流程节点类型</param>
        /// <param name="node">获得的节点</param>
        /// <returns>获得结果</returns>
        bool PeekProcedure(Type type, out ProcedureNode node);
        /// <summary>
        ///  获取流程；
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        /// <param name="node">获得的节点</param>
        /// <returns>获得结果</returns>
        bool PeekProcedure<T>(out ProcedureNode node) where T : ProcedureNode;
        /// <summary>
        /// 移除多个流程；
        /// </summary>
        /// <param name="types">流程类型集合</param>
        void RemoveProcedures(params Type[] types);
    }
}
