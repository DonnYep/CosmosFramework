using System;
using System.Runtime.InteropServices;
namespace Cosmos.Entity
{
    [StructLayout(LayoutKind.Auto)]
    public struct EntityGroupInfo : IEquatable<EntityGroupInfo>
    {
        public string EntityGroupName;
        public string[] EntityNames;
        public readonly static EntityGroupInfo None = new EntityGroupInfo();
        public EntityGroupInfo(string entityGroupName, string[] entityNames)
        {
            EntityGroupName = entityGroupName;
            EntityNames = entityNames;
        }
        public bool Equals(EntityGroupInfo other)
        {
            return other.EntityGroupName == this.EntityGroupName;
        }
    }
}
