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
            var childGo = childEntity.EntityObject.gameObject;
            var parentGo = parentEntity.EntityObject.gameObject;
            childGo.transform.SetParent(parentGo.transform);
        }
        public void DeatchFromParent(IEntity entity)
        {
            var childGo = entity.EntityObject.gameObject;
            childGo.transform.SetParent(SingleRoot.transform);
            childGo.transform.ResetLocalTransform();
        }
        public EntityObject InstantiateEntity(EntityObject entityObjectAsset)
        {
            var resGo = entityObjectAsset;
            var go = GameObject.Instantiate(resGo);
            go.transform.SetParent(SingleRoot.transform);
            return go;
        }
        public void ReleaseEntity(EntityObject entityObject)
        {
            GameObject.Destroy(entityObject);
        }
    }
}
