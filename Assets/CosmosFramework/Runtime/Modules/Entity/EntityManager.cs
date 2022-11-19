using Cosmos.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Entity
{
    //================================================
    /*
     * 使用步骤：
     * 
     * 1、实体对象管理模块。
     * 
     */
    //================================================
    [Module]
    internal sealed partial class EntityManager : Module, IEntityManager
    {
        #region Properties
        ///<inheritdoc/>
        public int EntityCount { get { return entityDict.Count; } }
        ///<inheritdoc/>
        public int EntityGroupCount { get { return entityGroupDict.Count; } }

        /// <summary>
        /// EntityGroupName===EntityGroup
        /// </summary>
        Dictionary<string, EntityGroup> entityGroupDict;
        /// <summary>
        /// EntityName===Entity
        /// </summary>
        Dictionary<string, Entity> entityDict;

        IResourceManager resourceManager;
        IEntityHelper entityHelper;
        public event Action<EntityRegisterFailureEventArgs> EntityRegisterFailure
        {
            add { entityRegisterFailure += value; }
            remove { entityRegisterFailure -= value; }
        }
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
        public void DeregisterEntity(string entityName)
        {
            //注销并清空
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            var hasEntity = entityDict.Remove(entityName, out var entity);
            if (!hasEntity)
                return;
            var entityGroupName = entity.EntityGroupName;
            if (string.IsNullOrEmpty(entityGroupName))
                return;
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (hasGroup)
            {
                entityGroup.RemoveEntity(entityName);
                if (entityGroup.EntityCount <= 0)
                {
                    entityGroupDict.Remove(entityGroupName);
                    EntityGroup.Release(entityGroup);
                }
            }
        }
        ///<inheritdoc/>
        public bool HasEntity(string entityName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            return entityDict.ContainsKey(entityName);
        }
        ///<inheritdoc/>
        public bool HasEntityObject(string entityName, int entityObjectId)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return false;
            return entity.HasEntityObject(entityObjectId);
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
        public bool ShowEntity(string entityName, out EntityObject entityObject)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            if (entityHelper == null)
            {
                throw new ArgumentNullException("Entity helper is invalid.");
            }
            entityObject = default;
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return false;
            entityObject = entity.ShowEntityObject();
            entityObject.OnShow();
            return hasEntity;
        }
        ///<inheritdoc/>
        public void HideEntity(string entityName, int entityObjectId)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            if (entityHelper == null)
            {
                throw new ArgumentNullException("Entity helper is invalid.");
            }
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return;
            entity.HideEntityObject(entityObjectId);
        }
        ///<inheritdoc/>
        public void HideEntity(string entityName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            if (entityHelper == null)
            {
                throw new ArgumentNullException("Entity helper is invalid.");
            }
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return;
            entity.HideAllEntityObject();
        }
        ///<inheritdoc/>
        public void HideEntityObject(EntityObject entityObject)
        {
            var hasEntity = entityDict.TryGetValue(entityObject.EntityName, out var entity);
            if (!hasEntity)
                return;
            entity.HideEntityObject(entityObject.EntityObjectId);
        }
        ///<inheritdoc/>
        public void SetEntityGroup(string entityName, string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return;
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (!hasGroup)
            {
                entityGroup = EntityGroup.Create(entityGroupName);
                entityGroup.AddEntity(entityName);
                entityGroupDict.TryAdd(entityGroupName, entityGroup);
            }
            entityGroup.AddEntity(entityName);
            entity.EntityGroupName = entityGroupName;
        }
        ///<inheritdoc/>
        public void ShowGroupEntities(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (!hasGroup)
                return;
            foreach (var entityName in entityGroup.EntityNames)
            {
                ShowEntity(entityName, out _);
            }
        }
        ///<inheritdoc/>
        public void HideGroupEntities(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (!hasGroup)
                return;
            foreach (var entityName in entityGroup.EntityNames)
            {
                HideEntity(entityName);
            }
        }
        ///<inheritdoc/>
        public void LeaveEntityGroup(string entityName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return;
            var entityGroupName = entity.EntityGroupName;
            if (string.IsNullOrEmpty(entityGroupName))
                return;
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (hasGroup)
            {
                entityGroup.RemoveEntity(entityName);
                if (entityGroup.EntityCount <= 0)
                {
                    entityGroupDict.Remove(entityGroupName);
                    EntityGroup.Release(entityGroup);
                }
            }
        }
        ///<inheritdoc/>
        public void DissolveEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (!hasGroup)
                return;
            foreach (var entityName in entityGroup.EntityNames)
            {
                if (entityDict.TryGetValue(entityName, out var entityInfo))
                {
                    entityInfo.EntityGroupName = string.Empty;
                }
            }
            entityGroupDict.Remove(entityGroupName);
            EntityGroup.Release(entityGroup);
        }
        ///<inheritdoc/>
        public EntityInfo GetEntityInfo(string entityName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity name is invalid.");
            }
            var hasEntity = entityDict.TryGetValue(entityName, out var entity);
            if (!hasEntity)
                return EntityInfo.None;
            return entity.GetEntityInfo();
        }
        ///<inheritdoc/>
        public EntityGroupInfo GetEntityGroupInfo(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new ArgumentNullException("Entity group name is invalid.");
            }
            var hasGroup = entityGroupDict.TryGetValue(entityGroupName, out var entityGroup);
            if (!hasGroup)
                return EntityGroupInfo.None;
            return entityGroup.GetEntityGroupInfo();
        }
        protected override void OnInitialization()
        {
            entityGroupDict = new Dictionary<string, EntityGroup>();
            entityDict = new Dictionary<string, Entity>();
            entityHelper = new DefaultEntityHelper();
        }
        protected override void OnPreparatory()
        {
            resourceManager = GameManager.GetModule<IResourceManager>();
        }
        void OnEntityRegisterSuccess(EntityAssetInfo entityAssetInfo, GameObject entityAsset, Type entityObjectType)
        {
            var groupName = entityAssetInfo.EntityGroupName;
            var entity = Entity.Create(entityAssetInfo.EntityName, entityAssetInfo.EntityGroupName, entityAsset, entityObjectType, entityHelper);
            if (!string.IsNullOrEmpty(groupName))
            {
                //这里处理组别；
                var hasGroup = HasEntityGroup(groupName);
                if (!hasGroup)
                {
                    var entityGroup = EntityGroup.Create(groupName);
                    entityGroupDict.TryAdd(groupName, entityGroup);
                }
                else
                {
                    entityGroupDict.TryGetValue(groupName, out var entityGroup);
                    entityGroup.AddEntity(entityAssetInfo.EntityName);
                }
            }
            entityDict.Add(entityAssetInfo.EntityName, entity);
            var successArgs = EntityRegisterSuccessEventArgs.Create(entityAssetInfo.AssetName, entityAssetInfo.EntityName, groupName, entityObjectType);
            entityRegisterSuccess?.Invoke(successArgs);
            EntityRegisterSuccessEventArgs.Release(successArgs);
        }
        void OnEntityRegisterFailure(EntityAssetInfo entityAssetInfo)
        {
            var failureArgs = EntityRegisterFailureEventArgs.Create(entityAssetInfo.AssetName, entityAssetInfo.EntityName, entityAssetInfo.EntityGroupName);
            entityRegisterFailure?.Invoke(failureArgs);
            EntityRegisterFailureEventArgs.Release(failureArgs);
        }
        #endregion
    }
}