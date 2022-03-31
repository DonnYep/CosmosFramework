using Cosmos.ObjectPool;
namespace Cosmos.Entity
{
    public interface IEntityGroup
    {
        string EntityGroupName { get; }
        int EntityCount { get; }
        object EntityAsset { get; }
        IEntity EntityRoot { get; }
        IObjectPool ObjectPool { get; }
        IEntityGroupHelper EntityGroupHelper { get; }
        bool HasEntity(string entityName);
        bool HasEntity(int entityId);
        IEntity GetEntity(int entityId);
        IEntity GetEntity(string entityName);
        IEntity[] GetEntities(string entityName);
        IEntity[] GetAllChildEntities();
        void ClearChildEntities();
    }
}
