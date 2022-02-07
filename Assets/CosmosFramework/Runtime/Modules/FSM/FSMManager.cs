using System.Collections.Generic;
using System;
using System.Linq;
using Cosmos;
namespace Cosmos.FSM
{
    //================================================
    /*
     * 1、状态机模块；
     */
    //================================================
    [Module]
    internal sealed partial class FSMManager : Module, IFSMManager
    {
        #region Properties
        /// <summary>
        /// 单个状态机
        /// </summary>
        Dictionary<TypeStringPair, FSMBase> fsmDict;
        /// <summary>
        /// 状态机群组集合
        /// </summary>
        Dictionary<string, FSMGroup> fsmGroupDict;
        List<FSMBase> fsmCache = new List<FSMBase>();
        public int FSMCount { get { return fsmDict.Count; } }
        #endregion

        #region Methods
        public FSMBase GetFSM<T>()
    where T : class
        {
            Type type = typeof(T).GetType();
            return GetFSM(type);
        }
        public FSMBase GetFSM(Type type)
        {
            fsmDict.TryGetValue(new TypeStringPair(type), out var fsm);
            return fsm;
        }
        public IList<FSMBase> GetAllFSMs()
        {
            return fsmDict.Values.ToArray();
        }
        public bool HasFSM<T>(string name)
           where T : class
        {
            return HasFSM(typeof(T), name);
        }
        public bool HasFSM(Type type, string name)
        {
            return fsmDict.ContainsKey(new TypeStringPair(type, name));
        }
        public IFSM<T> CreateFSM<T>(T owner, params FSMState<T>[] states) where T :class
        {
            return CreateFSM(string.Empty, owner, string.Empty, states);
        }
        /// <summary>
        /// 创建状态机；
        /// Individual表示创建的为群组FSM或者独立FSM，二者拥有不同的容器
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="owner">拥有者</param>
        /// <param name="groupName">是否为独立状态机</param>
        /// <param name="states">状态</param>
        /// <returns>创建成功后的状态机</returns>
        public IFSM<T> CreateFSM<T>(T owner, string groupName, params FSMState<T>[] states)
           where T : class
        {
            return CreateFSM(string.Empty, owner, groupName, states);
        }
        public IFSM<T> CreateFSM<T>(string name, T owner, string groupName, params FSMState<T>[] states)
           where T : class
        {
            Type type = typeof(T);
            FSM<T> fsm = default;
            var fsmKey = new TypeStringPair(type, name);
            if (string.IsNullOrEmpty(groupName))
            {
                if (fsmDict.ContainsKey(fsmKey))
                    throw new ArgumentException("FSMManager : FSM is exists" + type.ToString());
                fsm = FSM<T>.Create(name, owner, states);
                fsmDict.Add(fsmKey, fsm);
            }
            else
            {
                fsm = FSM<T>.Create(name, owner, states);
                fsm.GroupName = groupName;
                if (HasFSMGroup(groupName))
                {
                    if (fsmGroupDict[groupName].HasFSM(fsmKey))
                        fsm.Shutdown();
                    else
                        fsmGroupDict[groupName].AddFSM(fsmKey, fsm);
                }
                else
                {
                    var fsmPool = new FSMGroup();
                    fsmPool.AddFSM(fsmKey, fsm);
                    fsmGroupDict.Add(groupName, fsmPool);
                }
            }
            return fsm;
        }
        public IFSM<T> CreateFSM<T>(T owner, string groupName, IList<FSMState<T>> states)
           where T : class
        {
            return CreateFSM(string.Empty, owner, groupName, states);
        }
        public IFSM<T> CreateFSM<T>(string name, T owner, params FSMState<T>[] states) where T : class
        {
            return CreateFSM<T>(name, owner, string.Empty, states);
        }
        public IFSM<T> CreateFSM<T>(string name, T owner, IList<FSMState<T>> states)where T:class
        {
            return CreateFSM<T>(name, owner, string.Empty, states);
        }
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
        public IFSM<T> CreateFSM<T>(string name, T owner, string groupName, IList<FSMState<T>> states)
           where T : class
        {
            Type type = typeof(T);
            FSM<T> fsm = default;
            var fsmKey = new TypeStringPair(type, name);
            if (string.IsNullOrEmpty(groupName))
            {
                if (fsmDict.ContainsKey(fsmKey))
                    throw new ArgumentException("FSMManager : FSM is exists" + type.ToString());
                fsm = FSM<T>.Create(name, owner, states);
                fsmDict.Add(fsmKey, fsm);
            }
            else
            {
                fsm = FSM<T>.Create(name, owner, states);
                fsm.GroupName = groupName;
                if (HasFSMGroup(groupName))
                {
                    if (fsmGroupDict[groupName].HasFSM(fsmKey))
                        fsm.Shutdown();
                    else
                        fsmGroupDict[groupName].AddFSM(fsmKey, fsm);
                }
                else
                {
                    var fsmGroup = new FSMGroup();
                    fsmGroup.AddFSM(fsmKey, fsm);
                    fsmGroupDict.Add(groupName, fsmGroup);
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
        public void DestoryFSM(Type type)
        {
            FSMBase fsm = null;
            var fsmKey = new TypeStringPair(type);
            if (fsmDict.TryGetValue(fsmKey, out fsm))
            {
                fsm.Shutdown();
                fsmDict.Remove(fsmKey);
                var groupName = fsm.GroupName;

                if (!string.IsNullOrEmpty(groupName))
                {
                    fsmGroupDict.TryGetValue(groupName, out var group);
                    group.RemoveFSM(fsmKey);
                }
                fsm.GroupName = null;
            }
        }
        public bool PeekFSMGroup(string fsmGroupName, out IFSMGroup fsmGroup)
        {
            var rst = fsmGroupDict.TryGetValue(fsmGroupName, out var group);
            fsmGroup = group;
            return rst;
        }
        public void RemoveFSMGroup(string groupName)
        {
            if (!fsmGroupDict.TryRemove(groupName, out var fsmGroup))
                return;
            fsmGroup.AbortGroup();
        }
        /// <summary>
        /// 是否拥有指定类型的状态机集合
        /// </summary>
        /// <returns>是否存在</returns>
        public bool HasFSMGroup(string name)
        {
            return fsmGroupDict.ContainsKey(name);
        }
        /// <summary>
        /// 为一个状态机设置组别；
        /// </summary>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <param name="name">状态机名</param>
        /// <param name="fsmGroupName">状态机组别名</param>
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
        public void SetFSMGroup<T>(string fsmGroupName) where T : class
        {
            SetFSMGroup<T>(string.Empty, fsmGroupName);
        }
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