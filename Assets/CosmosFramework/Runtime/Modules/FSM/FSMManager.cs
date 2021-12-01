using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    //================================================
    /*
     * 1、状态机模块；
     */
    //================================================
    [Module]
    internal sealed class FSMManager : Module, IFSMManager
    {
        #region Properties
        /// <summary>
        /// 单个状态机
        /// </summary>
        Dictionary<Type, FSMBase> fsmIndividualDict;
        /// <summary>
        /// 状态机群组集合
        /// </summary>
        Dictionary<Type, FSMGroup> fsmGroupDict;
        List<FSMBase> fsmCache = new List<FSMBase>();
        public int FSMCount { get { return fsmIndividualDict.Count; } }
        #endregion

        #region Methods
        /// <summary>
        /// 为特定类型设置轮询间隔
        /// 若设置时间为小于等于0，则默认使用0；
        /// </summary>
        /// <typeparam name="T">类型目标</typeparam>
        /// <param name="interval">轮询间隔 毫秒</param>
        public void SetFSMGroupRefreshInterval<T>(int interval)
           where T : class
        {
            Type type = typeof(T);
            SetFSMGroupRefreshInterval(type, interval);
        }
        /// <summary>
        /// 为特定类型设置轮询间隔；
        /// 若设置时间为小于等于0，则默认使用0；
        /// </summary>
        /// <param name="type">类型目标</param>
        /// <param name="interval">轮询间隔 毫秒</param>
        public void SetFSMGroupRefreshInterval(Type type, int interval)
        {
            if (HasFSMGroup(type))
                fsmGroupDict[type].SetRefreshInterval(interval);
            else
                throw new ArgumentNullException("FSMManager：FSM Set not exist ! Type:" + type.ToString());
        }
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public void PauseFSMGroup<T>()
    where T : class
        {
            Type type = typeof(T);
            PauseFSMGroup(type);
        }
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        public void PauseFSMGroup(Type type)
        {
            if (HasFSMGroup(type))
                fsmGroupDict[type].OnPause();
            else
                throw new ArgumentNullException("FSMManager：FSM Set not exist ! Type:" + type.ToString());
        }
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public void UnPauseFSMGroup<T>()
           where T : class
        {
            UnPauseFSMGroup(typeof(T));
        }
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        public void UnPauseFSMGroup(Type type)
        {
            if (HasFSMGroup(type))
                fsmGroupDict[type].OnUnPause();
            else
                throw new ArgumentNullException("FSMManager：FSM Set not exist ! Type:" + type.ToString());
        }
        public FSMBase GetIndividualFSM<T>()
    where T : class
        {
            Type type = typeof(T).GetType();
            return GetIndividualFSM(type);
        }
        public FSMBase GetIndividualFSM(Type type)
        {
            if (fsmIndividualDict.ContainsKey(type))
            {
                return fsmIndividualDict[type];
            }
            else return null;
        }
        /// <summary>
        /// 获取某类型状态机元素集合中元素的个数
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <returns>元素数量</returns>
        public int GetFSMGroupElementCount<T>()
    where T : class
        {
            return GetFSMGroupElementCount(typeof(T));
        }
        public int GetFSMGroupElementCount(Type type)
        {
            if (!HasFSMGroup(type))
                throw new ArgumentNullException("FSMManager：FSM Set not exist ! Type:" + type.ToString());
            return fsmGroupDict[type].GetFSMCount();
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>状态机集合</returns>
        public List<FSMBase> GetFSMGroup<T>()
           where T : class
        {
            return GetFSMGroup(typeof(T));
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <returns>状态机集合</returns>
        public List<FSMBase> GetFSMGroup(Type type)
        {
            FSMGroup fsmPool;
            fsmGroupDict.TryGetValue(type, out fsmPool);
            return fsmPool.FSMList;
        }
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        public FSMBase GetGroupElementFSM<T>(Predicate<FSMBase> predicate)
           where T : class
        {
            return GetGroupElementFSM(typeof(T), predicate);
        }
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        public FSMBase GetGroupElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            if (fsmIndividualDict.ContainsKey(type))
            {
                return fsmIndividualDict[type];
            }
            else return null;
        }
        public FSMBase[] GetAllIndividualFSM()
        {
            if (fsmIndividualDict.Count <= 0)
                return null;
            List<FSMBase> fsms = new List<FSMBase>();
            foreach (var fsm in fsmIndividualDict)
            {
                fsms.Add(fsm.Value);
            }
            return fsms.ToArray();
        }
        public bool HasIndividualFSM<T>()
           where T : class
        {
            return HasIndividualFSM(typeof(T));
        }
        public bool HasIndividualFSM(Type type)
        {
            return fsmIndividualDict.ContainsKey(type);
        }
        /// <summary>
        /// 是否拥有指定类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>是否存在</returns>
        public bool HasFSMGroup<T>()
           where T : class
        {
            return HasFSMGroup(typeof(T));
        }
        public bool HasFSMGroup(Type type)
        {
            return fsmGroupDict.ContainsKey(type);
        }
        public bool GroupContainsFSM<T>(Predicate<FSMBase> predicate)
    where T : class
        {
            return GroupContainsFSM(typeof(T), predicate);
        }
        public bool GroupContainsFSM(Type type, Predicate<FSMBase> predicate)
        {
            if (!fsmGroupDict.ContainsKey(type))
                return false;
            var fsmPool = fsmGroupDict[type];
            return fsmPool.HasFSM(predicate);
        }
        public bool GroupContainsFSM<T>(FSMBase fsm)
           where T : class
        {
            return GroupContainsFSM(typeof(T), fsm);
        }
        public bool GroupContainsFSM(Type type, FSMBase fsm)
        {
            if (!fsmGroupDict.ContainsKey(type))
                return false;
            var fsmPool = fsmGroupDict[type];
            return fsmPool.HasFSM(fsm);
        }
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="individual">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(T owner, bool individual, params FSMState<T>[] states)
           where T : class
        {
            return CreateFSM(string.Empty, owner, individual, states);
        }
        public IFSM<T> CreateFSM<T>(string name, T owner, bool individual, params FSMState<T>[] states)
           where T : class
        {
            Type type = typeof(T);
            FSM<T> fsm = default;
            if (individual)
            {
                if (HasIndividualFSM(type))
                    throw new ArgumentException("FSMManager : FSM is exists" + type.ToString());
                fsm = FSM<T>.Create(name, owner, states);
                fsmIndividualDict.Add(type, fsm);
            }
            else
            {
                fsm = FSM<T>.Create(name, owner, states);
                if (HasFSMGroup(type))
                {
                    if (fsmGroupDict[type].HasFSM(fsm))
                        fsm.Shutdown();
                    else
                        fsmGroupDict[type].AddFSM(fsm);
                }
                else
                {
                    var fsmPool = new FSMGroup();
                    fsmPool.AddFSM(fsm);
                    fsmGroupDict.Add(type, fsmPool);
                }
            }
            return fsm;
        }
        public IFSM<T> CreateFSM<T>(T owner, bool individual, IList<FSMState<T>> states)
           where T : class
        {
            return CreateFSM(string.Empty, owner, individual, states);
        }
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="individual">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(string name, T owner, bool individual, IList<FSMState<T>> states)
           where T : class
        {
            Type type = typeof(T);
            FSM<T> fsm = default;
            if (individual)
            {
                if (HasIndividualFSM(type))
                    throw new ArgumentException("FSMManager : FSM is exists" + type.ToString());
                fsm = FSM<T>.Create(name, owner, states);
                fsmIndividualDict.Add(type, fsm);
            }
            else
            {
                fsm = FSM<T>.Create(name, owner, states);
                if (HasFSMGroup(type))
                {
                    if (fsmGroupDict[type].HasFSM(fsm))
                        fsm.Shutdown();
                    else
                        fsmGroupDict[type].AddFSM(fsm);
                }
                else
                {
                    var fsmPool = new FSMGroup();
                    fsmPool.AddFSM(fsm);
                    fsmGroupDict.Add(type, fsmPool);
                }
            }
            return fsm;
        }
        /// <summary>
        /// 销毁独立的状态机
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        public void DestoryIndividualFSM<T>()
           where T : class
        {
            DestoryIndividualFSM(typeof(T));
        }
        public void DestoryIndividualFSM(Type type)
        {
            FSMBase fsm = null;
            if (fsmIndividualDict.TryGetValue(type, out fsm))
            {
                fsm.Shutdown();
                fsmIndividualDict.Remove(type);
            }
        }
        public void DestoryFSMGroup<T>()
    where T : class
        {
            DestoryFSMGroup(typeof(T));
        }
        public void DestoryFSMGroup(Type type)
        {
            FSMBase fsm = null;
            if (fsmIndividualDict.TryGetValue(type, out fsm))
            {
                fsm.Shutdown();
                fsmIndividualDict.Remove(type);
            }
        }
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <typeparam name="T">拥有者</typeparam>
        /// <param name="predicate">查找条件</param>
        public void DestoryGroupElementFSM<T>(Predicate<FSMBase> predicate)
           where T : class
        {
            DestoryGroupElementFSM(typeof(T), predicate);
        }
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找条件</param>
        public void DestoryGroupElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            FSMGroup fsmPool;
            if (fsmGroupDict.TryGetValue(type, out fsmPool))
            {
                fsmPool.DestoryFSM(predicate);
            }
        }
        public void DestoryAllFSM()
        {
            if (fsmIndividualDict.Count > 0)
                foreach (var fsm in fsmIndividualDict)
                {
                    fsm.Value.Shutdown();
                }
            fsmCache.Clear();
            if (fsmGroupDict.Count > 0)
            {
                foreach (var fsmPool in fsmGroupDict.Values)
                {
                    fsmPool.DestoryAllFSM();
                }
            }
            fsmCache.Clear();
            fsmIndividualDict.Clear();
            fsmGroupDict.Clear();
        }
        #endregion
        protected override void OnInitialization()
        {
            fsmGroupDict = new Dictionary<Type, FSMGroup>();
            fsmIndividualDict = new Dictionary<Type, FSMBase>();
        }
        [TickRefresh]
        void OnRefresh()
        {
            if (IsPause)
                return;
            if (fsmIndividualDict.Count > 0)
                foreach (var fsm in fsmIndividualDict)
                {
                    fsm.Value.OnRefresh();
                }
            if (fsmGroupDict.Count > 0)
            {
                foreach (var fsmPool in fsmGroupDict.Values)
                {
                    fsmPool.OnRefresh();
                }
            }
        }
    }
}