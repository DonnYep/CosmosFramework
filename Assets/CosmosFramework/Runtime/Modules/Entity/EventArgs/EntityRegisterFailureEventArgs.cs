namespace Cosmos.Entity
{
    public class EntityRegisterFailureEventArgs : GameEventArgs
    {
        public override void Release()
        {
        }
        internal static EntityRegisterFailureEventArgs Create()
        {
            var eventArgs = ReferencePool.Acquire<EntityRegisterFailureEventArgs>();
            return eventArgs;
        }
        internal static void Release(EntityRegisterFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
