using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Cosmos.Entity
{
    //================================================
    /*
     * 实体模块：框架内唯一负责"对象存活与刷新"的模块。
     *
     * 1、实体分两种载体，走同一套流程：
     *    - EntityObject：持有 GameObject，用于表现（模型、特效、挂点）；
     *    - EntityLogic ：不持有 GameObject，用于逻辑帧、服务器算等纯逻辑对象。
     *
     * 2、刷新由本模块集中派发（Update / FixedUpdate / LateUpdate），
     *    实例实现 IEntityTickable / IEntityFixedTickable / IEntityLateTickable 后才会进入
     *    对应刷新列表；未实现的实例不产生任何轮询开销，也不再使用反射调用。
     *    刷新链：MonoGameManager.Update → GameManager.OnRefresh → EntityManager.OnUpdate
     *            → Entity.Refresh → IEntityTickable.OnEntityTick（一次接口调用）。
     *
     * 3、实例生成时拥有唯一 Id（在同一实体内部唯一），可归属组别；
     *    组支持整体冻结 / 解冻（PauseEntityGroup / UnPauseEntityGroup），
     *    组冻结与个体暂停（IEntityObject.Paused）互相独立、结果取并集。
     *
     * 4、实体由模块注册 / 生成 / 隐藏 / 回收，业务侧不要自行 Destroy 实体对象。
     */
    //================================================
    [Module(Priority = ModuleConstant.PRIORITY_ENTITY)]
    internal sealed partial class EntityManager : Module, IEntityManager
    {
        static readonly Action<IEntityObject> logicEntityReleaser = e => { };
        #region Properties
        ///<inheritdoc/>
        public int EntityCount { get { return entityDict.Count; } }
        ///<inheritdoc/>
        public int EntityGroupCount { get { return entityGroupDict.Count; } }
        ///<inheritdoc/>
        public int EntityObjectCount
        {
            get
            {
                var count = 0;
                for (int i = 0; i < entityList.Count; i++)
                    count += entityList[i].EntityObjectCount;
                return count;
            }
        }
        /// <summary>
        /// 实体组名===实体组；
        /// </summary>
        Dictionary<string, EntityGroup> entityGroupDict;
        /// <summary>
        /// 实体名===实体（注册项）；
        /// </summary>
        Dictionary<string, Entity> entityDict;
        /// <summary>
        /// 实体注册顺序；刷新按此顺序遍历，保证派发次序稳定且不受字典扩容影响；
        /// </summary>
        List<Entity> entityList;
        /// <summary>
        /// 处于冻结状态的组名；组内实例生成时会继承该状态；
        /// </summary>
        HashSet<string> pausedGroupSet;

        IResourceManager resourceManager;
        IEntityHelper entityHelper;
        ///<inheritdoc/>
        public event Action<EntityRegisterFailureEventArgs> EntityRegisterFailure
        {
            add { entityRegisterFailure += value; }
            remove { entityRegisterFailure -= value; }
        }
        ///<inheritdoc/>
        public event Action<EntityRegisterSuccessEventArgs> EntityRegisterSuccess
        {
            add { entityRegisterSuccess += value; }
            remove { entityRegisterSuccess -= value; }
        }
        Action<EntityRegisterFailureEventArgs> entityRegisterFailure;
        Action<EntityRegisterSuccessEventArgs> entityRegisterSuccess;
        #endregion

        #region Methods
        ///<inheritdoc/>
        public void SetEntityHelper(IEntityHelper entityHelper)
        {
            this.entityHelper = entityHelper;
        }
        ///<inheritdoc/>
        public void RegisterEntityAsync<T>(EntityAssetInfo entityAssetInfo) where T : EntityObject
        {
            if (string.IsNullOrEmpty(entityAssetInfo.AssetName))
            {
                throw new ArgumentNullException("Entity asset name is invalid.");
            }
            var entityName = entityAssetInfo.EntityName;
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            if (!HasEntity(entityName))
            {
                //注册成功
                resourceManager.LoadPrefabAsync(entityAssetInfo.AssetName, (entityAsset) =>
                {
                    if (entityAsset != null)
                    {
                        OnEntityRegisterSuccess(entityAssetInfo, entityAsset, typeof(T));
                    }
                    else
                    {
                        OnEntityRegisterFailure(entityAssetInfo);
                    }
                });
            }
            else
            {
                //注册失败
                OnEntityRegisterFailure(entityAssetInfo);
            }
        }
        ///<inheritdoc/>
        public void RegisterLogicEntity<T>(string entityName, string entityGroupName = "") where T : EntityLogic, new()
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            if (entityDict.ContainsKey(entityName))
            {
                throw new ArgumentException($"Entity : {entityName} is already registered !");
            }
            var entity = Entity.Create(entityName, entityGroupName ?? string.Empty, typeof(T), () => { return new T(); }, logicEntityReleaser);
            entityDict.Add(entityName, entity);
            entityList.Add(entity);
            AddEntityToGroup(entityName, entityGroupName);
        }
        ///<inheritdoc/>
        public void DeregisterEntity(string entityName)
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            var hasEntity = entityDict.Remove(entityName, out var entity);
            if (!hasEntity)
                return;
            entityList.Remove(entity);
            RemoveEntityFromGroup(entityName, entity.EntityGroupName);
            Entity.Release(entity);
        }
        ///<inheritdoc/>
        public bool HasEntity(string entityName)
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            return entityDict.ContainsKey(entityName);
        }
        ///<inheritdoc/>
        public bool HasEntityObject(string entityName, int entityObjectId)
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return false;
            return entity.HasEntityObject(entityObjectId);
        }
        ///<inheritdoc/>
        public bool HasEntityGroup(string entityGroupName)
        {
            Utility.Text.IsStringValid(entityGroupName, "EntityGroupName is invalid !");
            return entityGroupDict.ContainsKey(entityGroupName);
        }
        ///<inheritdoc/>
        public bool GetEntityObject(string entityName, int entityObjectId, out IEntityObject entityObject)
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            entityObject = null;
            if (!entityDict.TryGetValue(entityName, out var entity))
                return false;
            return entity.GetEntityObject(entityObjectId, out entityObject);
        }
        ///<inheritdoc/>
        public T ShowEntity<T>(string entityName) where T : class, IEntityObject
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            if (!entityDict.TryGetValue(entityName, out var entity))
            {
                throw new ArgumentException($"Entity : {entityName} is not registered !");
            }
            var entityObject = entity.ShowEntityObject();
            //组冻结状态需要在生成时继承，否则冻结期间新生成的实例会漏跑/多跑
            if (pausedGroupSet.Count > 0 && !string.IsNullOrEmpty(entityObject.EntityGroupName))
            {
                entityObject.SetGroupPaused(pausedGroupSet.Contains(entityObject.EntityGroupName));
            }
            entityObject.OnShow();
            if (entityObject is T result)
                return result;
            //类型不符时回滚，避免不匹配的实例留在存活列表里
            entity.HideEntityObject(entityObject.EntityObjectId);
            throw new InvalidCastException($"Entity : {entityName}'s object is {entityObject.GetType()}, not {typeof(T)} !");
        }
        ///<inheritdoc/>
        public void HideEntityObject(string entityName, int entityObjectId)
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            if (!entityDict.TryGetValue(entityName, out var entity))
                return;
            entity.HideEntityObject(entityObjectId);
        }
        ///<inheritdoc/>
        public void HideEntityObject(IEntityObject entityObject)
        {
            if (entityObject == null)
            {
                throw new ArgumentNullException("EntityObject is invalid !");
            }
            HideEntityObject(entityObject.EntityName, entityObject.EntityObjectId);
        }
        ///<inheritdoc/>
        public void HideEntities(string entityName)
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            if (!entityDict.TryGetValue(entityName, out var entity))
                return;
            entity.HideAllEntityObject();
        }
        ///<inheritdoc/>
        public void SetEntityGroup(string entityName, string entityGroupName)
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            Utility.Text.IsStringValid(entityGroupName, "EntityGroupName is invalid !");
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return;
            var oldGroupName = entity.EntityGroupName;
            if (oldGroupName == entityGroupName)
                return;
            if (!string.IsNullOrEmpty(oldGroupName))
            {
                RemoveEntityFromGroup(entityName, oldGroupName);
            }
            entity.EntityGroupName = entityGroupName;
            AddEntityToGroup(entityName, entityGroupName);
            //组变更后按新组的冻结状态重新同步
            entity.SetGroupPaused(pausedGroupSet.Contains(entityGroupName));
        }
        ///<inheritdoc/>
        public void ShowGroupEntities(string entityGroupName)
        {
            Utility.Text.IsStringValid(entityGroupName, "EntityGroupName is invalid !");
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (!hasGroup)
                return;
            foreach (var entityName in entityGroup.EntityNames.ToArray())
            {
                if (!entityDict.TryGetValue(entityName, out var entity))
                    continue;
                var entityObject = entity.ShowEntityObject();
                if (pausedGroupSet.Contains(entityObject.EntityGroupName))
                {
                    entityObject.SetGroupPaused(true);
                }
                entityObject.OnShow();
            }
        }
        ///<inheritdoc/>
        public void HideGroupEntities(string entityGroupName)
        {
            Utility.Text.IsStringValid(entityGroupName, "EntityGroupName is invalid !");
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (!hasGroup)
                return;
            foreach (var entityName in entityGroup.EntityNames.ToArray())
            {
                if (entityDict.TryGetValue(entityName, out var entity))
                {
                    entity.HideAllEntityObject();
                }
            }
        }
        ///<inheritdoc/>
        public void LeaveEntityGroup(string entityName)
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return;
            var entityGroupName = entity.EntityGroupName;
            if (string.IsNullOrEmpty(entityGroupName))
                return;
            RemoveEntityFromGroup(entityName, entityGroupName);
            entity.EntityGroupName = string.Empty;
            entity.SetGroupPaused(false);
        }
        ///<inheritdoc/>
        public void DissolveEntityGroup(string entityGroupName)
        {
            Utility.Text.IsStringValid(entityGroupName, "EntityGroupName is invalid !");
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (!hasGroup)
                return;
            var entityNames = entityGroup.EntityNames.ToArray();
            for (int i = 0; i < entityNames.Length; i++)
            {
                if (entityDict.TryGetValue(entityNames[i], out var entity))
                {
                    entity.EntityGroupName = string.Empty;
                    entity.SetGroupPaused(false);
                }
            }
            entityGroupDict.Remove(entityGroupName);
            EntityGroup.Release(entityGroup);
            //解散是显式销毁组，同名组重建时不应继承冻结状态
            pausedGroupSet.Remove(entityGroupName);
        }
        ///<inheritdoc/>
        public void PauseEntityGroup(string entityGroupName)
        {
            Utility.Text.IsStringValid(entityGroupName, "EntityGroupName is invalid !");
            pausedGroupSet.Add(entityGroupName);
            if (entityGroupDict.TryGetValue(entityGroupName, out var entityGroup))
            {
                SetGroupPaused(entityGroup, true);
            }
        }
        ///<inheritdoc/>
        public void UnPauseEntityGroup(string entityGroupName)
        {
            Utility.Text.IsStringValid(entityGroupName, "EntityGroupName is invalid !");
            pausedGroupSet.Remove(entityGroupName);
            if (entityGroupDict.TryGetValue(entityGroupName, out var entityGroup))
            {
                SetGroupPaused(entityGroup, false);
            }
        }
        ///<inheritdoc/>
        public bool IsEntityGroupPaused(string entityGroupName)
        {
            Utility.Text.IsStringValid(entityGroupName, "EntityGroupName is invalid !");
            return pausedGroupSet.Contains(entityGroupName);
        }
        ///<inheritdoc/>
        public EntityInfo GetEntityInfo(string entityName)
        {
            Utility.Text.IsStringValid(entityName, "EntityName is invalid !");
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return EntityInfo.None;
            return entity.GetEntityInfo();
        }
        ///<inheritdoc/>
        public EntityGroupInfo GetEntityGroupInfo(string entityGroupName)
        {
            Utility.Text.IsStringValid(entityGroupName, "EntityGroupName is invalid !");
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (!hasGroup)
                return EntityGroupInfo.None;
            return entityGroup.GetEntityGroupInfo();
        }
        protected override void OnInitialization()
        {
            entityGroupDict = new Dictionary<string, EntityGroup>();
            entityDict = new Dictionary<string, Entity>();
            entityList = new List<Entity>();
            pausedGroupSet = new HashSet<string>();
            entityHelper = new DefaultEntityHelper();
        }
        protected override void OnPreparatory()
        {
            resourceManager = GameManager.GetModule<IResourceManager>();
        }
        protected override void OnFixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            for (int i = 0; i < entityList.Count; i++)
            {
                entityList[i].FixedRefresh(deltaTime);
            }
        }
        protected override void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
            for (int i = 0; i < entityList.Count; i++)
            {
                entityList[i].Refresh(deltaTime);
            }
        }
        protected override void OnLateUpdate()
        {
            var deltaTime = Time.deltaTime;
            for (int i = 0; i < entityList.Count; i++)
            {
                entityList[i].LateRefresh(deltaTime);
            }
        }
        protected override void OnTermination()
        {
            for (int i = 0; i < entityList.Count; i++)
            {
                Entity.Release(entityList[i]);
            }
            entityList.Clear();
            entityDict.Clear();
            foreach (var entityGroup in entityGroupDict.Values)
            {
                EntityGroup.Release(entityGroup);
            }
            entityGroupDict.Clear();
            pausedGroupSet.Clear();
            EntityGroup.ClearPool();
            Entity.ClearPool();
        }
        void OnEntityRegisterSuccess(EntityAssetInfo entityAssetInfo, GameObject entityAsset, Type entityObjectType)
        {
            var entityName = entityAssetInfo.EntityName;
            var groupName = entityAssetInfo.EntityGroupName ?? string.Empty;
            if (entityDict.ContainsKey(entityName))
            {
                Utility.Debug.LogWarning($"[Cosmos] Entity : {entityName} is already registered, ignore the duplicated async registration.");
                return;
            }
            var helper = entityHelper;
            var asset = entityAsset;
            var objectType = entityObjectType;
            var entity = Entity.Create(entityName, groupName, objectType,
                () => { return helper.InstantiateEntity(asset, objectType); },
                e => { helper.ReleaseEntity(e as EntityObject); });
            entityDict.Add(entityName, entity);
            entityList.Add(entity);
            AddEntityToGroup(entityName, groupName);

            var successArgs = EntityRegisterSuccessEventArgs.Create(entityAssetInfo.AssetName, entityName, groupName, entityObjectType);
            entityRegisterSuccess?.Invoke(successArgs);
            EntityRegisterSuccessEventArgs.Release(successArgs);
        }
        void OnEntityRegisterFailure(EntityAssetInfo entityAssetInfo)
        {
            var failureArgs = EntityRegisterFailureEventArgs.Create(entityAssetInfo.AssetName, entityAssetInfo.EntityName, entityAssetInfo.EntityGroupName);
            entityRegisterFailure?.Invoke(failureArgs);
            EntityRegisterFailureEventArgs.Release(failureArgs);
        }
        /// <summary>
        /// 把实体加入组（组不存在则创建）。
        /// </summary>
        void AddEntityToGroup(string entityName, string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
                return;
            if (!entityGroupDict.TryGetValue(entityGroupName, out var entityGroup))
            {
                entityGroup = EntityGroup.Create(entityGroupName);
                entityGroupDict.Add(entityGroupName, entityGroup);
            }
            entityGroup.AddEntity(entityName);
        }
        /// <summary>
        /// 把实体移出组（组空则回收组）；不改变冻结状态。
        /// </summary>
        void RemoveEntityFromGroup(string entityName, string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
                return;
            if (!entityGroupDict.TryGetValue(entityGroupName, out var entityGroup))
                return;
            entityGroup.RemoveEntity(entityName);
            if (entityGroup.EntityCount <= 0)
            {
                entityGroupDict.Remove(entityGroupName);
                EntityGroup.Release(entityGroup);
            }
        }
        /// <summary>
        /// 冻结 / 解冻组内所有实体。
        /// </summary>
        void SetGroupPaused(EntityGroup entityGroup, bool paused)
        {
            foreach (var entityName in entityGroup.EntityNames)
            {
                if (entityDict.TryGetValue(entityName, out var entity))
                {
                    entity.SetGroupPaused(paused);
                }
            }
        }
        #endregion
    }
}
