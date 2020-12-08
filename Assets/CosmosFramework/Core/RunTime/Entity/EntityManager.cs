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
        Dictionary<string, GameObject> entityAssetDict;
        Dictionary<int, IEntity> entityIdDict;
        IReferencePoolManager referencePoolManager;
        IResourceManager resourceManager;
        IMonoManager monoManager;
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            entityGroupDict = new Dictionary<string, EntityGroup>();
            entityAssetDict = new Dictionary<string, GameObject>();
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
        public bool RegisterEntityGroup(EntityAssetInfo entityAssetInfo)
        {
            if (string.IsNullOrEmpty(entityAssetInfo.EntityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            entityGroupDict.Add(entityAssetInfo.EntityGroupName, new EntityGroup(entityAssetInfo.EntityGroupName));
            return true;
        }
        public bool DeregisterEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            entityGroupDict.Remove(entityGroupName);
            return true;
        }
        public bool HasEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            return entityGroupDict.ContainsKey(entityGroupName);
        }
        public EntityGroup GetEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            return entityGroup;
        }
        public EntityGroup[] GetAllEntityGroups()
        {
            var entityGroupList = new List<EntityGroup>();
            entityGroupList.AddRange(entityGroupDict.Values);
            return entityGroupList.ToArray();
        }
        public bool HasEntity(int entityId)
        {
            return entityIdDict.ContainsKey(entityId);
        }
        public void ActiveEntity(int entityId, EntityAssetInfo entityAssetInfo)
        {
            if (entityHelper== null)
            {
                throw new ArgumentNullException("You must set entity helper first.");
            }
            if (entityAssetInfo== null)
            {
                throw new ArgumentNullException("entityAssetInfo is invalid..");
            }
            if (string.IsNullOrEmpty(entityAssetInfo.EntityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            if (HasEntity(entityId))
            {
                throw new ArgumentException($"Entity id '{entityId}' is already exist.");
            }
            resourceManager.LoadPrefabAsync(entityAssetInfo, (go) => 
            {
                entityAssetDict.AddOrUpdate(entityAssetInfo.EntityGroupName, go);
            });
        }
        //public Coroutine CreateEntity(Type type, string entityName, Action<IEntity> loadDoneAction, Action<float> loadingAction)
        //{
        //    var attribute = type.GetCustomAttribute<EntityAssetAttribute>();
        //    if (attribute != null)
        //    {
        //        if (registeredEntityType.Contains(type))
        //        {
        //            var assetInfo = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
        //            var entity = referencePoolManager.Spawn(type) as IEntity;
        //            return resourceManager.LoadPrefabAsync(assetInfo, entityGo =>
        //            {
        //                entity.SetEntity(0, entityName, entityGo);
        //                loadDoneAction?.Invoke(entity);
        //            }, loadingAction, true);
        //        }
        //        else
        //        {
        //            throw new ArgumentException($"EntityManager : type：{type} has not been register");
        //        }
        //    }
        //    return null;
        //}
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
                var entityAssetInfo = new EntityAssetInfo(ea.EntityGroupName, ea.AssetBundleName, ea.AssetPath, ea.ResourcePath);
               RegisterEntityGroup(entityAssetInfo);
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
        IEntity SpawnEntity(Type entityType,string entityGroupName, object entityAsset, string entityAssetName)
        {
            var entity = referencePoolManager.Spawn(entityType) as IEntity;
            if(entityGroupDict.TryGetValue(entityGroupName,out var group))
            {
                group.AddEntity(entity);
                entity.SetEntity(0, entityAssetName, entityAsset,group);
            }
            else
            {
                group = new EntityGroup(entityGroupName);
            }
            entity.OnActive();
            return entity;
        }
        #endregion
    }
}