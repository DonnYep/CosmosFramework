namespace Cosmos.Entity
{
    public class EntityRegisterFailureEventArgs : GameEventArgs
    {
        public string AssetName { get; private set; }
        public string EntityName { get; private set; }
        public string EntityGroupName { get; private set; }
        public override void Release()
        {
            AssetName = string.Empty;
            EntityName = string.Empty;
            EntityGroupName = string.Empty;
        }
        internal static EntityRegisterFailureEventArgs Create(string assetName, string entityName, string entityGroupName)
        {
            var eventArgs = ReferencePool.Acquire<EntityRegisterFailureEventArgs>();
            eventArgs.AssetName = assetName;
            eventArgs.EntityName = entityName;
            eventArgs.EntityGroupName = entityGroupName;
            return eventArgs;
        }
        internal static void Release(EntityRegisterFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
