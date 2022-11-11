using Cosmos.ObjectPool;
using System;
using System.Collections.Generic;
namespace Cosmos.Entity
{
    internal partial class EntityManager
    {
        private class EntityGroup : IEntityGroup
        {
            static readonly Pool<EntityGroup> entityGroupPool
                = new Pool<EntityGroup>(() => { return new EntityGroup(); }, g => { g.Release(); });

            public string EntityGroupName { get; private set; }
            public int EntityCount { get { return entityLinkedList.Count; } }
            public  EntityObject EntityObjectAsset { get; private set; }
            public IEntity EntityRoot { get; private set; }
            public object EntityRootInstance { get; private set; }
            public IObjectPool ObjectPool { get; private set; }
            public bool UseObjectPool { get; private set; }
            public IEntityGroupHelper EntityGroupHelper { get; private set; }

            LinkedList<IEntity> entityLinkedList = new LinkedList<IEntity>();

            public void AssignObjectPool(IObjectPool objectPool)
            {
                this.ObjectPool = objectPool;
                this.UseObjectPool = true;
            }
            public bool HasEntity(string entityName)
            {
                if (string.IsNullOrEmpty(entityName))
                {
                    throw new ArgumentNullException("Entity asset name is invalid.");
                }
                foreach (IEntity entity in entityLinkedList)
                {
                    if (entity.EntityName == entityName)
                    {
                        return true;
                    }
                }
                return false;
            }
            public bool HasEntity(int entityId)
            {
                foreach (IEntity entity in entityLinkedList)
                {
                    if (entity.EntityId == entityId)
                    {
                        return true;
                    }
                }
                return false;
            }
            public IEntity GetEntity(int entityId)
            {
                foreach (IEntity entity in entityLinkedList)
                {
                    if (entity.EntityId == entityId)
                    {
                        return entity;
                    }
                }
                return null;
            }
            public IEntity GetEntity(string entityName)
            {
                if (string.IsNullOrEmpty(entityName))
                {
                    throw new ArgumentNullException("Entity asset name is invalid.");
                }
                foreach (IEntity entity in entityLinkedList)
                {
                    if (entity.EntityName == entityName)
                    {
                        return entity;
                    }
                }
                return null;
            }
            public IEntity[] GetEntities(string entityName)
            {
                List<IEntity> entityList = new List<IEntity>();
                if (string.IsNullOrEmpty(entityName))
                {
                    throw new ArgumentNullException("Entity asset name is invalid.");
                }
                foreach (IEntity entity in entityLinkedList)
                {
                    if (entity.EntityName == entityName)
                    {
                        entityList.Add(entity);
                    }
                }
                return entityList.ToArray();
            }
            public IEntity[] GetAllChildEntities()
            {
                List<IEntity> entityList = new List<IEntity>();
                foreach (IEntity entity in entityLinkedList)
                {
                    entityList.Add(entity);
                }
                return entityList.ToArray();
            }
            public void AddEntity(IEntity entity)
            {
                entityLinkedList.AddLast(entity);
            }
            public void RemoveEntity(IEntity entity)
            {
                entityLinkedList.Remove(entity);
            }
            public void ClearChildEntities()
            {
                entityLinkedList.Clear();
            }
            public void Release()
            {
                EntityGroupName = string.Empty;
                entityLinkedList.Clear();
                EntityObjectAsset = null;
                EntityRoot = null;
                ObjectPool = null;
                UseObjectPool = false;
                EntityGroupHelper = null;
            }
            public static EntityGroup Create(string name, EntityObject entityObjectAsset, IEntityGroupHelper  entityGroupHelper)
            {
                var entityGroup = entityGroupPool.Spawn();
                entityGroup.EntityGroupName = name;
                entityGroup.EntityObjectAsset = entityObjectAsset;
                entityGroup.EntityGroupHelper= entityGroupHelper;
                return entityGroup;
            }
            public static void Release(EntityGroup entityGroup)
            {
                entityGroupPool.Despawn(entityGroup);
            }
        }
    }
}