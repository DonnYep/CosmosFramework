using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.FSM
{
    //================================================
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
        Dictionary<Type, FSMGroup> fsmSetDict;
        List<FSMBase> fsmCache = new List<FSMBase>();
        public int FsmCount { get { return fsmIndividualDict.Count; } }
        #endregion

        #region Methods

        #region Module
        public override void OnInitialization()
        {
            fsmSetDict = new Dictionary<Type, FSMGroup>();
            fsmIndividualDict = new Dictionary<Type, FSMBase>(); 
        }
        #endregion
        /// <summary>
        /// 为特定类型设置轮询间隔
        /// 若设置时间为小于等于0，则默认使用0；
        /// </summary>
        /// <typeparam name="T">类型目标</typeparam>
        /// <param name="interval">轮询间隔</param>
        public void SetFSMSetRefreshInterval<T>(float interval)
           where T : class
        {
            Type type = typeof(T);
            SetFSMSetRefreshInterval(type, interval);
        }
        /// <summary>
        /// 为特定类型设置轮询间隔；
        /// 若设置时间为小于等于0，则默认使用0；
        /// </summary>
        /// <param name="type">类型目标</param>
        /// <param name="interval">轮询间隔</param>
        public void SetFSMSetRefreshInterval(Type type, float interval)
        {
            if (HasFSMSet(type))
                fsmSetDict[type].SetRefreshInterval(interval);
            else
                throw new ArgumentNullException("FSMManager：FSM Set not exist ! Type:" + type.ToString());
        }
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public void PauseFSMSet<T>()
   where T : class
        {
            Type type = typeof(T);
            PauseFSMSet(type);
        }
        /// <summary>
        /// 暂停指定类型fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        public void PauseFSMSet(Type type)
        {
            if (HasFSMSet(type))
                fsmSetDict[type].OnPause();
            else
                throw new ArgumentNullException("FSMManager：FSM Set not exist ! Type:" + type.ToString());
        }
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public void UnPauseFSMSet<T>()
           where T : class
        {
            UnPauseFSMSet(typeof(T));
        }
        /// <summary>
        /// 继续执行指定fsm集合
        /// </summary>
        /// <param name="type">目标类型</param>
        public void UnPauseFSMSet(Type type)
        {
            if (HasFSMSet(type))
                fsmSetDict[type].OnUnPause();
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
        public int GetFSMSetElementCount<T>()
   where T : class
        {
            return GetFSMSetElementCount(typeof(T));
        }
        public int GetFSMSetElementCount(Type type)
        {
            if (!HasFSMSet(type))
                throw new ArgumentNullException("FSMManager：FSM Set not exist ! Type:" + type.ToString());
            return fsmSetDict[type].GetFSMCount();
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>状态机集合</returns>
        public List<FSMBase> GetFSMSet<T>()
           where T : class
        {
            return GetFSMSet(typeof(T));
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <returns>状态机集合</returns>
        public List<FSMBase> GetFSMSet(Type type)
        {
            FSMGroup fsmPool;
            fsmSetDict.TryGetValue(type, out fsmPool);
            return fsmPool.FSMList;
        }
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        public FSMBase GetSetElementFSM<T>(Predicate<FSMBase> predicate)
           where T : class
        {
            return GetSetElementFSM(typeof(T), predicate);
        }
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        public FSMBase GetSetElementFSM(Type type, Predicate<FSMBase> predicate)
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
        public bool HasFSMSet<T>()
           where T : class
        {
            return HasFSMSet(typeof(T));
        }
        public bool HasFSMSet(Type type)
        {
            return fsmSetDict.ContainsKey(type);
        }
        public bool HasSetElementFSM<T>(Predicate<FSMBase> predicate)
   where T : class
        {
            return HasSetElementFSM(typeof(T), predicate);
        }
        public bool HasSetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            if (!fsmSetDict.ContainsKey(type))
                return false;
            var fsmPool = fsmSetDict[type];
            return fsmPool.HasFSM(predicate);
        }
        public bool HasSetElementFSM<T>(FSMBase fsm)
           where T : class
        {
            return HasSetElementFSM(typeof(T), fsm);
        }
        public bool HasSetElementFSM(Type type, FSMBase fsm)
        {
            if (!fsmSetDict.ContainsKey(type))
                return false;
            var fsmPool = fsmSetDict[type];
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
        //cluster集群；
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
                if (HasFSMSet(type))
                {
                    if (fsmSetDict[type].HasFSM(fsm))
                        fsm.Shutdown();
                    else
                        fsmSetDict[type].AddFSM(fsm);
                }
                else
                {
                    var fsmPool = new FSMGroup();
                    fsmPool.AddFSM(fsm);
                    fsmSetDict.Add(type, fsmPool);
                }
            }
            return fsm;
        }
        public IFSM<T> CreateFSM<T>(T owner, bool individual, List<FSMState<T>> states)
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
        public IFSM<T> CreateFSM<T>(string name, T owner, bool individual, List<FSMState<T>> states)
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
                if (HasFSMSet(type))
                {
                    if (fsmSetDict[type].HasFSM(fsm))
                        fsm.Shutdown();
                    else
                        fsmSetDict[type].AddFSM(fsm);
                }
                else
                {
                    var fsmPool = new FSMGroup();
                    fsmPool.AddFSM(fsm);
                    fsmSetDict.Add(type, fsmPool);
                }
            }
            return fsm;
        }
        /// <summary>
        /// 销毁独立的状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
        public void DestoryFSMSet<T>()
where T : class
        {
            DestoryFSMSet(typeof(T));
        }
        public void DestoryFSMSet(Type type)
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
        public void DestorySetElementFSM<T>(Predicate<FSMBase> predicate)
           where T : class
        {
            DestorySetElementFSM(typeof(T), predicate);
        }
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找条件</param>
        public void DestorySetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            FSMGroup fsmPool;
            if (fsmSetDict.TryGetValue(type, out fsmPool))
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
            if (fsmSetDict.Count > 0)
            {
                foreach (var fsmPool in fsmSetDict.Values)
                {
                    fsmPool.DestoryAllFSM();
                }
            }
            fsmCache.Clear();
            fsmIndividualDict.Clear();
            fsmSetDict.Clear();
        }
        #endregion
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
            if (fsmSetDict.Count > 0)
            {
                foreach (var fsmPool in fsmSetDict.Values)
                {
                    fsmPool.OnRefresh();
                }
            }
        }
    }
}