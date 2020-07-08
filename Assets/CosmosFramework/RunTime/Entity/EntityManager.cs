using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Entity
{
    /// <summary>
    /// 实例对象管理器；
    /// 管理例如角色身上的Gadget
    /// </summary>
    internal class EntityManager : Module<EntityManager>
    {
        #region Properties
        public int EntityTypeCount { get { return entityTypeObjectDict.Count; } }
        Dictionary<Type, Dictionary<int, IEntityObject>> entityTypeObjectDict = new Dictionary<Type, Dictionary<int, IEntityObject>>();
        HashSet<Type> entityObjectTypeHashSet = new HashSet<Type>();
        Type entityObjectType = typeof(EntityObject);
        List<IEntityObject> entityObjectCacheSet = new List<IEntityObject>();
        #endregion

        #region Methods
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            foreach (var entityDict in entityTypeObjectDict)
            {
                foreach (var entity in entityDict.Value)
                {
                    entity.Value.OnRefresh();
                }
            }
        }
        public override void OnTermination()
        {
            base.OnTermination();
            entityTypeObjectDict.Clear();
        }
        public int GetEntityCount<T>()
            where T :EntityObject, new()
        {
            Type type = typeof(T);
            return GetEntityCount(type);
        }
        public int GetEntityCount(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                return -1;
            return entityTypeObjectDict[type].Count;
        }
        public void AttachEntity<T>(int entityID, Transform target)
            where T : EntityObject,new ()
        {
            Type type = typeof(T);
            AttachEntity(type, entityID, target);
        }
        public void AttachEntity(Type type, int entityID, Transform target)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            if (target == null)
                throw new ArgumentNullException("EntityManager : can not attach entity");
            var dict = entityTypeObjectDict[type];
            if (dict.ContainsKey(entityID))
            {
                dict[entityID].OnAttach(target);
            }
            else
                throw new ArgumentNullException("EntityManager : can not attach entity, entity id not exist !  Entity ID : " + entityID);
        }
        public void DetachEntity<T>(int entityID)
            where T : EntityObject
        {
            Type type = typeof(T);
            DetachEntity(type,entityID);
        }
        public void DetachEntity(Type type, int entityID)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var dict = entityTypeObjectDict[type];
            if (dict.ContainsKey(entityID))
            {
                dict[entityID].OnDetach();
            }
            else
                throw new ArgumentNullException("EntityManager : can not attach entity, entity id not exist !  Entity ID : " + entityID);
        }
        public void AddEntity<T>(int entityID, IEntityObject entity)
            where T : EntityObject, new()
        {
            Type type = typeof(T);
            AddEntity(type,entityID,entity);
        }
        public void AddEntity(Type type, int entityID, IEntityObject entity)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (entity == null)
                throw new ArgumentNullException("EntityManager : can not register entity,entityObject is  empty");
            if (!entityObjectTypeHashSet.Contains(type))
                entityObjectTypeHashSet.Add(type);
            if (entityTypeObjectDict.ContainsKey(type))
            {
                var dict = entityTypeObjectDict[type];
                if (!dict.ContainsKey(entityID))
                {
                    dict.Add(entityID, entity);
                }
                else
                    throw new ArgumentNullException("EntityManager : can not register entity,entityObject  already exist !");
            }
            else
            {
                entityTypeObjectDict.Add(type, new Dictionary<int, IEntityObject>());
                entityTypeObjectDict[type].Add(entityID, entity);
            }
        }
        /// <summary>
        /// 移除实体对象
        /// 实体对象被移除后，自动被引用池回收
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entityID">实体ID</param>
        /// <returns>返回是否移除成功</returns>
        public bool RemoveEntity<T>(int entityID)
            where T : EntityObject, new()
        {
            Type type = typeof(T);
            return RemoveEntity(type,entityID);
        }
        public bool RemoveEntity(Type type, int entityID)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            bool result = false;
            var dict = entityTypeObjectDict[type];
            if (dict.ContainsKey(entityID))
            {
                var entity = dict[entityID];
                dict.Remove(entityID);
                Facade.DespawnReference(entity);
                result = true;
            }
            else
                throw new ArgumentNullException("EntityManager : can not register entity,entityObject is  empty");
            return result;
        }
        /// <summary>
        /// 移除entity，并获得这个entity下的gameobject对象;
        /// 返回是否移除Entity对象成功，GameObject entity可能为空
        /// </summary>
        /// <typeparam name="T">entity类型</typeparam>
        /// <param name="entityID">实体ID</param>
        /// <param name="entity">实体的对象，可能为空</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveEntity<T>( int entityID, out GameObject entity)
            where T : EntityObject, new()
        {
            Type type = typeof(T);
            bool result = false;
            RemoveEntity(type, entityID, out entity);
            return result;
        }
        public bool RemoveEntity(Type type, int entityID,out GameObject entity)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            bool result = false;
            var dict = entityTypeObjectDict[type];
            if (dict.ContainsKey(entityID))
            {
                var entityObject= dict[entityID];
                entity = entityObject.GetEntity();
                dict.Remove(entityID);
                Facade.DespawnReference(entityObject);
                result = true;
            }
            else
                throw new ArgumentNullException("EntityManager : can not register entity,entityObject is  empty");
            return result;
        }
        public IEntityObject GetEntity<T>(int entityID)
            where T : EntityObject, new()
        {
            Type type = typeof(T);
            return GetEntity(type,entityID);
        }
        public IEntityObject GetEntity(Type type, int entityID)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            IEntityObject entity = default;
            var dict = entityTypeObjectDict[type];
            if (dict.ContainsKey(entityID))
            {
                entity = dict[entityID];
            }
            else
                throw new ArgumentNullException("EntityManager : can not register entity,entityObject is  empty");
            return entity;
        }
        public ICollection<IEntityObject> GetEntityCollection<T>()
            where T:EntityObject, new()
        {
            Type type = typeof(T);
            return GetEntityCollection(type);
        }
        public ICollection< IEntityObject> GetEntityCollection(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var dict = entityTypeObjectDict[type];
            return dict.Values;
        }
        public IEntityObject[] GetEntities<T>()
            where T:EntityObject, new()
        {
            Type type = typeof(T);
            return GetEntities(type);
        }
        public IEntityObject[] GetEntities(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var dict = entityTypeObjectDict[type];
            entityObjectCacheSet.Clear();
            entityObjectCacheSet.AddRange(dict.Values);
            return entityObjectCacheSet.ToArray();
        }
        public List<IEntityObject> GetEntityList<T>()
            where T : EntityObject,new ()
        {
            Type type = typeof(T);
            return GetEntityList(type);
        }
        public List<IEntityObject> GetEntityList(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var dict = entityTypeObjectDict[type];
            entityObjectCacheSet.Clear();
            entityObjectCacheSet.AddRange(dict.Values);
            return entityObjectCacheSet;
        }
        public bool SetEntity<T>(int entityID, GameObject entityObject)
            where T : EntityObject,new()
        {
            Type type = typeof(T);
            return SetEntity(type, entityID, entityObject);
        }
        public bool SetEntity(Type type, int entityID, GameObject entityObject)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            bool result = false;
            var dict = entityTypeObjectDict[type];
            if (dict.ContainsKey(entityID))
            {
                dict[entityID].SetEntity(entityObject);
                result = true;
            }
            else
                throw new ArgumentNullException("EntityManager : can not register entity,entityObject is  empty");
            return result;
        }
        public void RegisterEntity<T>()
            where T : EntityObject,new()
        {
            Type type = typeof(T);
            RegisterEntity(type);
        }
        public void RegisterEntity(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentException("EntityManager : can not register entity,entityObject already exist Entity Type : " + type.ToString());
            entityTypeObjectDict.Add(type, new Dictionary<int, IEntityObject>());
            entityObjectTypeHashSet.Add(type);
        }
        public bool DeregisterEntity<T>()
            where T : EntityObject,new ()
        {
            Type type = typeof(T);
            return DeregisterEntity(type);
        }
        public bool DeregisterEntity(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            bool result = false;
            result = entityTypeObjectDict.Remove(type);
            entityObjectTypeHashSet.Remove(type);
            return result;
        }
        public bool ActiveEntity<T>(int entityID)
            where T : EntityObject,new()
        {
            Type type = typeof(T);
            return ActiveEntity(type,entityID);
        }
        public bool ActiveEntity(Type type, int entityID)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            bool result = false;
            var dict = entityTypeObjectDict[type];
            if (dict.ContainsKey(entityID))
            {
                dict[entityID].OnActive();
                result = true;
            }
            else
                throw new ArgumentNullException("EntityManager : can not register entity, entityID is  empty .EntityID : " + entityID);
            return result;
        }
        public void ActiveEntities<T>()
            where T : EntityObject,new()
        {
            Type type = typeof(T);
            ActiveEntities(type);
        }
        public void ActiveEntities(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            var dict = entityTypeObjectDict[type];
            foreach (var entity in dict)
            {
                entity.Value.OnActive();
            }
        }
        public bool DeactiveEntity<T>(int entityID)
            where T : EntityObject,new()
        {
            Type type = typeof(T);
           return DeactiveEntity(type, entityID);
        }
        public bool DeactiveEntity(Type type, int entityID)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            bool result = false;
            var dict = entityTypeObjectDict[type];
            if (dict.ContainsKey(entityID))
            {
                dict[entityID].OnDeactive();
                result = true;
            }
            else
                throw new ArgumentNullException("EntityManager : can not register entity, entityID is  empty .EntityID : " + entityID);
            return result;
        }
        public void DeactiveEntities<T>()
            where T:EntityObject,new()
        {
            Type type = typeof(T);
           DeactiveEntities(type);
        }
        public void DeactiveEntities(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var dict = entityTypeObjectDict[type];
            foreach (var entity in dict)
            {
                entity.Value.OnDeactive();
            }
        }
        public bool HasEntity<T>(int entityID)
            where T : EntityObject,new()
        {
            Type type = typeof(T);
            return HasEntity(type, entityID);
        }
        public bool HasEntity(Type type, int entityID)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                return false;
            bool result = false;
            var dict = entityTypeObjectDict[type];
            if (dict.ContainsKey(entityID))
            {
                dict[entityID].OnDeactive();
                result = true;
            }
            return result;
        }
        public bool HasEntityType<T>()
    where T : EntityObject,new()
        {
            Type type = typeof(T);
            return HasEntityType(type);
        }
        public bool HasEntityType(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityObjectTypeHashSet.Contains(type))
                return false;
            return true;
        }
        #endregion
    }
}