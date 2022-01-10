using Cosmos.Entity;
using UnityEngine;
namespace Cosmos
{
    public class DefaultEntityGroupHelper : IEntityGroupHelper
    {
        GameObject root = CosmosEntry.EntityManager.Instance();
        GameObject activeRoot;
        GameObject deactiveRoot;
        public DefaultEntityGroupHelper()
        {
            activeRoot = new GameObject("ActiveEntities");
            deactiveRoot = new GameObject("DeactiveEntities");
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
