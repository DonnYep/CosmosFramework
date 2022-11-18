using System.Collections.Generic;

namespace Cosmos.Entity
{
    public interface IEntityGroup
    {
        string EntityGroupName { get; }
        IEnumerable<string> EntityNames { get; }
        int EntityCount { get; }
        bool HasEntity(string entityName);
        bool AddEntity(string entityName);
        bool RemoveEntity(string entityName);
    }
}
