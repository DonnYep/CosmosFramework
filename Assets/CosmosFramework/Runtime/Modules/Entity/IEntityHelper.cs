namespace Cosmos.Entity
{
    public interface IEntityHelper
    {
        /// <summary>
        /// 实例化实体；
        /// </summary>
        /// <param name="entityObjectAsset">实体资源</param>
        /// <returns>实体对象</returns>
        EntityObject InstantiateEntity(EntityObject entityObjectAsset);
        /// <summary>
        /// 释放实体；
        /// </summary>
        /// <param name="entityObject">实体对象</param>
        void ReleaseEntity(EntityObject entityObject);
        void AttachToParent(IEntity childEntity, IEntity parentEntity);
        void DeatchFromParent(IEntity entity);
    }
}
