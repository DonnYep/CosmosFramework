using System;

namespace Cosmos.Entity
{
    public class EntityRegisterSuccessEventArgs : GameEventArgs
    {
        public string AssetName { get; private set; }
        public string EntityName { get; private set; }
        public string EntityGroupName { get; private set; }
        public Type EntityObjectType { get; private set; }
        public override void Release()
        {
            AssetName = string.Empty;
            EntityName = string.Empty;
            EntityGroupName = string.Empty;
            EntityObjectType = null;
        }
        internal static EntityRegisterSuccessEventArgs Create(string assetName, string entityName, string entityGroupName, Type entityObjectType)
        {
            var eventArgs = ReferencePool.Acquire<EntityRegisterSuccessEventArgs>();
            eventArgs.AssetName = assetName;
            eventArgs.EntityName = entityName;
            eventArgs.EntityGroupName = entityGroupName;
            eventArgs.EntityObjectType = entityObjectType;
            return eventArgs;
        }
        internal static void Release(EntityRegisterSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
