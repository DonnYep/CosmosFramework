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
    public interface IProcedureManager: IModuleManager
    {
        /// <summary>
        /// 流程节点数量；
        /// </summary>
        int ProcedureCount { get; }
        /// <summary>
        /// 当前节点；
        /// </summary>
        ProcedureState CurrentProcedureNode { get; }
        /// <summary>
        /// 添加多个流程；
        /// </summary>
        /// <param name="nodes">流程集合</param>
        void AddProcedures(params ProcedureState[] nodes);
        /// <summary>
        /// 运行流程；
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        void RunProcedure<T>() where T : ProcedureState;
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
        bool HasProcedure<T>() where T : ProcedureState;
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
        bool PeekProcedure(Type type, out ProcedureState node);
        /// <summary>
        ///  获取流程；
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        /// <param name="node">获得的节点</param>
        /// <returns>获得结果</returns>
        bool PeekProcedure<T>(out ProcedureState node) where T : ProcedureState;
        /// <summary>
        /// 移除多个流程；
        /// </summary>
        /// <param name="types">流程类型集合</param>
        void RemoveProcedures(params Type[] types);
        /// <summary>
        /// 移除一个流程；
        /// </summary>
        /// <typeparam name="T">流程节点类型</typeparam>
        /// <returns>移除结果</returns>
        bool RemoveProcedure<T>() where T : ProcedureState;
        /// <summary>
        /// 移除一个流程；
        /// </summary>
        /// <param name="type">流程节点类型</param>
        /// <returns>移除结果</returns>
        bool RemoveProcedure(Type type);

    }
}
