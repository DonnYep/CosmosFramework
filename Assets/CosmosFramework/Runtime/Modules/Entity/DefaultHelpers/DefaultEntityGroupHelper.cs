using UnityEngine;
namespace Cosmos.Entity
{
    public class DefaultEntityGroupHelper : IEntityGroupHelper
    {
        GameObject root = CosmosEntry.EntityManager.Instance();
        GameObject activeRoot = new GameObject("ActiveEntities");
        GameObject deactiveRoot = new GameObject("DeactiveEntities");
        public DefaultEntityGroupHelper()
        {
            activeRoot.transform.SetAlignParent(root.transform);
            deactiveRoot.transform.SetAlignParent(root.transform);
        }
        public void OnEntitySpawn(object entityInstance)
        {
            var go = entityInstance.As<GameObject>();
            go.SetActive(true);
            go.transform.SetAlignParent(activeRoot.transform);
        }
        public void OnEntityDespawn(object entityInstance)
        {
            var go = entityInstance.As<GameObject>();
            go.SetActive(false);
            go.transform.SetAlignParent(deactiveRoot.transform);
        }
    }
}
