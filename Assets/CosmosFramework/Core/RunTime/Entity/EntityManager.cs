using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cosmos.Reference;
using UnityEngine;
namespace Cosmos.Entity
{
    /// <summary>
    /// 实例对象管理器；
    /// 管理例如角色身上的Gadget
    /// </summary>
    [Module]
    internal class EntityManager : Module// , IEntityManager
    {
        #region Properties
        public int EntityGroupCount { get { return entityGroupDict.Count; } }
        Action<EntityBase> entitySpawnSucceed;
        public event Action<IEntity> EntitySpawnSucceed
        {
            add { entitySpawnSucceed += value; }
            remove { entitySpawnSucceed -= value; }
        }
        IEntityHelper entityHelper;
        /// <summary>
        /// 所有实体列表
        /// </summary>
        public Dictionary<string, EntityGroup> entityGroupDict { get; private set; }
        Dictionary<int, IEntity> entityIdDict;
        IReferencePoolManager referencePoolManager;
        IResourceManager resourceManager;
        IMonoManager monoManager;
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            entityGroupDict = new Dictionary<string, EntityGroup>();
            entityIdDict = new Dictionary<int, IEntity>();
        }
        public override void OnPreparatory()
        {
            referencePoolManager = GameManager.GetModule<IReferencePoolManager>();
            resourceManager = GameManager.GetModule<IResourceManager>();
            monoManager = GameManager.GetModule<IMonoManager>();
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
        public void ActiveEntities(string entityGroupName)
        {

            entityGroupDict.TryGetValue(entityGroupName, out var group);
            group?.ActiveEntities();
        }
        public void DeactiveEntities(string entityGroupName)
        {
            entityGroupDict.TryGetValue(entityGroupName, out var group);
            group?.DeactiveEntities();
        }
        /// <summary>
        /// 注册EntityGroup (异步)；
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
                entityGroupDict.TryAdd(entityAssetInfo.EntityGroupName, new EntityGroup(entityAssetInfo.EntityGroupName, entityAsset));
            }
        }
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
                    entityGroupDict.TryAdd(entityAssetInfo.EntityGroupName, new EntityGroup(entityAssetInfo.EntityGroupName, entityAsset));
                });
            }
            return null;
        }
        public void DeregisterEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            entityGroupDict.Remove(entityGroupName);
        }
        public bool HasEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            return entityGroupDict.ContainsKey(entityGroupName);
        }
        public object GetGroupEntityAsset(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            return entityGroup;
        }
        public bool HasEntity(int entityId)
        {
            return entityIdDict.ContainsKey(entityId);
        }
        public void ActiveEntity(int entityId, string entityName, string entityGroupName, object userData)
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
                throw new ArgumentException($"Entity id '{entityId}' is already exist.");
            }
            if (!HasEntityGroup(entityGroupName))
            {
                throw new ArgumentException($"Entity group {entityGroupName}  is not exist.");
            }
            entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            var entity = entityGroup.GetEntity(entityName);
            if (entity != null)
            {
                entity.OnActive();
            }
            else
            {
                entity = entityHelper.CreateEntity(entityGroup.EntityAsset, entityGroup, userData);
                entity.OnActive();
            }
        }
        public void DeactiveEntity(int entityId)
        {
            if (entityHelper == null)
            {
                throw new ArgumentNullException("You must set entity helper first.");
            }
            if (HasEntity(entityId))
            {
                throw new ArgumentException($"Entity id '{entityId}' is already exist.");
            }
            entityIdDict.TryGetValue(entityId, out var entity);
            entityHelper.ReleaseEntity(entity.EntityAsset);
        }
        public void DeactiveEntity(IEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity is invalid.");
            }
            entity.OnDeactive();
        }
        /// <summary>
        /// 自动获取所有程序集中挂载EntityAssetAttribute的类，并注册EntityGroup (异步)；
        /// </summary>
        /// <returns>协程对象</returns>
        public Coroutine AutoRegisterEntityGroupsAsync()
        {
            return monoManager.StartCoroutine(EnumAutoRegisterEntityGroups());
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
        /// <summary>
        /// 生成实体对象
        /// </summary>
        /// <param name="entityGroupName">实体类型</param>
        /// <param name="entityAsset">实体资源对象</param>
        /// <param name="entityAssetName">实体名称</param>
        /// <returns>生成实体后的接口引用</returns>
        IEntity SpawnEntity(Type entityType, string entityGroupName, object entityAsset, string entityAssetName)
        {
            var entity = referencePoolManager.Spawn(entityType) as IEntity;
            if (entityGroupDict.TryGetValue(entityGroupName, out var group))
            {
                group.AddEntity(entity);
                entity.SetEntity(0, entityAssetName, entityAsset, group);
            }
            else
            {
                //group = new EntityGroup(entityGroupName);
            }
            entity.OnActive();
            return entity;
        }
        #endregion
    }
}