using Cosmos.Entity;
using UnityEngine;

namespace Cosmos
{
    public class DefaultEntityHelper : IEntityHelper
    {
        GameObject m_SingleRoot;
        GameObject SingleRoot
        {
            get
            {
                if (m_SingleRoot == null)
                {
                    m_SingleRoot = new GameObject("SingleEntities");
                    m_SingleRoot.transform.SetAlignParent(CosmosEntry.EntityManager.Instance().transform);
                }
                return m_SingleRoot;
            }
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
            childGo.transform.SetParent(SingleRoot.transform);
            childGo.transform.ResetLocalTransform();
        }
        public object InstantiateEntity(object entityAsset)
        {
            var resGo = entityAsset.CastTo<GameObject>();
            var go = GameObject.Instantiate(resGo);
            go.transform.SetParent(SingleRoot.transform);
            return go;
        }

        public void ReleaseEntity(object entityInstance)
        {
            var go = entityInstance.As<GameObject>();
            GameObject.Destroy(go);
        }
    }
}
