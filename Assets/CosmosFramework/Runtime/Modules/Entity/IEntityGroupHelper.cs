namespace Cosmos.Entity
{
    public interface IEntityGroupHelper
    {
        void OnEntitySpawn(EntityObject entityObject);
        void OnEntityDespawn(EntityObject  entityObject);
    }
}
