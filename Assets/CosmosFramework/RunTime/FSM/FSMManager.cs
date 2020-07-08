using System.Collections;
using System.Collections.Generic;
using System;
using Cosmos;
namespace Cosmos.FSM
{
    /// <summary>
    /// fsmMgr设计成，轮询是在具体对象山给轮询的，fsmMgr作为一个Fsm的事件中心
    /// </summary>
    internal sealed class FSMManager : Module<FSMManager>
    {
        /// <summary>
        /// 单个状态机
        /// </summary>
        Dictionary<Type, FSMBase> fsmIndividualDict = new Dictionary<Type, FSMBase>();
        /// <summary>
        /// 状态机群组集合
        /// </summary>
        Dictionary<Type, List<FSMBase>> fsmSetDict = new Dictionary<Type, List<FSMBase>>();
        List<FSMBase> fsmCache = new List<FSMBase>();
        bool isPause = false;
        internal int FsmCount { get { return fsmIndividualDict.Count; } }
        internal FSMBase GetIndividualFSM<T>()
            where T : class
        {
            Type type = typeof(T).GetType();
            return GetIndividualFSM(type);
        }
        internal FSMBase GetIndividualFSM(Type type)
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
        internal int GetFSMSetElementCount<T>()
    where T : class
        {
            return GetFSMSetElementCount(typeof(T));
        }
        internal int GetFSMSetElementCount(Type type)
        {
            if (!HasFSMSet(type))
                throw new ArgumentNullException("FSMManager：FSM Set not exist ! Type:" + type.ToString());
            return fsmSetDict[type].Count;
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>状态机集合</returns>
        internal List<FSMBase> GetFSMSet<T>()
            where T : class
        {
            return GetFSMSet(typeof(T));
        }
        /// <summary>
        /// 获取某一类型的状态机集合
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <returns>状态机集合</returns>
        internal List<FSMBase> GetFSMSet(Type type)
        {
            List<FSMBase> fsmSet;
            fsmSetDict.TryGetValue(type, out fsmSet);
            return fsmSet;
        }
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        internal FSMBase GetSetElementFSM<T>(Predicate<FSMBase> predicate)
            where T:class
        {
            return GetSetElementFSM(typeof(T),predicate);
        }
        /// <summary>
        /// 通过查找语句获得某一类型的状态机元素
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找语句</param>
        /// <returns>查找到的FSM</returns>
        internal FSMBase GetSetElementFSM(Type type,Predicate<FSMBase> predicate)
        {
            if (fsmIndividualDict.ContainsKey(type))
            {
                return fsmIndividualDict[type];
            }
            else return null;
        }
        internal FSMBase[] GetAllIndividualFSM()
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
        internal bool HasIndividualFSM<T>()
            where T : class
        {
            return HasIndividualFSM(typeof(T));
        }
        internal bool HasIndividualFSM(Type type)
        {
            return fsmIndividualDict.ContainsKey(type);
        }
        /// <summary>
        /// 是否拥有指定类型的状态机集合
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>是否存在</returns>
        internal bool HasFSMSet<T>()
            where T:class
        {
            return HasFSMSet(typeof(T));
        }
        internal bool HasFSMSet(Type type)
        {
            return fsmSetDict.ContainsKey(type);
        }
        internal bool HasSetElementFSM<T>(Predicate<FSMBase> predicate)
    where T : class
        {
            return HasSetElementFSM(typeof(T),predicate);
        }
        internal bool HasSetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            if (!fsmSetDict.ContainsKey(type))
                return false;
            var fsmSet = fsmSetDict[type];
            return fsmSet.Find(predicate)!=null;
        }
        internal bool HasSetElementFSM<T>(FSMBase fsm)
            where T:class
        {
            return HasSetElementFSM(typeof(T), fsm);
        }
        internal bool HasSetElementFSM(Type type, FSMBase fsm)
        {
            if (!fsmSetDict.ContainsKey(type))
                return false;
            var fsmSet = fsmSetDict[type];
            return fsmSet.Contains(fsm);
        }
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="Individual">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        internal IFSM<T> CreateFSM<T>(T owner, bool Individual, params FSMState<T>[] states)
            where T : class
        {
            return CreateFSM(string.Empty, owner,Individual, states);
        }
        internal IFSM<T> CreateFSM<T>(string name,  T owner, bool Individual, params FSMState<T>[] states)
            where T : class
        {
            Type type = typeof(T).GetType();
            FSM<T> fsm = default;
            if (Individual)
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
                    if (fsmSetDict[type].Contains(fsm))
                        fsm.Shutdown();
                    else
                        fsmSetDict[type].Add(fsm);
                }
                else
                    fsmSetDict.Add(type, new List<FSMBase>() { fsm });
            }
            return fsm;
        }
        internal IFSM<T> CreateFSM<T>(T owner, bool Individual, List<FSMState<T>> states)
            where T : class
        {
            return CreateFSM(string.Empty, owner,Individual, states);
        }
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
        internal IFSM<T> CreateFSM<T>(string name, T owner, bool Individual, List<FSMState<T>> states)
            where T : class
        {
            Type type = typeof(T).GetType();
            FSM<T> fsm = default;
            if (Individual)
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
                    if (fsmSetDict[type].Contains(fsm))
                        fsm.Shutdown();
                    else
                        fsmSetDict[type].Add(fsm);
                }
                else
                    fsmSetDict.Add(type, new List<FSMBase>() { fsm });
            }
            return fsm;
        }
        /// <summary>
        /// 销毁独立的状态机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal void DestoryIndividualFSM<T>()
            where T : class
        {
            DestoryIndividualFSM(typeof(T));
        }
        internal void DestoryIndividualFSM(Type type)
        {
            FSMBase fsm = null;
            if (fsmIndividualDict.TryGetValue(type, out fsm))
            {
                fsm.Shutdown();
                fsmIndividualDict.Remove(type);
            }
        }
        internal void DestoryFSMSet<T>()
where T : class
        {
            DestoryFSMSet(typeof(T));
        }
        internal void DestoryFSMSet(Type type)
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
        internal void DestorySetElementFSM<T>(Predicate<FSMBase> predicate)
            where T:class
        {
            DestorySetElementFSM(typeof(T), predicate);
        }
        /// <summary>
        /// 销毁某类型的集合元素状态机
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="predicate">查找条件</param>
        internal void DestorySetElementFSM(Type type, Predicate<FSMBase> predicate)
        {
            List<FSMBase> fsmSet;
            if (fsmSetDict.TryGetValue(type, out fsmSet))
            {
                var fsm = fsmSet.Find(predicate);
                fsm.Shutdown();
                fsmSetDict[type].Remove(fsm);
            }
        }
        internal void ShutdownFSMSet<T>()
            where T:class
        {
            ShutdownFSMSet(typeof(T));
        }
        /// <summary>
        /// 终止某一类型的状态机集合
        /// </summary>
        /// <param name="type">拥有者类型</param>
        internal void ShutdownFSMSet(Type type)
        {
            if(fsmSetDict.ContainsKey(type))
            {
                var fsmSet = fsmSetDict[type];
                int count = fsmSet.Count;
                for (int i = 0; i < count; i++)
                {
                    fsmSet[i].Shutdown();
                }
            }
        }
        /// <summary>
        /// 终止独立的状态机
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        internal void ShutdownIndividualFSM<T>()
            where T:class
        {
            ShutdownIndividualFSM(typeof(T));
        }
        internal void ShutdownIndividualFSM(Type type)
        {
            if (fsmIndividualDict.ContainsKey(type))
                fsmIndividualDict[type].Shutdown();
        }
        internal void ShutdownAllFSM()
        {
            if (fsmIndividualDict.Count > 0)
                foreach (var fsm in fsmIndividualDict)
                {
                    fsm.Value.Shutdown();
                }
            fsmCache.Clear();
            if (fsmSetDict.Count > 0)
            {
                foreach (var fsmSet in fsmSetDict.Values)
                {
                    fsmCache.AddRange(fsmSet);
                }
                foreach (var fsm in fsmCache)
                {
                    fsm.Shutdown();
                }
            }
            fsmCache.Clear();
            fsmIndividualDict.Clear();
            fsmSetDict.Clear();
        }
        #region Module
        public override void OnRefresh()
        {
            if (isPause)
                return;
            if (fsmIndividualDict.Count > 0)
                foreach (var fsm in fsmIndividualDict)
                {
                    fsm.Value.OnRefresh();
                }
            fsmCache.Clear();
            if (fsmSetDict.Count > 0)
            {
                foreach (var fsmSet in fsmSetDict.Values)
                {
                    fsmCache.AddRange(fsmSet);
                }
                int count = fsmCache.Count;
                for (int i = 0; i < count; i++)
                {
                    fsmCache[i].OnRefresh();
                }
            }
        }
        #endregion
    }
}