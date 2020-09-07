using System.Collections;
using System.Collections.Generic;
using System;
using Cosmos;
namespace Cosmos.Entity
{
    /// <summary>
    /// 与Unity解耦的实体对象
    /// </summary>
    public interface IEntity:IBehaviour,IControllable,IOperable,IRefreshable
    {
        /// <summary>
        /// 获取实体编号。
        /// </summary>
        int Id { get; }
        /// <summary>
        /// 获取实体资源名称。
        /// </summary>
        string EnitityAssetName { get; }
        /// <summary>
        /// 获取实体实例。
        /// </summary>
        object Handle { get; }
        /// <summary>
        /// 实体附加子实体。
        /// </summary>
        /// <param name="childEntity">附加的子实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        void OnAttached(IEntity childEntity, object userData);
        /// <summary>
        /// 实体解除子实体。
        /// </summary>
        /// <param name="childEntity">解除的子实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        void OnDetached(IEntity childEntity, object userData);
        /// <summary>
        /// 实体附加子实体。
        /// </summary>
        /// <param name="parentEntity">被附加的父实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        void OnAttachTo(IEntity parentEntity, object userData);
        /// <summary>
        /// 实体解除子实体。
        /// </summary>
        /// <param name="parentEntity">被解除的父实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        void OnDetachFrom(IEntity parentEntity, object userData);
    }
}
