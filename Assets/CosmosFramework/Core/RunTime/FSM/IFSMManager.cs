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
        int FsmCount { get ; }
        void SetFSMSetRefreshInterval<T>(float interval) where T : class;
        /// <summary>
        /// 为特定类型设置轮询间隔；
        /// 若设置时间为小于等于0，则默认使用0；
        /// </summary>
        /// <param name="type">类型目标</param>
        /// <param name="interval">轮询间隔</param>
        void SetFSMSetRefreshInterval(Type type, float interval);
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        void PauseFSMSet<T>() where T : class;
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        void PauseFSMSet(Type type);
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        void UnPauseFSMSet<T>() where T : class;
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        void UnPauseFSMSet(Type type);
        FSMBase GetIndividualFSM<T>() where T : class;
        FSMBase GetIndividualFSM(Type type);
        /// <summary>
        /// 获取某类型状态机元素集合中元素的个数
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <returns>元素数量</returns>
        int GetFSMSetElementCount<T>() where T : class;
        int GetFSMSetElementCount(Type type);
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>状态机集合</returns>
        List<FSMBase> GetFSMSet<T>() where T : class;
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <returns>状态机集合</returns>
        List<FSMBase> GetFSMSet(Type type);
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        FSMBase GetSetElementFSM<T>(Predicate<FSMBase> predicate) where T : class;
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        FSMBase GetSetElementFSM(Type type, Predicate<FSMBase> predicate);
        FSMBase[] GetAllIndividualFSM();
        bool HasIndividualFSM<T>() where T : class;
        bool HasIndividualFSM(Type type);
        /// <summary>
        /// 是否拥有指定类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>是否存在</returns>
        bool HasFSMSet<T>() where T : class;
        bool HasFSMSet(Type type);
        bool HasSetElementFSM<T>(Predicate<FSMBase> predicate) where T : class;
        bool HasSetElementFSM(Type type, Predicate<FSMBase> predicate);
        bool HasSetElementFSM<T>(FSMBase fsm) where T : class;
        bool HasSetElementFSM(Type type, FSMBase fsm);
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
        IFSM<T> CreateFSM<T>(T owner, bool Individual, List<FSMState<T>> states) where T : class;
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
        IFSM<T> CreateFSM<T>(string name, T owner, bool Individual, List<FSMState<T>> states) where T : class;
        /// <summary>
        /// 销毁独立的状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void DestoryIndividualFSM<T>() where T : class;
        void DestoryIndividualFSM(Type type);
        void DestoryFSMSet<T>() where T : class;
        void DestoryFSMSet(Type type);
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <param name="predicate">查找条件</param>
        void DestorySetElementFSM<T>(Predicate<FSMBase> predicate) where T : class;
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找条件</param>
        void DestorySetElementFSM(Type type, Predicate<FSMBase> predicate);
        void DestoryAllFSM();
    }
}
