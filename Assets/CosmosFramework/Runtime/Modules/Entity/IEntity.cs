namespace Cosmos.Entity
{
    /// <summary>
    /// 实体接口；
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 实体id；
        /// </summary>
        int EntityId { get; }
        /// <summary>
        /// 实体名称；
        /// </summary>
        string EntityName { get; }
        /// <summary>
        /// 实体索引的具体对象；
        /// </summary>
        EntityObject EntityObject{ get; }
        /// <summary>
        /// 实体所属的实体组
        /// </summary>
        IEntityGroup EntityGroup{get;}
        /// <summary>
        /// 父实体对象；
        /// </summary>
        IEntity ParentEntity { get; }
        /// <summary>
        /// 子实体总数；
        /// </summary>
        int ChildEntityCount { get; }
        /// <summary>
        /// 获取一个子实体；
        /// </summary>
        /// <returns>获取的子实体</returns>
        IEntity GetChildEntity();
        /// <summary>
        /// 获取所有子实体；
        /// </summary>
        /// <returns>所有子实体的数组</returns>
        IEntity[] GetChildEntities();
    }
}
