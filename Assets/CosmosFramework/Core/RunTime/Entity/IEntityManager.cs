using Cosmos.Entity;
using System;
using System.Collections.Generic;

namespace Cosmos
{
    public interface IEntityManager:IModuleManager
    {
        int EntityTypeCount { get; }
        bool AddEntity<T>(T entity) where T : class, IEntity, new();
        /// <summary>
        /// 回收单个实体对象;
        /// 若实体对象存在于缓存中，则移除。若不存在，则不做操作；
        /// </summary>
        /// <param name="entity">实体对象</param>
        void RecoveryEntity<T>(T entity) where T : class, IEntity, new();
        /// <summary>
        /// 回收某一类型的实体对象
        /// </summary>
        /// <param name="type">实体类型</param>
        void RecoveryEntities(Type type);
        // TODO  使用特性来管理实体资源加载
        T CreateEntity<T>(Action<float> loadingCallback, Action<IEntity> loadDoneCallback) where T : class, IEntity, new();
        int GetEntityCount<T>() where T : class, IEntity, new();
        int GetEntityCount(Type type);
        T GetEntity<T>(Predicate<T> predicate) where T : class, IEntity, new();
        IEntity GetEntity(Type type, Predicate<IEntity> predicate);
        ICollection<IEntity> GetEntityCollection<T>() where T : IEntity, new();
        ICollection<IEntity> GetEntityCollection(Type type);
        IEntity[] GetEntities<T>() where T : class, IEntity, new();
        IEntity[] GetEntities(Type type);
        List<IEntity> GetEntityList<T>() where T : class, IEntity, new();
        List<IEntity> GetEntityList(Type type);
        void RegisterEntityType<T>() where T : class, IEntity, new();
        void RegisterEntityType(Type type);
        bool DeregisterEntityType<T>() where T : class, IEntity, new();
        bool DeregisterEntityType(Type type);
        bool ActiveEntity<T>(T entity) where T : class, IEntity, new();
        void ActiveEntity<T>(Predicate<T> predicate) where T : class, IEntity, new();
        bool ActiveEntity(Type type, IEntity entity);
        void ActiveEntity(Type type, Predicate<IEntity> predicate);
        void ActiveEntities<T>() where T : class, IEntity, new();
        void ActiveEntities(Type type);
        void DeactiveEntities(Type type);
        bool HasEntity<T>(Predicate<T> predicate) where T : class, IEntity, new();
        bool HasEntity(Type type, Predicate<IEntity> predicate);
        bool HasEntityType<T>() where T : class, IEntity, new();
        bool HasEntityType(Type type);
    }
}
