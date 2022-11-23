using System;
using System.Collections.Generic;
namespace Cosmos.FSM
{
    //================================================
    /*
     * 1、状态机模块；
     * 
     * 2、状态机组别设置是互斥的。一个状态机只允许拥有一个组别；
     */
    //================================================
    public interface IFSMManager : IModuleManager, IModuleInstance
    {
        /// <summary>
        /// 状态机数量；
        /// </summary>
        int FSMCount { get; }
        /// <summary>
        /// 状态机组数量；
        /// </summary>
        int FSMGroupCount { get; }
        /// <summary>
        /// 获取状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>状态机基类</returns>
        FSMBase GetFSM<T>() where T : class;
        /// <summary>
        /// 获取状态机；
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <returns>状态机基类</returns>
        FSMBase GetFSM(Type type);
        /// <summary>
        /// 获取所有状态机；
        /// </summary>
        /// <returns>状态机集合</returns>
        IList<FSMBase> GetAllFSMs();
        /// <summary>
        /// 是否存在状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <returns>存在结果</returns>
        bool HasFSM<T>(string name) where T : class;
        /// <summary>
        /// 是否存在状态机；
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="name">状态机名称</param>
        /// <returns>存在结果</returns>
        bool HasFSM(Type type, string name);
        /// <summary>
        /// 创建状态机；
        /// 不分配状态机组；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(T owner, IList<FSMState<T>> states) where T : class;
        /// <summary>
        /// 创建状态机；
        /// 不分配状态机组；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(T owner, params FSMState<T>[] states) where T : class;
        /// <summary>
        ///  创建状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="fsmGroupName">状态机组名，若为空，则不分配组</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(T owner, string fsmGroupName, IList<FSMState<T>> states) where T : class;
        /// <summary>
        /// 创建状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="fsmGroupName">状态机组名，若为空，则不分配组</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(T owner, string fsmGroupName, params FSMState<T>[] states) where T : class;
        /// <summary>
        ///  创建状态机；
        /// 不分配状态机组；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(string name, T owner, IList<FSMState<T>> states) where T : class;
        /// <summary>
        ///  创建状态机；
        /// 不分配状态机组；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(string name, T owner, params FSMState<T>[] states) where T : class;
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="fsmGroupName">状态机组名</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(string name, T owner, string fsmGroupName, IList<FSMState<T>> states) where T : class;
        /// <summary>
        /// 创建状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="fsmGroupName">状态机组名，若为空，则不分配组</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(string name, T owner, string fsmGroupName, params FSMState<T>[] states) where T : class;
        /// <summary>
        /// 销毁独立的状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        void DestoryFSM<T>() where T : class;
        /// <summary>
        /// 销毁独立的状态机；
        /// </summary>
        /// <param name="type">拥有者类型</param>
        void DestoryFSM(Type type);
        /// <summary>
        /// 获取状态机组
        /// </summary>
        /// <param name="fsmGroupName">状态机组名</param>
        /// <param name="fsmGroup">状态机组</param>
        /// <returns>是否存在组</returns>
        bool PeekFSMGroup(string fsmGroupName, out IFSMGroup fsmGroup);
        /// <summary>
        /// 移除状态机组；
        /// </summary>
        /// <param name="fsmGroupName">状态机组名</param>
        void RemoveFSMGroup(string fsmGroupName);
        /// <summary>
        /// 是否拥有指定类型的状态机集合；
        /// </summary>
        /// <returns>是否存在</returns>
        bool HasFSMGroup(string name);
        /// <summary>
        /// 为状态机设置组别；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名</param>
        /// <param name="fsmGroupName">状态机组名</param>
        void SetFSMGroup<T>(string name, string fsmGroupName) where T : class;
        /// <summary>
        /// 为状态机设置组别；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="fsmGroupName">状态机组名</param>
        void SetFSMGroup<T>(string fsmGroupName) where T : class;
        /// <summary>
        /// 销毁所有状态机；
        /// </summary>
        void DestoryAllFSM();
    }
}
