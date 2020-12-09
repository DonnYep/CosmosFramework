using System.Collections;
using System.Collections.Generic;
using Cosmos;
using System;
namespace Cosmos.Entity
{
    /// <summary>
    /// 实体对象
    /// </summary>
    public abstract class EntityBase : IEntity
    {
        /// <summary>
        /// 实体id；
        /// </summary>
        public int EntityId { get; protected set; }
        /// <summary>
        /// 实体名称；
        /// </summary>
        public string EntityName { get; protected set; }
        /// <summary>
        /// 实体索引的具体对象；
        /// </summary>
        public object EntityAsset { get; protected set; }
        /// <summary>
        /// 获取实体所属的实体组;
        /// </summary>
        public IEntityGroup EntityGroup { get; protected set; }
        /// <summary>
        /// 父实体对象；
        /// </summary>
        public IEntity ParentEntity { get; protected set; }
        public int ChildEntityCount { get; }
        /// <summary>
        /// 子实体集合；
        /// </summary>
        protected List<IEntity> childEntities;
        protected EntityBase()
        {
            childEntities = new List<IEntity>();
        }
        /// <summary>
        /// 引用池回收；
        /// </summary>
        public virtual void Clear() { }
        /// <summary>
        /// 激活实体对象；
        /// </summary>
        public abstract void OnActive();
        /// <summary>
        /// 失活实体对象；
        /// </summary>
        public abstract void OnDeactive();
        /// <summary>
        /// 实体轮询。
        /// </summary>
        public virtual void OnRefresh() { }
        /// <summary>
        /// 挂载到一个父类对象;
        /// </summary>
        /// <param name="parent">父类对象</param>
        public abstract void OnAttachTo(IEntity parent);
        /// <summary>
        /// 挂载一个实体到此实体上;
        /// </summary>
        /// <param name="child">附加的子实体</param>
        public virtual void OnAttached(IEntity child) { }
        /// <summary>
        /// 从父类对象移除;
        /// </summary>
        public abstract void OnDetachFrom(IEntity parent);
        /// <summary>
        /// 移除子对象;
        /// </summary>
        public virtual void OnDetached(IEntity child) { }
        /// <summary>
        /// 当对象被回收;
        /// </summary>
        public abstract void OnRecycle();
        public void SetEntity(int entityId, string entityName, object entityAsset, IEntityGroup entityGroup)
        {
            this.EntityId = entityId;
            this.EntityName = entityName;
            this.EntityAsset = entityAsset;
            this.EntityGroup = entityGroup;
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
    }
}