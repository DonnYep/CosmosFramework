using System.Collections.Generic;
namespace Cosmos.FSM
{
    internal sealed partial class FSMManager
    {
        /// <summary>
        /// 状态机容器
        /// </summary>
        private class FSMGroup : IFSMGroup
        {
            #region Properties
            string groupName;
            readonly Dictionary<TypeStringPair, FSMBase> fsmDict
                   = new Dictionary<TypeStringPair, FSMBase>();
            public Dictionary<TypeStringPair, FSMBase> FSMDict { get { return fsmDict; } }
            public string GroupName { get { return groupName; } set { groupName = value; } }
            public int FSMCount { get { return fsmDict.Count; } }
            #endregion

            #region Methods
            public FSMBase GetFSM(TypeStringPair fsmKey)
            {
                fsmDict.TryGetValue(fsmKey, out var fsm);
                return fsm;
            }
            public bool HasFSM(TypeStringPair fsmKey)
            {
                return fsmDict.ContainsKey(fsmKey);
            }
            internal void AddFSM(TypeStringPair fsmKey, FSMBase fsm)
            {
                fsmDict.Add(fsmKey, fsm);
                fsm.GroupName = groupName;
            }
            internal void RemoveFSM(TypeStringPair fsmKey)
            {
                fsmDict.Remove(fsmKey, out var fsm);
                fsm.GroupName = string.Empty;
            }
            internal void Clear()
            {
                fsmDict.Clear();
                groupName = string.Empty;
            }
            #endregion
        }
    }
}
