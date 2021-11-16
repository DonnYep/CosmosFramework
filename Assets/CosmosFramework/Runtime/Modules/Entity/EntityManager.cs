using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    internal class EntityManager : Module, IEntityManager
    {
        #region Properties
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

        #endregion
        #region Methods
        public void SetHelper(IEntityHelper helper)
        {
            if (helper == null)
                throw new ArgumentNullException("Entity helper is valid !");
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
                var pool = new EntityGroup(entityAssetInfo.EntityGroupName, entityAsset, entityHelper);
                if (entityAssetInfo.UseObjectPool)
                {
                    var objectPool = objectPoolManager.RegisterObjectPool(entityAssetInfo.EntityGroupName, entityAsset);
                    pool.AssignObjectPool(objectPool);
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
                    var entityAssetInfo = new EntityAssetInfo(att.EntityGroupName, att.AssetBundleName, att.AssetPath,att.UseObjectPool) ;
                    RegisterEntityGroup(entityAssetInfo);
                }
            }
        }
        /// <summary>
        /// 注册EntityGroup (同步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <typeparam name="T">挂载EntityAssetAttribute特性的类型</typeparam>
        public void RegisterEntityGroup<T>() where T : class
        {
            RegisterEntityGroup(typeof(T));
        }

        /// <summary>
        /// 注册EntityGroup (异步)；
        /// </summary>
        /// <param name="entityAssetInfo">实体对象信息</param>
        public async Task RegisterEntityGroupAsync(EntityAssetInfo entityAssetInfo)
        {
            if (string.IsNullOrEmpty(entityAssetInfo.EntityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            var result = HasEntityGroup(entityAssetInfo.EntityGroupName);
            if (!result)
            {
                await resourceManager.LoadPrefabAsync(entityAssetInfo, (entityAsset) =>
                {
                    var pool = new EntityGroup(entityAssetInfo.EntityGroupName, entityAsset, entityHelper);
                    if (entityAssetInfo.UseObjectPool)
                    {
                        var objectPool = objectPoolManager.RegisterObjectPool(entityAssetInfo.EntityGroupName, entityAsset);
                        pool.AssignObjectPool(objectPool);
                    }
                    entityGroupDict.TryAdd(entityAssetInfo.EntityGroupName, pool);
                });
            }
        }
        /// <summary>
        /// 注册EntityGroup (异步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <param name="entityType">挂载EntityAssetAttribute特性的类型</param>
        /// <returns>协程对象</returns>
        public async Task RegisterEntityGroupAsync(Type entityType)
        {
            await EnumRegisterEntityGroupAsync(entityType);
        }
        /// <summary>
        /// 注册EntityGroup (异步)；
        /// 特性 EntityAssetAttribute 有效；
        /// </summary>
        /// <typeparam name="T">挂载EntityAssetAttribute特性的类型</typeparam>
        /// <returns>协程对象</returns>
        public async Task RegisterEntityGroupAsync<T>() where T : class
        {
            await RegisterEntityGroupAsync(typeof(T));
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
        /// 为一个实体组设置根节点；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="root">根节点</param>
        /// <returns>是否设置成功</returns>
        public bool SetEntityGroupRoot(string entityGroupName, IEntity root)
        {
            if (string.IsNullOrEmpty(entityGroupName))
                throw new ArgumentNullException("Entity group name is invalid.");
            if (root == null)
                throw new ArgumentNullException($"Root entity is invalidl.");
            if (!entityGroupDict.TryGetValue(entityGroupName, out var entityGroup))
                return false;
            entityGroup.AssignEntityRoot(root);
            return true;
        }
        /// <summary>
        /// 为一个实体组设置根节点；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="root">根节点</param>
        /// <returns>是否设置成功</returns>
        public bool SetEntityGroupRoot(string entityGroupName, object root)
        {
            if (string.IsNullOrEmpty(entityGroupName))
                throw new ArgumentNullException("Entity group name is invalid.");
            if (root == null)
                throw new ArgumentNullException($"Root entity is invalidl.");
            if (!entityGroupDict.TryGetValue(entityGroupName, out var entityGroup))
                return false;
            entityGroup.AssignEntityRoot(root);
            return true;
        }
        /// <summary>
        /// 激活&添加实体对象；
        /// </summary>
        /// <param name="entityId">自定义的实体Id</param>
        /// <param name="entityName">自定义实体名称</param>
        /// <param name="entityGroupName">注册的实体组名称</param>
        /// <returns>实体对象</returns>
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
                if (entityGroup.ObjectPool!=null)
                {
                    entityInstance = entityGroup.ObjectPool.Spawn();
                }
                else
                {
                    entityInstance = entityHelper.InstantiateEntity(entityGroup.EntityAsset);
                }
                entity = Entity.Create(entityId, entityName, entityInstance, entityGroup);

                entityGroup.AddEntity(entity);
            }
            entityIdDict.AddOrUpdate(entityId, entity);
            return entity;
        }
        /// <summary>
        ///  激活&添加实体对象；
        /// </summary>
        /// <param name="entityName">自定义实体名称</param>
        /// <param name="entityGroupName">注册的实体组名称</param>
        /// <returns>实体对象</returns>
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
                if (entityGroup.ObjectPool != null)
                {
                    entityInstance = entityGroup.ObjectPool.Spawn();
                }
                else
                {
                    entityInstance = entityHelper.InstantiateEntity(entityGroup.EntityAsset);
                }
                entity = Entity.Create(entityId, entityName, entityInstance, entityGroup);
                entityGroup.AddEntity(entity);
            }
            entityIdDict.AddOrUpdate(entityId, entity);
            return entity;
        }
        /// <summary>
        /// 激活&添加实体对象；
        ///  显示指定实体组的任意一个；
        /// </summary>
        /// <param name="entityGroupName">注册的实体组名称</param>
        /// <returns>实体对象</returns>
        public IEntity ShowEntity(string entityGroupName)
        {
            return ShowEntity(null, entityGroupName);
        }
        /// <summary>
        /// 失活&移除实体对象
        /// </summary>
        /// <param name="entityId">实体Id</param>
        public void DeactiveEntity(int entityId)
        {
            if (entityHelper == null)
                throw new ArgumentNullException("You must set entity helper first.");
            if (!HasEntity(entityId))
                throw new ArgumentException($"Entity id '{entityId}' is not exist.");
            entityIdDict.Remove(entityId, out var entity);
            DeactiveEntityObject(entity);
        }
        /// <summary>
        /// 失活&移除实体对象
        /// </summary>
        /// <param name="entity">实体对象接口索引</param>
        public void DeactiveEntity(IEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Entity is invalid.");
            DeactiveEntity(entity.EntityId);
        }
        /// <summary>
        /// 失活&移除实体组；
        /// 并不注销实体组；
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
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
            entityHelper.DeatchFromParent(childEntity);
            entityHelper.AttachToParent(childEntity, parentEntity);
            parentEntity.CastTo<Entity>().AddChild(childEntity);
            childEntity.CastTo<Entity>().SetParent(parentEntity);
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
            entityHelper.DeatchFromParent(childEntity);
            childEntity.CastTo<Entity>().ClearParent();
            parentEntity.CastTo<Entity>().RemoveChild(childEntity);
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
        public async Task AutoRegisterEntityGroupsAsync()
        {
            await EnumAutoRegisterEntityGroups();
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
                var attribute = entityAttributes[i];
                if (!HasEntityGroup(attribute.EntityGroupName))
                {
                    var entityAssetInfo = new EntityAssetInfo(attribute.EntityGroupName, attribute.AssetBundleName, attribute.AssetPath);
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
                    var entityAssetInfo = new EntityAssetInfo(att.EntityGroupName, att.AssetBundleName, att.AssetPath);
                    resourceManager.LoadPrefabAsync(entityAssetInfo, (entityAsset) =>
                    {
                        var entityGroup = new EntityGroup(entityAssetInfo.EntityGroupName, entityAsset, entityHelper);
                        if (entityAssetInfo.UseObjectPool)
                        {
                            var objectPool = objectPoolManager.RegisterObjectPool(entityAssetInfo.EntityGroupName, entityAsset);
                            entityGroup.AssignObjectPool(objectPool);
                        }
                        entityGroupDict.TryAdd(entityAssetInfo.EntityGroupName, entityGroup);
                    });
                }
            }
            yield return null;
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
            var entityGroup = entity.EntityGroup;
            if (entityGroup != null)
            {
                entityGroup.ObjectPool.Despawn(entity.EntityInstance);
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