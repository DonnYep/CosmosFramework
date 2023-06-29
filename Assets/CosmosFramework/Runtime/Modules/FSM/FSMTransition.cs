namespace Cosmos.FSM
{
    public abstract  class FSMTransition<T>
        where T:class
    {
        protected string transitionName;
        public string TransitionName { get { return transitionName; } }
        public abstract bool Handler(IFSM<T> fsm);
    }
}