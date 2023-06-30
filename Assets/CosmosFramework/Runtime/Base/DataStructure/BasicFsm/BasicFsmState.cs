using System.Collections.Generic;

namespace Cosmos
{
    public abstract class BasicFsmState<T> where T : class
    {
        List<BasicFsmTransition<T>> transitionList = new List<BasicFsmTransition<T>>();
        Dictionary<BasicFsmTransition<T>, BasicFsmState<T>> transitionStateDict
            = new Dictionary<BasicFsmTransition<T>, BasicFsmState<T>>();
        public void AddTransition(BasicFsmTransition<T> trigger, BasicFsmState<T> state)
        {
            if (transitionStateDict.ContainsKey(trigger))
                return;
            transitionStateDict.Add(trigger, state);
            transitionList.Add(trigger);
        }
        public void RemoveTransition(BasicFsmTransition<T> trigger)
        {
            if (!transitionStateDict.ContainsKey(trigger))
                return;
            transitionStateDict.Remove(trigger);
            transitionList.Remove(trigger);
        }
        public BasicFsmState<T> GetTransitionState(BasicFsmTransition<T> trigger)
        {
            if (transitionStateDict.ContainsKey(trigger))
                return transitionStateDict[trigger];
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
                    fsm.ChangeState(GetTransitionState(transitionList[i]).GetType());
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
