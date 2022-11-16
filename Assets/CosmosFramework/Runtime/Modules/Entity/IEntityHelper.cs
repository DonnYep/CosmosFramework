using System;
using UnityEngine;

namespace Cosmos.Entity
{
    public interface IEntityHelper
    {
        /// <summary>
        /// 实例化实体；
        /// </summary>
        /// <param name="entityObjectAsset">实体资源</param>
        /// <returns>实体对象</returns>
        EntityObject InstantiateEntity(GameObject entityAsset, Type entityObjectType);
        /// <summary>
        /// 释放实体；
        /// </summary>
        /// <param name="entityObject">实体对象</param>
        void ReleaseEntity(EntityObject entityObject);
    }
}
