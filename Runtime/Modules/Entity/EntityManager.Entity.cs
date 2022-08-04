﻿using System.Collections.Generic;
namespace Cosmos.Entity
{
    internal partial class EntityManager
    {
        /// <summary>
        /// 实体对象
        /// </summary>
        private class Entity : IEntity
        {
            static Pool<Entity> entityPool = new Pool<Entity>(() => { return new Entity(); }, e => { e.Release(); });
            /// <summary>
            /// 实体id；
            /// </summary>
            public int EntityId { get; private set; }
            /// <summary>
            /// 实体名称；
            /// </summary>
            public string EntityName { get; private set; }
            /// <summary>
            /// 实体实例对象；
            /// </summary>
            public object EntityInstance { get; private set; }
            /// <summary>
            ///   实体所属的实体组;
            /// </summary>
            public IEntityGroup EntityGroup { get; private set; }
            /// <summary>
            /// 父实体对象；
            /// </summary>
            public IEntity ParentEntity { get; private set; }
            /// <summary>
            /// 实体的子对象总数；
            /// </summary>
            public int ChildEntityCount { get; }
            /// <summary>
            /// 子实体集合；
            /// </summary>
            List<IEntity> childEntities;
            public Entity()
            {
                childEntities = new List<IEntity>();
            }
            /// <summary>
            /// 引用池回收；
            /// </summary>
            public void Release()
            {
                ParentEntity = null;
                childEntities.Clear();
                EntityInstance = null;
                EntityId = 0;
                EntityGroup = null;
                EntityName = null;
            }
            /// <summary>
            /// 实体轮询。
            /// </summary>
            public void OnRefresh() { }//TODO Entity实体轮询未实现；
            /// <summary>
            /// 挂载到一个父类对象;
            /// </summary>
            /// <param name="parent">父类对象</param>
            public void SetParent(IEntity parent)
            {
                this.ParentEntity = parent;
            }
            /// <summary>
            /// 从父类对象移除;
            /// </summary>
            public void ClearParent()
            {
                this.ParentEntity = null;
            }
            /// <summary>
            /// 移除子对象;
            /// </summary>
            public void RemoveChild(IEntity child)
            {
                childEntities.Remove(child);
            }
            /// <summary>
            /// 挂载一个实体到此实体上;
            /// </summary>
            /// <param name="child">附加的子实体</param>
            public void AddChild(IEntity child)
            {
                childEntities.Add(child);
            }
            /// <summary>
            /// 获取一个子实体
            /// </summary>
            /// <returns>获取的子实体</returns>
            public IEntity GetChildEntity()
            {
                return childEntities.Count > 0 ? childEntities[0] : null;
            }
            /// <summary>
            /// 获取所有子实体
            /// </summary>
            /// <returns>所有子实体的数组</returns>
            public IEntity[] GetChildEntities()
            {
                return childEntities.ToArray();
            }
            /// <summary>
            /// 设置实体数据；
            /// </summary>
            /// <param name="entityId">实体id</param>
            /// <param name="entityName">实体名称</param>
            /// <param name="entityAsset"> 实体实例对象</param>
            /// <param name="entityGroup">实体所属的实体组</param>
            public void SetEntity(int entityId, string entityName, object entityAsset, IEntityGroup entityGroup)
            {
                this.EntityId = entityId;
                this.EntityName = entityName;
                this.EntityInstance = entityAsset;
                this.EntityGroup = entityGroup;
            }
            public static Entity Create(int entityId, string entityName, object entityInstance, IEntityGroup entityGroup)
            {
                var entity = entityPool.Spawn();
                entity.SetEntity(entityId, entityName, entityInstance, entityGroup);
                return entity;
            }
            public static void Release(Entity entity)
            {
                entityPool.Despawn(entity);
            }
        }
    }
}