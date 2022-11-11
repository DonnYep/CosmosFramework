using System.Collections.Generic;
namespace Cosmos.Entity
{
    /// <summary>
    /// 实体；
    /// </summary>
    public sealed class Entity : IEntity
    {
        static Pool<Entity> entityPool = new Pool<Entity>
            (
                () => { return new Entity(); },
                e => { e.Release(); }
            );
        /// <inheritdoc/>
        public int EntityId { get; private set; }
        /// <inheritdoc/>
        public string EntityName { get; private set; }
        /// <inheritdoc/>
        public EntityObject EntityObject { get; private set; }
        /// <inheritdoc/>
        public IEntityGroup EntityGroup { get; private set; }
        /// <inheritdoc/>
        public IEntity ParentEntity { get; private set; }
        /// <inheritdoc/>
        public int ChildEntityCount { get; }
        /// <inheritdoc/>
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
            EntityId = 0;
            EntityGroup = null;
            EntityName = null;
            EntityObject = null;
        }
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
        /// <param name="entityObjectAsset"> 实体实例对象</param>
        /// <param name="entityGroup">实体所属的实体组</param>
        public void SetEntity(int entityId, string entityName, EntityObject entityObjectAsset, IEntityGroup entityGroup)
        {
            this.EntityId = entityId;
            this.EntityName = entityName;
            this.EntityObject = entityObjectAsset;
            this.EntityGroup = entityGroup;
        }
        internal static Entity Create(int entityId, string entityName, EntityObject entityObject, IEntityGroup entityGroup)
        {
            var entity = entityPool.Spawn();
            entity.SetEntity(entityId, entityName, entityObject, entityGroup);
            return entity;
        }
        internal static void Release(Entity entity)
        {
            entityPool.Despawn(entity);
        }
    }
}