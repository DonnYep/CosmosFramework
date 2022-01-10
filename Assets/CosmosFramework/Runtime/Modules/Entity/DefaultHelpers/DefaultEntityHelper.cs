using Cosmos.Entity;
using UnityEngine;

namespace Cosmos
{
    public class DefaultEntityHelper : IEntityHelper
    {
        GameObject root = CosmosEntry.EntityManager.Instance();
        GameObject singleRoot;
        public DefaultEntityHelper()
        {
            singleRoot = new GameObject("SingleEntities");
            singleRoot.transform.SetAlignParent(root.transform);
        }
        public void AttachToParent(IEntity childEntity, IEntity parentEntity)
        {
            var childGo = childEntity.EntityInstance.CastTo<GameObject>();
            var parentGo = parentEntity.EntityInstance.CastTo<GameObject>();
            childGo.transform.SetParent(parentGo.transform);
        }
        public void DeatchFromParent(IEntity entity)
        {
            var childGo = entity.EntityInstance.CastTo<GameObject>();
            childGo.transform.SetParent(singleRoot.transform);
            childGo.transform.ResetLocalTransform();
        }
        public object InstantiateEntity(object entityAsset)
        {
            var resGo = entityAsset.CastTo<GameObject>();
            var go = GameObject.Instantiate(resGo);
            go.transform.SetParent(singleRoot.transform);
            return go;
        }

        public void ReleaseEntity(object entityInstance)
        {
            var go = entityInstance.As<GameObject>();
            GameObject.Destroy(go);
        }
    }
}
