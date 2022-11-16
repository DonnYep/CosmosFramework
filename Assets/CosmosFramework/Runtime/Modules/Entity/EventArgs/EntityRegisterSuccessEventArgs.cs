namespace Cosmos.Entity
{
    public class EntityRegisterSuccessEventArgs : GameEventArgs
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
        internal static EntityRegisterSuccessEventArgs Create(string assetName, string entityName, string entityGroupName)
        {
            var eventArgs = ReferencePool.Acquire<EntityRegisterSuccessEventArgs>();
            eventArgs.AssetName = assetName;
            eventArgs.EntityName = entityName;
            eventArgs.EntityGroupName = entityGroupName;
            return eventArgs;
        }
        internal static void Release(EntityRegisterSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
