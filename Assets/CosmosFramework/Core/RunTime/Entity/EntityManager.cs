using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace Cosmos.Entity
{
    /// <summary>
    /// 实例对象管理器；
    /// 管理例如角色身上的Gadget
    /// </summary>
    internal class EntityManager : Module<EntityManager>
    {
        #region Properties
        internal int EntityTypeCount { get { return entityTypeObjectDict.Count; } }
        Dictionary<Type, List<IEntityObject>> entityTypeObjectDict = new Dictionary<Type, List<IEntityObject>>();
        Type entityObjectType = typeof(IEntityObject);
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
                    entity.OnRefresh();
                }
            }
        }
        public override void OnTermination()
        {
            base.OnTermination();
            entityTypeObjectDict.Clear();
        }
        internal bool AddEntity<T>(T entity)
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            return AddEntity(type,entity);
        }
        internal bool AddEntity(Type type,IEntityObject entity)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            bool result = false;
            if (!entityTypeObjectDict.ContainsKey(type))
            {
                entityTypeObjectDict.Add(type, new List<IEntityObject>() { entity });
                result = true;
            }
            else
            {
                var set = entityTypeObjectDict[type];
                if (!set.Contains(entity))
                {
                    set.Add(entity);
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 回收单个实体对象;
        /// 若实体对象存在于缓存中，则移除。若不存在，则不做操作；
        /// </summary>
        /// <param name="entity">实体对象</param>
        internal void RecoveryEntity<T>(T entity)
              where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            if (entityTypeObjectDict.ContainsKey(type))
            {
                var set = entityTypeObjectDict[type];
                if (set.Contains(entity))
                    set.Remove(entity);
            }
            Facade.DespawnReference(entity);
        }
        /// <summary>
        /// 回收某一类型的实体对象
        /// </summary>
        /// <param name="type">实体类型</param>
        internal void RecoveryEntities(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var set = entityTypeObjectDict[type];
            int length = set.Count;
            for (int i = 0; i < length; i++)
            {
                Facade.DespawnReference(set[i]);
            }
        }
        // TODO  使用特性来管理实体资源加载
        internal T CreateEntity<T>(Action<float> loadingCallback, Action<IEntityObject> loadDoneCallback)
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            EntityResourceAttribute attribute = type.GetCustomAttribute<EntityResourceAttribute>();
            if (attribute != null)
            {
                if (HasEntityType(type))
                {
                }
            }
            return default;
        }
        internal int GetEntityCount<T>()
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            return GetEntityCount(type);
        }
        internal int GetEntityCount(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                return -1;
            return entityTypeObjectDict[type].Count;
        }
        internal T GetEntity<T>(Predicate<T> predicate)
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            T entity = default;
            var set = entityTypeObjectDict[type];
            int length = set.Count;
            for (int i = 0; i < length; i++)
            {
                if (predicate(set[i] as T))
                    return set[i] as T;
            }
            if (entity == null)
                throw new ArgumentNullException("EntityManager : can not register entity,entityObject is  empty");
            return entity;
        }
        internal IEntityObject GetEntity(Type type, Predicate<IEntityObject> predicate)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            IEntityObject entity = default;
            var set = entityTypeObjectDict[type];
            int length = set.Count;
            for (int i = 0; i < length; i++)
            {
                if (predicate(set[i]))
                    return set[i];
            }
            if (entity == null)
                throw new ArgumentNullException("EntityManager : can not register entity,entityObject is  empty");
            return entity;
        }
        internal ICollection<IEntityObject> GetEntityCollection<T>()
            where T : IEntityObject, new()
        {
            Type type = typeof(T);
            return GetEntityCollection(type);
        }
        internal ICollection<IEntityObject> GetEntityCollection(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var set = entityTypeObjectDict[type];
            return set;
        }
        internal IEntityObject[] GetEntities<T>()
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            return GetEntities(type);
        }
        internal IEntityObject[] GetEntities(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var set = entityTypeObjectDict[type];
            return set.ToArray();
        }
        internal List<IEntityObject> GetEntityList<T>()
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            return GetEntityList(type);
        }
        internal List<IEntityObject> GetEntityList(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var set = entityTypeObjectDict[type];
            return set;
        }
        internal void RegisterEntityType<T>()
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            RegisterEntityType(type);
        }
        internal void RegisterEntityType(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentException("EntityManager : can not register entity,entityObject already exist Entity Type : " + type.ToString());
            entityTypeObjectDict.Add(type, new List<IEntityObject>());
        }
        internal bool DeregisterEntityType<T>()
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            return DeregisterEntityType(type);
        }
        internal bool DeregisterEntityType(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            bool result = false;
            result = entityTypeObjectDict.Remove(type);
            return result;
        }
        internal bool ActiveEntity<T>(T entity)
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            return ActiveEntity(type, entity);
        }
        internal void ActiveEntity<T>(Predicate<T> predicate)
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            var set = entityTypeObjectDict[type];
            int length = set.Count;
            for (int i = 0; i < length; i++)
            {
                if (predicate(set[i] as T))
                    set[i].OnActive();
            }
        }
        internal bool ActiveEntity(Type type, IEntityObject entity)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            bool result = false;
            var set = entityTypeObjectDict[type];
            if (set.Contains(entity))
            {
                entity.OnActive();
                result = true;
            }
            else
                throw new ArgumentNullException("EntityManager : can not register entity, entity is  empty Entity : " + entity.ToString());
            return result;
        }
        internal void ActiveEntity(Type type, Predicate<IEntityObject> predicate)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var set = entityTypeObjectDict[type];
            int length = set.Count;
            for (int i = 0; i < length; i++)
            {
                if (predicate(set[i]))
                    set[i].OnActive();
            }
        }
        internal void ActiveEntities<T>()
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            ActiveEntities(type);
        }
        internal void ActiveEntities(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            var set = entityTypeObjectDict[type];
            int length = set.Count;
            for (int i = 0; i < length; i++)
                set[i].OnActive();
        }
        internal void DeactiveEntities<T>()
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            DeactiveEntities(type);
        }
        internal void DeactiveEntities(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                throw new ArgumentNullException("EntityManager : can not attach entity, entity type not exist !  Entity Type : " + type.ToString());
            var set = entityTypeObjectDict[type];
            int length = set.Count;
            for (int i = 0; i < length; i++)
                set[i].OnDeactive();
        }
        internal bool HasEntity<T>(Predicate<T> predicate)
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                return false;
            var set = entityTypeObjectDict[type];
            int length = set.Count;
            for (int i = 0; i < length; i++)
            {
                if (predicate(set[i] as T))
                    return true;
            }
            return false;
        }
        internal bool HasEntity(Type type,  Predicate<IEntityObject> predicate)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                return false;
            var set = entityTypeObjectDict[type];
            int length = set.Count;
            for (int i = 0; i < length; i++)
            {
                if (predicate(set[i]))
                    return true;
            }
            return false;
        }
        internal bool HasEntityType<T>()
            where T : class, IEntityObject, new()
        {
            Type type = typeof(T);
            return HasEntityType(type);
        }
        internal bool HasEntityType(Type type)
        {
            if (!entityObjectType.IsAssignableFrom(type))
                throw new TypeAccessException("EntityManager : type  is not sub class of EntityObject ! Type : " + type.ToString());
            if (!entityTypeObjectDict.ContainsKey(type))
                return false;
            return true;
        }
        #endregion
    }
}