using Cosmos.Entity;
using System;
using UnityEngine;

namespace Cosmos
{
    public class DefaultEntityHelper : IEntityHelper
    {
        public EntityObject InstantiateEntity(GameObject entityAsset, Type entityObjectType)
        {
            var go = GameObject.Instantiate(entityAsset);
            var entity = go.GetOrAddComponent(entityObjectType) as EntityObject;
            return entity;
        }
        public void ReleaseEntity(EntityObject entityObject)
        {
            var go = entityObject.As<GameObject>();
            GameObject.Destroy(go);
        }
    }
}
