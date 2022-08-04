﻿namespace Cosmos.Entity
{
    public interface IEntityHelper
    {
        /// <summary>
        /// 实例化实体；
        /// </summary>
        /// <param name="entityAsset">实体资源</param>
        /// <returns>实体对象</returns>
        object InstantiateEntity(object entityAsset);
        /// <summary>
        /// 释放实体；
        /// </summary>
        /// <param name="entityInstance">实体实例</param>
        void ReleaseEntity(object entityInstance);
        void AttachToParent(IEntity childEntity, IEntity parentEntity);
        void DeatchFromParent(IEntity entity);
    }
}
