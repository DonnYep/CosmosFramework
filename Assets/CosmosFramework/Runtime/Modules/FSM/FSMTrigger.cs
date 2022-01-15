namespace Cosmos.FSM
{
    public abstract  class FSMTrigger<T>
        where T:class
    {
        protected string triggerName;
        public string TriggerName { get { return triggerName; } }
        public abstract bool Handler(IFSM<T> fsm);
    }
}