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
        ///<inheritdoc/>
        public int FSMCount { get { return fsmDict.Count; } }
        ///<inheritdoc/>
        public int FSMGroupCount { get { return fsmGroupDict.Count; } }
        #endregion

        #region Methods
        ///<inheritdoc/>
        public FSMBase GetFSM<T>()
    where T : class
        {
            Type type = typeof(T).GetType();
            return GetFSM(type);
        }
        ///<inheritdoc/>
        public FSMBase GetFSM(Type type)
        {
            fsmDict.TryGetValue(new TypeStringPair(type), out var fsm);
            return fsm;
        }
        ///<inheritdoc/>
        public IList<FSMBase> GetAllFSMs()
        {
            return fsmDict.Values.ToArray();
        }
        ///<inheritdoc/>
        public bool HasFSM<T>(string name)
           where T : class
        {
            return HasFSM(typeof(T), name);
        }
        ///<inheritdoc/>
        public bool HasFSM(Type type, string name)
        {
            return fsmDict.ContainsKey(new TypeStringPair(type, name));
        }
        ///<inheritdoc/>
        public IFSM<T> CreateFSM<T>(T owner, IList<FSMState<T>> states) where T : class
        {
            return CreateFSM(string.Empty, owner, string.Empty, states);
        }
        ///<inheritdoc/>
        public IFSM<T> CreateFSM<T>(T owner, params FSMState<T>[] states) where T :class
        {
            return CreateFSM(string.Empty, owner, string.Empty, states);
        }
        ///<inheritdoc/>
        public IFSM<T> CreateFSM<T>(T owner, string fsmGroupName, IList<FSMState<T>> states)
           where T : class
        {
            return CreateFSM(string.Empty, owner, fsmGroupName, states);
        }
        ///<inheritdoc/>
        public IFSM<T> CreateFSM<T>(T owner, string fsmGroupName, params FSMState<T>[] states)
           where T : class
        {
            return CreateFSM(string.Empty, owner, fsmGroupName, states);
        }
        ///<inheritdoc/>
        public IFSM<T> CreateFSM<T>(string name, T owner, IList<FSMState<T>> states)where T:class
        {
            return CreateFSM<T>(name, owner, string.Empty, states);
        }
        ///<inheritdoc/>
        public IFSM<T> CreateFSM<T>(string name, T owner, params FSMState<T>[] states) where T : class
        {
            return CreateFSM<T>(name, owner, string.Empty, states);
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public void DestoryFSM<T>()
           where T : class
        {
            DestoryFSM(typeof(T));
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public bool PeekFSMGroup(string fsmGroupName, out IFSMGroup fsmGroup)
        {
            var rst = fsmGroupDict.TryGetValue(fsmGroupName, out var group);
            fsmGroup = group;
            return rst;
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public bool HasFSMGroup(string fsmGroupName)
        {
            return fsmGroupDict.ContainsKey(fsmGroupName);
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public void SetFSMGroup<T>(string fsmGroupName) where T : class
        {
            SetFSMGroup<T>(string.Empty, fsmGroupName);
        }
        ///<inheritdoc/>
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