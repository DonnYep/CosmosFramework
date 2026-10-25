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
            //注意：EntityObject 是 MonoBehaviour，不能用 as GameObject 取句柄（恒为 null），
            //必须走 Handle（即 gameObject），否则 Destroy(null) 等于什么也没做。
            if (entityObject == null)
                return;
            GameObject.Destroy(entityObject.Handle);
        }
    }
}
