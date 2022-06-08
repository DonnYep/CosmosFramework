using Cosmos.ObjectPool;
using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Cosmos.Entity
{
    //================================================
    /*
     * 使用步骤：
     * 1. SetHelper 设置IEntityHelper 帮助体
     * 2.注册实体组RegisterEntityGroup
     * 3.ShowEntity，并接收返回值
     * 4.DeactiveEntity，回收实体
     * 5.注销实体组DeregisterEntityGroup
     * 可选：
     * 1.SetEntityGroupRoot设置根节点
     * 2.DeactiveEntityGroup失活组
     * 
     * 
     * 1、实体对象管理模块。
     * 
     * 2、实体组表示为，一组使用同一个预制体资源的组别。
     * 
     * 3、独立实体表示为，一个拥有独立预制体资源的对象。
     * 
     * 4、若使用pool生成，则由对象池生成。
     * 
     * 5、若不使用pool，则IEntityHelper生成实体。
     * 
     * 6、实体对象被实例化后才会存在与此。若被回收，则清理实体相关缓存。
     * 
     */
    //================================================
    [Module]
    internal partial class EntityManager : Module, IEntityManager
    {
        #region Properties
        ///<inheritdoc/>
        public int EntityGroupCount { get { return entityGroupDict.Count; } }
        IEntityHelper entityHelper;
        /// <summary>
        /// 实体组；
        /// </summary>
        Dictionary<string, EntityGroup> entityGroupDict;
        /// <summary>
        /// 所有生成的实体，不论是归组或独立，都被记录在此；
        /// </summary>
        Dictionary<int, IEntity> entityIdDict;
        IObjectPoolManager objectPoolManager;
        IResourceManager resourceManager;
        IEntityGroupHelper defaultEntityGroupHelper;
        #endregion
        #region Methods
        ///<inheritdoc/>
        public void SetHelper(IEntityHelper helper)
        {
            if (helper == null)
                throw new ArgumentNullException("Entity helper is valid !");
            this.entityHelper = helper;
        }
        ///<inheritdoc/>
        public void SetDefautEntityGroupHelper(IEntityGroupHelper entityGroupHelper)
        {
            if (entityGroupHelper == null)
                throw new ArgumentNullException("EntityGroupHelper is valid !");
            defaultEntityGroupHelper = entityGroupHelper;
        }
        ///<inheritdoc/>
        public async Task RegisterEntityGroupAsync(EntityAssetInfo entityAssetInfo, IEntityGroupHelper entityGroupHelper = null)
        {
            if (string.IsNullOrEmpty(entityAssetInfo.EntityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            var result = HasEntityGroup(entityAssetInfo.EntityGroupName);
            if (!result)
            {
                await resourceManager.LoadPrefabAsync(entityAssetInfo.AssetName, (entityAsset) =>
                {
                    entityGroupHelper = entityGroupHelper != null ? entityGroupHelper : defaultEntityGroupHelper;
                    var entityGroup = EntityGroup.Create(entityAssetInfo.EntityGroupName, entityAsset, entityGroupHelper);
                    if (entityAssetInfo.UseObjectPool)
                    {
                        var objectPool = objectPoolManager.RegisterObjectPool(entityAssetInfo.EntityGroupName, entityAsset);
                        entityGroup.AssignObjectPool(objectPool);
                    }
                    entityGroupDict.TryAdd(entityAssetInfo.EntityGroupName, entityGroup);
                });
            }
        }
        ///<inheritdoc/>
        public void DeregisterEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            entityGroupDict.Remove(entityGroupName, out var entityGroup);
            objectPoolManager.DeregisterObjectPool(entityGroup.ObjectPool.ObjectPoolName);
            EntityGroup.Release(entityGroup);

        }
        ///<inheritdoc/>
        public bool HasEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            return entityGroupDict.ContainsKey(entityGroupName);
        }
        ///<inheritdoc/>
        public object GetGroupEntityAsset(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            return entityGroup;
        }
        ///<inheritdoc/>
        public bool HasEntity(int entityId)
        {
            return entityIdDict.ContainsKey(entityId);
        }
        ///<inheritdoc/>
        public IEntity ShowEntity(int entityId, string entityName, string entityGroupName)
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
                object entityInstance = null;
                if (entityGroup.UseObjectPool)
                {
                    entityInstance = entityGroup.ObjectPool.Spawn();
                    entityGroup.EntityGroupHelper.OnEntitySpawn(entityInstance);
                }
                else
                {
                    entityInstance = entityHelper.InstantiateEntity(entityGroup.EntityAsset);
                }
                entity = Entity.Create(entityId, entityName, entityInstance, entityGroup);

                entityGroup.AddEntity(entity);
            }
            entityIdDict[entityId] = entity;
            return entity;
        }
        ///<inheritdoc/>
        public IEntity ShowEntity(string entityName, string entityGroupName)
        {
            if (entityHelper == null)
            {
                throw new ArgumentNullException("You must set entity helper first.");
            }
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            if (!HasEntityGroup(entityGroupName))
            {
                throw new ArgumentException($"Entity group {entityGroupName}  is not exist.");
            }
            entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            var entityId = GenerateEntityId();
            var entity = entityGroup.GetEntity(entityId);
            if (entity == null)
            {
                object entityInstance = null;
                if (entityGroup.UseObjectPool)
                {
                    entityInstance = entityGroup.ObjectPool.Spawn();
                    entityGroup.EntityGroupHelper.OnEntitySpawn(entityInstance);
                }
                else
                {
                    entityInstance = entityHelper.InstantiateEntity(entityGroup.EntityAsset);
                }
                entity = Entity.Create(entityId, entityName, entityInstance, entityGroup);
                entityGroup.AddEntity(entity);
            }
            entityIdDict[entityId] = entity;
            return entity;
        }
        ///<inheritdoc/>
        public IEntity ShowEntity(string entityGroupName)
        {
            return ShowEntity(null, entityGroupName);
        }
        ///<inheritdoc/>
        public void DeactiveEntity(int entityId)
        {
            if (entityHelper == null)
                throw new ArgumentNullException("You must set entity helper first.");
            if (!HasEntity(entityId))
                throw new ArgumentException($"Entity id '{entityId}' is not exist.");
            entityIdDict.Remove(entityId, out var entity);
            DeactiveEntityObject(entity);
        }
        ///<inheritdoc/>
        public void DeactiveEntity(IEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity is invalid.");
            DeactiveEntity(entity.EntityId);
        }
        ///<inheritdoc/>
        public void DeactiveEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
                throw new ArgumentNullException("Entity group name is invalid.");
            if (!HasEntityGroup(entityGroupName))
                throw new ArgumentException($"Entity group {entityGroupName}  is not exist.");
            entityGroupDict.TryGetValue(entityGroupName, out var group);
            var childEntities = group.GetAllChildEntities();
            var length = childEntities.Length;
            for (int i = 0; i < length; i++)
            {
                DeactiveEntityObject(childEntities[i]);
            }
        }
        ///<inheritdoc/>
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
            entityHelper.DeatchFromParent(childEntity);
            entityHelper.AttachToParent(childEntity, parentEntity);
            parentEntity.CastTo<Entity>().AddChild(childEntity);
            childEntity.CastTo<Entity>().SetParent(parentEntity);
        }
        ///<inheritdoc/>
        public void AttachEntity(int childEntityId, IEntity parentEntity)
        {
            if (parentEntity == null)
            {
                throw new ArgumentNullException($"Parent  entity is invalidl.");
            }
            AttachEntity(childEntityId, parentEntity.EntityId);
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public void DeatchEntity(int childEntityId)
        {
            if (!HasEntity(childEntityId))
            {
                throw new ArgumentNullException($"Can not find child entity'{childEntityId}'.");
            }
            entityIdDict.TryGetValue(childEntityId, out var childEntity);
            var parentEntity = childEntity.ParentEntity;
            entityHelper.DeatchFromParent(childEntity);
            childEntity.CastTo<Entity>().ClearParent();
            parentEntity.CastTo<Entity>().RemoveChild(childEntity);
        }
        ///<inheritdoc/>
        public void DeatchEntity(IEntity childEntity)
        {
            if (childEntity == null)
            {
                throw new ArgumentNullException("Child entity is invalid");
            }
            DeatchEntity(childEntity.EntityId);
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public void DeatchChildEntities(IEntity parentEntity)
        {
            if (parentEntity == null)
            {
                throw new ArgumentNullException($"Parent  entity is invalidl.");
            }
            DeatchChildEntities(parentEntity.EntityId);
        }
        protected override void OnInitialization()
        {
            entityGroupDict = new Dictionary<string, EntityGroup>();
            entityIdDict = new Dictionary<int, IEntity>();
        }
        protected override void OnPreparatory()
        {
            objectPoolManager = GameManager.GetModule<IObjectPoolManager>();
            resourceManager = GameManager.GetModule<IResourceManager>();
            defaultEntityGroupHelper = new DefaultEntityGroupHelper();
            entityHelper = new DefaultEntityHelper();
        }
        int GenerateEntityId()
        {
            var id = Utility.Algorithm.RandomRange(0, int.MaxValue);
            while (entityIdDict.ContainsKey(id))
            {
                id = Utility.Algorithm.RandomRange(0, int.MaxValue);
            }
            return id;
        }
        void DeactiveEntityObject(IEntity entity)
        {
            while (entity.ChildEntityCount > 0)
            {
                var childEntity = entity.GetChildEntity();
                DeactiveEntity(childEntity);
            }
            var entityGroup = (EntityGroup)entity.EntityGroup;
            if (entityGroup != null)
            {
                entityGroup.ObjectPool.Despawn(entity.EntityInstance.CastTo<UnityEngine.GameObject>());
                entityGroup.EntityGroupHelper.OnEntityDespawn(entity.EntityInstance);
                entityGroup.RemoveEntity(entity);
            }
            else
            {
                entityHelper.ReleaseEntity(entity.EntityInstance);
            }
            Entity.Release(entity.CastTo<Entity>());
        }
        #endregion
    }
}