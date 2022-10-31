namespace Cosmos.Entity
{
    public class EntityRegisterSuccessEventArgs : GameEventArgs
    {
        public override void Release()
        {
        }
        internal static EntityRegisterSuccessEventArgs Create()
        {
            var eventArgs = ReferencePool.Acquire<EntityRegisterSuccessEventArgs>();
            return eventArgs;
        }
        internal static void Release(EntityRegisterSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
