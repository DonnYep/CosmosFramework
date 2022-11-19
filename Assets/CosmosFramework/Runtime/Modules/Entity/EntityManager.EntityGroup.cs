using System;
using System.Collections.Generic;
using System.Linq;

namespace Cosmos.Entity
{
    internal partial class EntityManager
    {
        /// <summary>
        /// 实体组，实体组用于分组管理实体对象。
        /// </summary>
        private class EntityGroup : IEntityGroup
        {
            static readonly Pool<EntityGroup> entityGroupPool
                = new Pool<EntityGroup>
                (
                     () => { return new EntityGroup(); }
                    , g => { g.Release(); }
                );
            HashSet<string> entityNameHash = new HashSet<string>();
            public IEnumerable<string> EntityNames
            {
                get { return entityNameHash; }
            }
            public string EntityGroupName { get; private set; }
            public int EntityCount { get { return entityNameHash.Count; } }
            public bool HasEntity(string entityName)
            {
                if (string.IsNullOrEmpty(entityName))
                {
                    throw new ArgumentNullException("Entity asset name is invalid.");
                }
                return entityNameHash.Contains(entityName);
            }
            public bool AddEntity(string entityName)
            {
                return entityNameHash.Add(entityName);
            }
            public bool RemoveEntity(string entityName)
            {
                return entityNameHash.Remove(entityName);
            }
            public void Release()
            {
                EntityGroupName = string.Empty;
                entityNameHash.Clear();
            }
            public EntityGroupInfo GetEntityGroupInfo()
            {
                var entityGroupInfo = new EntityGroupInfo(EntityGroupName, entityNameHash.ToArray());
                return entityGroupInfo;
            }
            public static EntityGroup Create(string groupName)
            {
                var entityGroup = entityGroupPool.Spawn();
                entityGroup.EntityGroupName = groupName;
                return entityGroup;
            }
            public static void Release(EntityGroup entityGroup)
            {
                entityGroupPool.Despawn(entityGroup);
            }
            public static void ClearPool()
            {
                entityGroupPool.Clear();
            }
        }
    }
}