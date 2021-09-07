using System.Collections;
namespace Cosmos.FSM
{
    public abstract  class FSMTrigger<T>
        where T:class
    {
        public FSMTrigger()
        {
            OnInitialization();
        }
        protected string triggerName;
        public string TriggerName { get { return triggerName; } }
        public abstract void OnInitialization();
        public abstract bool Handler(IFSM<T> fsm);
    }
}