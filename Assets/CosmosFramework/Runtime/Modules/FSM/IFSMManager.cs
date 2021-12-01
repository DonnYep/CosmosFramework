using Cosmos.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface IFSMManager:IModuleManager
    {
        int FSMCount { get ; }
        /// <summary>
        /// 为特定类型设置轮询间隔
        /// 若设置时间为小于等于0，则默认使用0；
        /// </summary>
        /// <typeparam name="T">类型目标</typeparam>
        /// <param name="interval">轮询间隔 毫秒</param>
        void SetFSMGroupRefreshInterval<T>(int interval) where T : class;
        /// <summary>
        /// 为特定类型设置轮询间隔；
        /// 若设置时间为小于等于0，则默认使用0；
        /// </summary>
        /// <param name="type">类型目标</param>
        /// <param name="interval">轮询间隔 毫秒</param>
        void SetFSMGroupRefreshInterval(Type type, int interval);
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        void PauseFSMGroup<T>() where T : class;
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        void PauseFSMGroup(Type type);
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        void UnPauseFSMGroup<T>() where T : class;
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        void UnPauseFSMGroup(Type type);
        FSMBase GetIndividualFSM<T>() where T : class;
        FSMBase GetIndividualFSM(Type type);
        /// <summary>
        /// 获取某类型状态机元素集合中元素的个数
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <returns>元素数量</returns>
        int GetFSMGroupElementCount<T>() where T : class;
        int GetFSMGroupElementCount(Type type);
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>状态机集合</returns>
        List<FSMBase> GetFSMGroup<T>() where T : class;
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <returns>状态机集合</returns>
        List<FSMBase> GetFSMGroup(Type type);
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        FSMBase GetGroupElementFSM<T>(Predicate<FSMBase> predicate) where T : class;
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        FSMBase GetGroupElementFSM(Type type, Predicate<FSMBase> predicate);
        FSMBase[] GetAllIndividualFSM();
        bool HasIndividualFSM<T>() where T : class;
        bool HasIndividualFSM(Type type);
        /// <summary>
        /// 是否拥有指定类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>是否存在</returns>
        bool HasFSMGroup<T>() where T : class;
        bool HasFSMGroup(Type type);
        bool GroupContainsFSM<T>(Predicate<FSMBase> predicate) where T : class;
        bool GroupContainsFSM(Type type, Predicate<FSMBase> predicate);
        bool GroupContainsFSM<T>(FSMBase fsm) where T : class;
        bool GroupContainsFSM(Type type, FSMBase fsm);
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="Individual">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(T owner, bool Individual, params FSMState<T>[] states) where T : class;
        IFSM<T> CreateFSM<T>(string name, T owner, bool Individual, params FSMState<T>[] states) where T : class;
        IFSM<T> CreateFSM<T>(T owner, bool Individual, IList<FSMState<T>> states) where T : class;
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="Individual">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        IFSM<T> CreateFSM<T>(string name, T owner, bool Individual, IList<FSMState<T>> states) where T : class;
        /// <summary>
        /// 销毁独立的状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void DestoryIndividualFSM<T>() where T : class;
        void DestoryIndividualFSM(Type type);
        void DestoryFSMGroup<T>() where T : class;
        void DestoryFSMGroup(Type type);
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <param name="predicate">查找条件</param>
        void DestoryGroupElementFSM<T>(Predicate<FSMBase> predicate) where T : class;
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找条件</param>
        void DestoryGroupElementFSM(Type type, Predicate<FSMBase> predicate);
        void DestoryAllFSM();
    }
}
