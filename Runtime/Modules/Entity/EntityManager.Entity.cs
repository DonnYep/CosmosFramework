using System;
using UnityEngine;
using System.Collections.Generic;

namespace Cosmos.Entity
{
    internal sealed partial class EntityManager
    {
        private class Entity : IEquatable<Entity>
        {
            static readonly Pool<Entity> entityPool = new Pool<Entity>
                    (() => { return new Entity(); }, e => { e.Release(); });

            Pool<EntityObject> entityObjectPool;

            int spawnIndex;

            Dictionary<int, EntityObject> entityObjectDict;
            public string EntityName { get; set; }
            public Type EntityObjectType { get; set; }
            public string EntityGroupName { get; set; }
            public IEntityHelper EntityHelper { get; private set; }
            public GameObject EntityAsset { get; set; }
            public Entity()
            {
                entityObjectPool = new Pool<EntityObject>(GenerateEntityObject, DespawnEntityObject);
                entityObjectDict = new Dictionary<int, EntityObject>();
            }
            public EntityObject ShowEntityObject()
            {
                var entityObject = entityObjectPool.Spawn();
                entityObjectDict.TryAdd(entityObject.EntityObjectId, entityObject);
                return entityObject;
            }
            public bool HasEntityObject(int entityObjectId)
            {
                return entityObjectDict.ContainsKey(entityObjectId);
            }
            public void HideAllEntityObject()
            {
                foreach (var entityObject in entityObjectDict.Values)
                {
                    entityObjectPool.Despawn(entityObject);
                }
                entityObjectDict.Clear();
            }
            public void HideEntityObject(int entityObjectId)
            {
                if (entityObjectDict.TryRemove(entityObjectId, out var entityObject))
                {
                    entityObject.OnHide();
                    entityObjectPool.Despawn(entityObject);
                }
            }
            public bool Equals(Entity other)
            {
                return other.EntityName == this.EntityName;
            }
            public void Release()
            {
                EntityName = string.Empty;
                EntityObjectType = null;
                EntityGroupName = string.Empty;
                EntityAsset = null;
                spawnIndex = 0;
                entityObjectDict.Clear();
                foreach (var e in entityObjectPool)
                {
                    EntityHelper.ReleaseEntity(e);
                }
                entityObjectPool.Clear();
                EntityHelper = null;
            }
            public static Entity Create(string entityName, string entityGroupName, GameObject entityAsset, Type entityObjectType, IEntityHelper entityHelper)
            {
                var entity = entityPool.Spawn();
                entity.EntityName = entityName;
                entity.EntityAsset = entityAsset;
                entity.EntityGroupName = entityGroupName;
                entity.EntityObjectType = entityObjectType;
                entity.EntityHelper = entityHelper;
                return entity;
            }
            public static void Release(Entity entity)
            {
                entityPool.Despawn(entity);
            }
            EntityObject GenerateEntityObject()
            {
                var entityObject = EntityHelper.InstantiateEntity(EntityAsset, EntityObjectType);
                entityObject.OnInit(EntityName, spawnIndex++, EntityGroupName);
                return entityObject;
            }
            void DespawnEntityObject(EntityObject entityObject)
            {
                entityObject.OnRecycle();
            }
        }
    }
}
