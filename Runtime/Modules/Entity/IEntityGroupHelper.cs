namespace Cosmos.Entity
{
    public interface IEntityGroupHelper
    {
        void OnEntitySpawn(object entityInstance);
        void OnEntityDespawn(object entityInstance);
    }
}
