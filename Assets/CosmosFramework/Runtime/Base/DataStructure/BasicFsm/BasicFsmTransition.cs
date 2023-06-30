namespace Cosmos
{
    public abstract class BasicFsmTransition<T>
        where T:class
    {
        public abstract bool Handler(BasicFsm<T> fsm);
    }
}
