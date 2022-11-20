using System;
using System.Runtime.InteropServices;
namespace Cosmos.Entity
{
    [StructLayout(LayoutKind.Auto)]
    public struct EntityInfo : IEquatable<EntityInfo>
    {
        public string EntityName;
        public int EntityObjectCount;
        public int[] EntityObjectIds;

        public readonly static EntityInfo None = new EntityInfo();
        public EntityInfo(string entityName, int entityObjectCount, int[] entityObjectIds)
        {
            EntityName = entityName;
            EntityObjectCount = entityObjectCount;
            EntityObjectIds = entityObjectIds;
        }
        public bool Equals(EntityInfo other)
        {
            return other.EntityName == this.EntityName;
        }
    }
}
