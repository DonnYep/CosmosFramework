using System.Collections.Generic;
using System;
using System.Linq;
namespace Cosmos.FSM
{
    //================================================
    /*
     * 1、状态机模块；
     * 
     * 2、状态机组别设置是互斥的。一个状态机只允许拥有一个组别；
     */
    //================================================
    [Module]
    internal sealed partial class FSMManager : Module, IFSMManager
    {
        #region Properties
        FSMGroupPool fsmGroupPool = new FSMGroupPool();
        /// <summary>
        /// 单个状态机
        /// </summary>
        Dictionary<TypeStringPair, FSMBase> fsmDict;
        /// <summary>
        /// 状态机群组集合
        /// </summary>
        Dictionary<string, FSMGroup> fsmGroupDict;
        List<FSMBase> fsmCache = new List<FSMBase>();
        /// <summary>
        /// 状态机数量；
        /// </summary>
        public int FSMCount { get { return fsmDict.Count; } }
        #endregion

        #region Methods
        /// <summary>
        /// 获取状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns>状态机基类</returns>
        public FSMBase GetFSM<T>()
    where T : class
        {
            Type type = typeof(T).GetType();
            return GetFSM(type);
        }
        /// <summary>
        /// 获取状态机；
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <returns>状态机基类</returns>
        public FSMBase GetFSM(Type type)
        {
            fsmDict.TryGetValue(new TypeStringPair(type), out var fsm);
            return fsm;
        }
        /// <summary>
        /// 获取所有状态机；
        /// </summary>
        /// <returns>状态机集合</returns>
        public IList<FSMBase> GetAllFSMs()
        {
            return fsmDict.Values.ToArray();
        }
        /// <summary>
        /// 是否存在状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <returns>存在结果</returns>
        public bool HasFSM<T>(string name)
           where T : class
        {
            return HasFSM(typeof(T), name);
        }
        /// <summary>
        /// 是否存在状态机；
        /// </summary>
        /// <param name="type">拥有者类型</param>
        /// <param name="name">状态机名称</param>
        /// <returns>存在结果</returns>
        public bool HasFSM(Type type, string name)
        {
            return fsmDict.ContainsKey(new TypeStringPair(type, name));
        }
        /// <summary>
        /// 创建状态机；
        /// 不分配状态机组；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(T owner, IList<FSMState<T>> states) where T : class
        {
            return CreateFSM(string.Empty, owner, string.Empty, states);
        }
        /// <summary>
        /// 创建状态机；
        /// 不分配状态机组；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(T owner, params FSMState<T>[] states) where T :class
        {
            return CreateFSM(string.Empty, owner, string.Empty, states);
        }
        /// <summary>
        ///  创建状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="fsmGroupName">状态机组名，若为空，则不分配组</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(T owner, string fsmGroupName, IList<FSMState<T>> states)
           where T : class
        {
            return CreateFSM(string.Empty, owner, fsmGroupName, states);
        }
        /// <summary>
        /// 创建状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="fsmGroupName">状态机组名，若为空，则不分配组</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(T owner, string fsmGroupName, params FSMState<T>[] states)
           where T : class
        {
            return CreateFSM(string.Empty, owner, fsmGroupName, states);
        }
        /// <summary>
        ///  创建状态机；
        /// 不分配状态机组；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(string name, T owner, IList<FSMState<T>> states)where T:class
        {
            return CreateFSM<T>(name, owner, string.Empty, states);
        }
        /// <summary>
        ///  创建状态机；
        /// 不分配状态机组；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(string name, T owner, params FSMState<T>[] states) where T : class
        {
            return CreateFSM<T>(name, owner, string.Empty, states);
        }
        /// <summary>
        /// 创建状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="fsmGroupName">状态机组名</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(string name, T owner, string fsmGroupName, IList<FSMState<T>> states)
           where T : class
        {
            Type type = typeof(T);
            FSM<T> fsm = default;
            var fsmKey = new TypeStringPair(type, name);
            if (string.IsNullOrEmpty(fsmGroupName))
            {
                if (fsmDict.ContainsKey(fsmKey))
                    throw new ArgumentException("FSMManager : FSM is exists" + type.ToString());
                fsm = FSM<T>.Create(name, owner, states);
                fsmDict.Add(fsmKey, fsm);
            }
            else
            {
                fsm = FSM<T>.Create(name, owner, states);
                fsm.GroupName = fsmGroupName;
                if (HasFSMGroup(fsmGroupName))
                {
                    if (fsmGroupDict[fsmGroupName].HasFSM(fsmKey))
                        fsm.Shutdown();
                    else
                        fsmGroupDict[fsmGroupName].AddFSM(fsmKey, fsm);
                }
                else
                {
                    var fsmGroup = fsmGroupPool.Spawn();
                    fsmGroup.GroupName = fsmGroupName;
                    fsmGroup.AddFSM(fsmKey, fsm);
                    fsmGroupDict.Add(fsmGroupName, fsmGroup);
                }
            }
            return fsm;
        }
        /// <summary>
        /// 创建状态机；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="fsmGroupName">状态机组名，若为空，则不分配组</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(string name, T owner, string fsmGroupName, params FSMState<T>[] states)
           where T : class
        {
            Type type = typeof(T);
            FSM<T> fsm = default;
            var fsmKey = new TypeStringPair(type, name);
            if (string.IsNullOrEmpty(fsmGroupName))
            {
                if (fsmDict.ContainsKey(fsmKey))
                    throw new ArgumentException("FSMManager : FSM is exists" + type.ToString());
                fsm = FSM<T>.Create(name, owner, states);
                fsmDict.Add(fsmKey, fsm);
            }
            else
            {
                fsm = FSM<T>.Create(name, owner, states);
                fsm.GroupName = fsmGroupName;
                if (HasFSMGroup(fsmGroupName))
                {
                    if (fsmGroupDict[fsmGroupName].HasFSM(fsmKey))
                        fsm.Shutdown();
                    else
                        fsmGroupDict[fsmGroupName].AddFSM(fsmKey, fsm);
                }
                else
                {
                    var fsmGroup = fsmGroupPool.Spawn();
                    fsmGroup.GroupName = fsmGroupName;
                    fsmGroup.AddFSM(fsmKey, fsm);
                    fsmGroupDict.Add(fsmGroupName, fsmGroup);
                }
            }
            return fsm;
        }
        /// <summary>
        /// 销毁独立的状态机
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        public void DestoryFSM<T>()
           where T : class
        {
            DestoryFSM(typeof(T));
        }
        /// <summary>
        /// 销毁独立的状态机；
        /// </summary>
        /// <param name="type">拥有者类型</param>
        public void DestoryFSM(Type type)
        {
            FSMBase fsm = null;
            var fsmKey = new TypeStringPair(type);
            if (fsmDict.TryGetValue(fsmKey, out fsm))
            {
                fsm.Shutdown();
                fsmDict.Remove(fsmKey);
                var fsmGroupName = fsm.GroupName;

                if (!string.IsNullOrEmpty(fsmGroupName))
                {
                    fsmGroupDict.TryGetValue(fsmGroupName, out var group);
                    group.RemoveFSM(fsmKey);
                }
                fsm.GroupName = null;
            }
        }
        /// <summary>
        /// 获取状态机组
        /// </summary>
        /// <param name="fsmGroupName">状态机组名</param>
        /// <param name="fsmGroup">状态机组</param>
        /// <returns>是否存在组</returns>
        public bool PeekFSMGroup(string fsmGroupName, out IFSMGroup fsmGroup)
        {
            var rst = fsmGroupDict.TryGetValue(fsmGroupName, out var group);
            fsmGroup = group;
            return rst;
        }
        /// <summary>
        /// 移除状态机组；
        /// </summary>
        /// <param name="fsmGroupName">状态机组名</param>
        public void RemoveFSMGroup(string fsmGroupName)
        {
            if (!fsmGroupDict.TryRemove(fsmGroupName, out var fsmGroup))
                return;
            var dict = fsmGroup.FSMDict;
            foreach (var fsm in dict)
            {
                fsm.Value.GroupName = string.Empty;
            }
            fsmGroupPool.Despawn(fsmGroup);
        }
        /// <summary>
        /// 是否拥有指定类型的状态机集合；
        /// </summary>
        /// <returns>是否存在</returns>
        public bool HasFSMGroup(string fsmGroupName)
        {
            return fsmGroupDict.ContainsKey(fsmGroupName);
        }
        /// <summary>
        /// 为状态机设置组别；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名</param>
        /// <param name="fsmGroupName">状态机组名</param>
        public void SetFSMGroup<T>(string name, string fsmGroupName) where T : class
        {
            var fsmKey = new TypeStringPair(typeof(T), name);
            fsmDict.TryGetValue(fsmKey, out var fsm);
            if (!string.IsNullOrEmpty(fsm.GroupName))
            {
                fsmGroupDict.TryGetValue(fsm.GroupName, out var group);
                group?.RemoveFSM(fsmKey);
            }
            fsmGroupDict.TryGetValue(fsmGroupName, out var newGroup);
            newGroup?.AddFSM(fsmKey, fsm);
        }
        /// <summary>
        /// 为状态机设置组别；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="fsmGroupName">状态机组名</param>
        public void SetFSMGroup<T>(string fsmGroupName) where T : class
        {
            SetFSMGroup<T>(string.Empty, fsmGroupName);
        }
        /// <summary>
        /// 销毁所有状态机；
        /// </summary>
        public void DestoryAllFSM()
        {
            if (fsmDict.Count > 0)
            {
                foreach (var fsm in fsmDict)
                {
                    fsm.Value.Shutdown();
                    fsm.Value.GroupName = string.Empty;
                }
            }
            fsmCache.Clear();
            if (fsmGroupDict.Count > 0)
            {
                foreach (var fsmGroup in fsmGroupDict.Values)
                {
                    fsmGroup.Clear();
                }
            }
            fsmCache.Clear();
            fsmDict.Clear();
            fsmGroupDict.Clear();
            fsmGroupPool.Clear();
        }
        #endregion
        protected override void OnInitialization()
        {
            fsmGroupDict = new Dictionary<string, FSMGroup>();
            fsmDict = new Dictionary<TypeStringPair, FSMBase>();
        }
        [TickRefresh]
        void OnRefresh()
        {
            if (IsPause)
                return;
            if (fsmDict.Count > 0)
                foreach (var fsm in fsmDict)
                {
                    fsm.Value.OnRefresh();
                }
        }
    }
}