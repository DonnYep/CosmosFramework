using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace Cosmos.Entity
{
    /// <summary>
    /// 实例对象管理器；
    /// 管理例如角色身上的Gadget
    /// </summary>
    [Module]
    internal class EntityManager : Module, IEntityManager
    {
        #region Properties
        public int EntityGroupCount { get { return entityGroupDict.Count; } }
        IEntityHelper entityHelper;
        /// <summary>
        /// 所有实体列表
        /// </summary>
        Dictionary<string, EntityGroup> entityGroupDict;
        Dictionary<int, IEntity> entityIdDict;
        IObjectPoolManager objectPoolManager;
        IResourceManager resourceManager;
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            entityGroupDict = new Dictionary<string, EntityGroup>();
            entityIdDict = new Dictionary<int, IEntity>();
        }
        public override void OnPreparatory()
        {
            objectPoolManager = GameManager.GetModule<IObjectPoolManager>();
            resourceManager = GameManager.GetModule<IResourceManager>();
        }
        public override void OnRefresh()
        {
            if (IsPause)
                return;
        }
        public void SetHelper(IEntityHelper helper)
        {
            this.entityHelper = helper;
        }

        /// <summary>
        /// 注册EntityGroup (同步)；
        /// </summary>
        /// <param name="entityAssetInfo">实体对象信息</param>
        public void RegisterEntityGroup(EntityAssetInfo entityAssetInfo)
        {
            if (string.IsNullOrEmpty(entityAssetInfo.EntityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            var result = HasEntityGroup(entityAssetInfo.EntityGroupName);
            if (!result)
            {
                var entityAsset = resourceManager.LoadPrefab(entityAssetInfo);
                var pool = new EntityGroup(entityAssetInfo.EntityGroupName, entityAsset);
                if (entityAssetInfo.UseObjectPool)
                {
                    var objectPool= objectPoolManager.RegisterObjectPool(entityAssetInfo.EntityGroupName, entityAsset);
                    pool.SetObjectPool(objectPool);
                }
                entityGroupDict.TryAdd(entityAssetInfo.EntityGroupName, pool);
            }
        }
        /// <summary>
        /// 注册EntityGroup (同步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <param name="entityType">挂载EntityAssetAttribute特性的类型</param>
        public void RegisterEntityGroup(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("Entity type is invalid.");
            }
            var attributes = entityType.GetCustomAttributes<EntityAssetAttribute>().ToArray();
            if (attributes.Length <= 0)
            {
                throw new ArgumentNullException($"Entity: type {entityType} attribute is invalid.");
            }
            var length = attributes.Length;
            for (int i = 0; i < length; i++)
            {
                if (!HasEntityGroup(attributes[i].EntityGroupName))
                {
                    var att = attributes[i];
                    var entityAssetInfo = new EntityAssetInfo(att.EntityGroupName, att.AssetBundleName, att.AssetPath, att.ResourcePath) {  UseObjectPool=att.UseObjectPool};
                    RegisterEntityGroup(entityAssetInfo);
                }
            }
        }
        /// <summary>
        /// 注册EntityGroup (同步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <typeparam name="T">挂载EntityAssetAttribute特性的类型</typeparam>
        public void RegisterEntityGroup<T>()where T:class
        {
            RegisterEntityGroup(typeof(T));
        }

        /// <summary>
        /// 注册EntityGroup (异步)；
        /// </summary>
        /// <param name="entityAssetInfo">实体对象信息</param>
        public Coroutine RegisterEntityGroupAsync(EntityAssetInfo entityAssetInfo)
        {
            if (string.IsNullOrEmpty(entityAssetInfo.EntityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            var result = HasEntityGroup(entityAssetInfo.EntityGroupName);
            if (!result)
            {
                return resourceManager.LoadPrefabAsync(entityAssetInfo, (entityAsset) =>
                {
                    var pool = new EntityGroup(entityAssetInfo.EntityGroupName, entityAsset);
                    if (entityAssetInfo.UseObjectPool)
                    {
                        var objectPool = objectPoolManager.RegisterObjectPool(entityAssetInfo.EntityGroupName, entityAsset);
                        pool.SetObjectPool(objectPool);
                    }
                    entityGroupDict.TryAdd(entityAssetInfo.EntityGroupName, pool);
                });
            }
            return null;
        }
        /// <summary>
        /// 注册EntityGroup (异步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <param name="entityType">挂载EntityAssetAttribute特性的类型</param>
        /// <returns>协程对象</returns>
        public Coroutine RegisterEntityGroupAsync(Type entityType)
        {
            return Utility.Unity.StartCoroutine( EnumRegisterEntityGroupAsync(entityType));
        }
        /// <summary>
        /// 注册EntityGroup (异步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <typeparam name="T">挂载EntityAssetAttribute特性的类型</typeparam>
        /// <returns>协程对象</returns>
        public Coroutine RegisterEntityGroupAsync<T>()where T:class
        {
            return RegisterEntityGroupAsync(typeof(T));
        }

        /// <summary>
        /// 注销EntityGroup；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        public void DeregisterEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            entityGroupDict.Remove(entityGroupName);
        }
        /// <summary>
        /// 注销EntityGroup;
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <typeparam name="T">挂载EntityAssetAttribute特性的类型</typeparam>
        public void DeregisterEntityGroup<T>()
        {
            DeregisterEntityGroup(typeof(T));
        }
        /// <summary>
        /// 注销EntityGroup;
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <param name="entityType">挂载EntityAssetAttribute特性的类型</param>
        public void DeregisterEntityGroup(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("Entity type is invalid.");
            }
            var attributes = entityType.GetCustomAttributes<EntityAssetAttribute>().ToArray();
            if (attributes.Length <= 0)
            {
                throw new ArgumentNullException($"Entity: type {entityType} attribute is invalid.");
            }
            var length = attributes.Length;
            for (int i = 0; i < length; i++)
            {
                DeregisterEntityGroup(attributes[i].EntityGroupName);
            }
        }
        /// <summary>
        /// 是否存在实体组；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>是否存在</returns>
        public bool HasEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            return entityGroupDict.ContainsKey(entityGroupName);
        }
        /// <summary>
        /// 获得实体组的实体资源；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>是否存在</returns>
        public object GetGroupEntityAsset(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            return entityGroup;
        }
        /// <summary>
        /// 是否存在实体；
        /// </summary>
        /// <param name="entityId">自定义的实体id;</param>
        /// <returns>是否存在</returns>
        public bool HasEntity(int entityId)
        {
            return entityIdDict.ContainsKey(entityId);
        }
        /// <summary>
        /// 失活&移除实体组‘；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        public void DeactiveEntities(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            if (!HasEntityGroup(entityGroupName))
            {
                throw new ArgumentException($"Entity group {entityGroupName}  is not exist.");
            }
            entityGroupDict.TryGetValue(entityGroupName, out var group);
            var childEntities = group.GetAllChildEntities();
            group.ClearChildEntities();
            if (group.ObjectPool != null)
            {
                var length = childEntities.Length;
                for (int i = 0; i < length; i++)
                {
                    group.ObjectPool.Despawn(childEntities[i].EntityInstance);
                    ReferencePool.Release(childEntities[i].CastTo<Entity>());
                }
            }
            else
            {
                var length = childEntities.Length;
                for (int i = 0; i < length; i++)
                {
                    entityHelper.DespawnEntityInstance(childEntities[i].EntityInstance);
                    ReferencePool.Release(childEntities[i].CastTo<Entity>());
                }
            }
        }
        /// <summary>
        /// 激活&添加实体对象；
        /// </summary>
        /// <param name="entityId">自定义的实体Id</param>
        /// <param name="entityName">实体名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="entityRoot">实体对象的父节点</param>
        public void ActiveEntity(int entityId, string entityName, string entityGroupName)
        {
            if (entityHelper == null)
            {
                throw new ArgumentNullException("You must set entity helper first.");
            }
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            if (HasEntity(entityId))
            {
                throw new ArgumentException($"Entity '{entityId}' is already exist.");
            }
            if (!HasEntityGroup(entityGroupName))
            {
                throw new ArgumentException($"Entity group {entityGroupName}  is not exist.");
            }
            entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            var entity = entityGroup.GetEntity(entityName);
            if (entity == null)
            {
                entity = ReferencePool.Accquire<Entity>();
                object entityInstance = null;
                if (entityGroup.ObjectPool != null)
                {
                    entityInstance= entityGroup.ObjectPool.Spawn();
                }
                else
                {
                    entityInstance = entityHelper.SpanwEntityInstance(entityGroup.EntityAsset);
                }
                entity.SetEntity(entityId, entityName, entityInstance, entityGroup);
                entityGroup.AddEntity(entity);
            }
            entityIdDict.TryAdd(entityId, entity);
        }
        /// <summary>
        /// 失活&移除实体对象
        /// </summary>
        /// <param name="entityId">实体Id</param>
        public void DeactiveEntity(int entityId)
        {
            if (entityHelper == null)
            {
                throw new ArgumentNullException("You must set entity helper first.");
            }
            if (!HasEntity(entityId))
            {
                throw new ArgumentException($"Entity id '{entityId}' is not exist.");
            }
            entityIdDict.TryRemove(entityId, out var entity);
            if (entity.EntityGroup.ObjectPool != null)
            {
                entity.EntityGroup.ObjectPool.Despawn(entity.EntityInstance);
            }
            else
            {
                entityHelper.DespawnEntityInstance(entity.EntityInstance);
            }
            ReferencePool.Release(entity.CastTo<Entity>());
        }
        /// <summary>
        /// 失活&移除实体对象
        /// </summary>
        /// <param name="entity">实体对象接口索引</param>
        public void DeactiveEntity(IEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity is invalid.");
            }
            DeactiveEntity(entity.EntityId);
        }

        /// <summary>
        /// 挂载子实体到父实体；
        /// </summary>
        /// <param name="childEntityId">子实体Id</param>
        /// <param name="parentEntityId">父实体Id</param>
        public void AttachEntity(int childEntityId, int parentEntityId)
        {
            if (childEntityId == parentEntityId)
            {
                throw new ArgumentException($"Can not attach entity when child entity id equals to parent entity id '{parentEntityId}'.");
            }
            if (!HasEntity(childEntityId))
            {
                throw new ArgumentNullException($"Can not find child entity'{childEntityId}'.");
            }
            if (!HasEntity(parentEntityId))
            {
                throw new ArgumentNullException($"Can not find parent entity'{parentEntityId}'.");
            }
            entityIdDict.TryGetValue(childEntityId, out var childEntity);
            entityIdDict.TryGetValue(parentEntityId, out var parentEntity);
            var latestParentEntity = childEntity.ParentEntity;
            entityHelper.Attach(childEntity, parentEntity);
            entityHelper.Deatch(childEntity, latestParentEntity);
            parentEntity.CastTo<Entity>().OnAttached(childEntity);
            childEntity.CastTo<Entity>().OnAttachTo(parentEntity);
        }
        /// <summary>
        /// 挂载子实体到父实体；
        /// </summary>
        /// <param name="childEntityId">子实体Id</param>
        /// <param name="parentEntity">父实体</param>
        public void AttachEntity(int childEntityId, IEntity parentEntity)
        {
            if (parentEntity == null)
            {
                throw new ArgumentNullException($"Parent  entity is invalidl.");
            }
            AttachEntity(childEntityId, parentEntity.EntityId);
        }
        /// <summary>
        /// 挂载子实体到父实体；
        /// </summary>
        /// <param name="childEntity">子实体</param>
        /// <param name="parentEntity">父实体</param>
        public void AttachEntity(IEntity childEntity, IEntity parentEntity)
        {
            if (childEntity == null)
            {
                throw new ArgumentNullException($"Child entity is invalidl.");
            }
            if (parentEntity == null)
            {
                throw new ArgumentNullException($"Parent  entity is invalidl.");
            }
            AttachEntity(childEntity.EntityId, parentEntity.EntityId);
        }

        /// <summary>
        /// 解除子实体的挂载；
        /// 断开与父实体的连接；
        /// </summary>
        /// <param name="childEntityId">需要与父实体断开的子实体Id</param>
        public void DeatchEntity(int childEntityId)
        {
            if (!HasEntity(childEntityId))
            {
                throw new ArgumentNullException($"Can not find child entity'{childEntityId}'.");
            }
            entityIdDict.TryGetValue(childEntityId, out var childEntity);
            var parentEntity = childEntity.ParentEntity;
            entityHelper.Deatch(childEntity, parentEntity);
            childEntity.CastTo<Entity>().OnDetachFrom(parentEntity);
            parentEntity.CastTo<Entity>().OnDetached(childEntity);
        }
        /// <summary>
        /// 解除子实体的挂载；
        /// 断开与父实体的连接；
        /// </summary>
        /// <param name="childEntity">需要与父实体断开的子实体</param>
        public void DeatchEntity(IEntity childEntity)
        {
            if (childEntity == null)
            {
                throw new ArgumentNullException("Child entity is invalid");
            }
            DeatchEntity(childEntity.EntityId);
        }

        /// <summary>
        /// 解除实体的子实体挂载；
        /// </summary>
        /// <param name="parentEntityId">父实体Id</param>
        public void DeatchChildEntities(int parentEntityId)
        {
            if (!HasEntity(parentEntityId))
            {
                throw new ArgumentNullException($"Can not find child entity'{parentEntityId}'.");
            }
            entityIdDict.TryGetValue(parentEntityId, out var parentEntity);
            var childLength = parentEntity.ChildEntityCount;
            for (int i = 0; i < childLength; i++)
            {
                try
                {
                    DeatchEntity(childLength);
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError(e);
                }
            }
        }
        /// <summary>
        /// 解除实体的子实体挂载；
        /// </summary>
        /// <param name="parentEntity">父实体</param>
        public void DeatchChildEntities(IEntity parentEntity)
        {
            if (parentEntity == null)
            {
                throw new ArgumentNullException($"Parent  entity is invalidl.");
            }
            DeatchChildEntities(parentEntity.EntityId);
        }

        /// <summary>
        /// 自动获取所有程序集中挂载EntityAssetAttribute的类，并注册EntityGroup (异步)；
        /// </summary>
        /// <returns>协程对象</returns>
        public Coroutine AutoRegisterEntityGroupsAsync()
        {
            return Utility.Unity.StartCoroutine(EnumAutoRegisterEntityGroups());
        }
        IEnumerator EnumAutoRegisterEntityGroups()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var entityAttributes = new List<EntityAssetAttribute>();
            var length = assemblies.Length;
            for (int i = 0; i < length; i++)
            {
                var attributes = Utility.Assembly.GetAttributesInAssembly<EntityAssetAttribute>(assemblies[i]);
                entityAttributes.AddRange(attributes);
            }
            var attLength = entityAttributes.Count;
            for (int i = 0; i < attLength; i++)
            {
                var ea = entityAttributes[i];
                if (!HasEntityGroup(ea.EntityGroupName))
                {
                    var entityAssetInfo = new EntityAssetInfo(ea.EntityGroupName, ea.AssetBundleName, ea.AssetPath, ea.ResourcePath);
                    RegisterEntityGroup(entityAssetInfo);
                }
            }
            yield return null;
        }
        IEnumerator EnumRegisterEntityGroupAsync(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("Entity type is invalid.");
            }
            var attributes = entityType.GetCustomAttributes<EntityAssetAttribute>().ToArray();
            if (attributes.Length <= 0)
            {
                throw new ArgumentNullException($"Entity: type {entityType} attribute is invalid.");
            }
            var length = attributes.Length;
            for (int i = 0; i < length; i++)
            {
                if (!HasEntityGroup(attributes[i].EntityGroupName))
                {
                    var att = attributes[i];
                    var entityAssetInfo = new EntityAssetInfo(att.EntityGroupName, att.AssetBundleName, att.AssetPath, att.ResourcePath);
                    resourceManager.LoadPrefabAsync(entityAssetInfo, (entityAsset) =>
                    {
                        var pool = new EntityGroup(entityAssetInfo.EntityGroupName, entityAsset);
                        if (entityAssetInfo.UseObjectPool)
                        {
                            var objectPool = objectPoolManager.RegisterObjectPool(entityAssetInfo.EntityGroupName, entityAsset);
                            pool.SetObjectPool(objectPool);
                        }
                        entityGroupDict.TryAdd(entityAssetInfo.EntityGroupName, pool);
                    });
                }
            }
            yield return null;
        }
        #endregion
    }
}