using Cosmos.Entity;
using UnityEngine;
namespace Cosmos
{
    public class DefaultEntityGroupHelper : IEntityGroupHelper
    {
        GameObject activeRoot;
        GameObject deactiveRoot;
        GameObject ActiveRoot
        {
            get
            {
                if (activeRoot == null)
                {
                    activeRoot = new GameObject("ActivedEntities");
                    activeRoot.transform.SetAlignParent(CosmosEntry.EntityManager.Instance().transform);
                }
                return activeRoot;
            }
        }
        GameObject DeactiveRoot
        {
            get
            {
                if (deactiveRoot == null)
                {
                    deactiveRoot = new GameObject("DeactivedEntities");
                    deactiveRoot.transform.SetAlignParent(CosmosEntry.EntityManager.Instance().transform);
                }
                return deactiveRoot;
            }
        }
        public void OnEntitySpawn(EntityObject entityObject)
        {
            var go = entityObject.gameObject;
            go.SetActive(true);
            go.transform.SetAlignParent(ActiveRoot.transform);
        }
        public void OnEntityDespawn(EntityObject entityObject)
        {
            var go = entityObject.gameObject;
            go.SetActive(false);
            go.transform.SetAlignParent(DeactiveRoot.transform);
        }
    }
}
