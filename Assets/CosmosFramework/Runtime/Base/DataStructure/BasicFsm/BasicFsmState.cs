using System;
using System.Collections.Generic;

namespace Cosmos
{
    public abstract class BasicFsmState<T> where T : class
    {
        List<BasicFsmTransition<T>> transitionList = new List<BasicFsmTransition<T>>();
        Dictionary<BasicFsmTransition<T>, Type> transitionStateDict
            = new Dictionary<BasicFsmTransition<T>, Type>();
        public void AddTransition(BasicFsmTransition<T> transition, Type stateType)
        {
            if (transitionStateDict.ContainsKey(transition))
                return;
            if (!this.GetType().IsAssignableFrom(stateType))
                throw new ArgumentException($"State type {stateType.FullName} is invalid !");
            transitionStateDict.Add(transition, stateType);
            transitionList.Add(transition);
        }
        public void RemoveTransition(BasicFsmTransition<T> transition)
        {
            if (!transitionStateDict.ContainsKey(transition))
                return;
            transitionStateDict.Remove(transition);
            transitionList.Remove(transition);
        }
        public Type GetTransitionStateType(BasicFsmTransition<T> transition)
        {
            if (transitionStateDict.ContainsKey(transition))
                return transitionStateDict[transition];
            return null;
        }
        public virtual void OnInit(BasicFsm<T> fsm) { }
        public abstract void OnEnter(BasicFsm<T> fsm);
        public abstract void OnLogic(BasicFsm<T> fsm);
        public virtual void UpdateTransition(BasicFsm<T> fsm)
        {
            for (int i = 0; i < transitionList.Count; i++)
            {
                if (transitionList[i].Handler(fsm))
                {
                    fsm.ChangeState(GetTransitionStateType(transitionList[i]));
                    return;
                }
            }
        }
        public abstract void OnExit(BasicFsm<T> fsm);
        public virtual void OnDestroy(BasicFsm<T> fsm) { }
        public virtual void Clear()
        {
            transitionList.Clear();
            transitionStateDict.Clear();
        }
    }
}
