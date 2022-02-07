using System;
using System.Collections.Generic;
namespace Cosmos.FSM
{
    public interface IFSMManager : IModuleManager
    {
        int FSMCount { get; }
        FSMBase GetFSM<T>() where T : class;
        FSMBase GetFSM(Type type);
        IList<FSMBase> GetAllFSMs();
        bool HasFSM<T>(string name) where T : class;
        bool HasFSM(Type type, string name);
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="groupName">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(T owner, string groupName, params FSMState<T>[] states) where T : class;
        IFSM<T> CreateFSM<T>(T owner, params FSMState<T>[] states) where T : class;
        IFSM<T> CreateFSM<T>(string name, T owner, string groupName, params FSMState<T>[] states) where T : class;
        IFSM<T> CreateFSM<T>(string name, T owner,  params FSMState<T>[] states) where T : class;
        IFSM<T> CreateFSM<T>(T owner, string groupName, IList<FSMState<T>> states) where T : class;
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="groupName">状态机组名</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(string name, T owner, string groupName, IList<FSMState<T>> states) where T : class;
        /// <summary>
        /// 销毁独立的状态机
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        void DestoryFSM<T>() where T : class;
        void DestoryFSM(Type type);
        bool PeekFSMGroup(string fsmGroupName, out IFSMGroup fsmGroup);
        void RemoveFSMGroup(string groupName);
        /// <summary>
        /// 是否拥有指定类型的状态机集合
        /// </summary>
        /// <returns>是否存在</returns>
        bool HasFSMGroup(string name);
        /// <summary>
        /// 为一个状态机设置组别；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名</param>
        /// <param name="fsmGroupName">状态机组别名</param>
        void SetFSMGroup<T>(string name, string fsmGroupName) where T : class;
        void SetFSMGroup<T>(string fsmGroupName) where T : class;
        void DestoryAllFSM();
    }
}
