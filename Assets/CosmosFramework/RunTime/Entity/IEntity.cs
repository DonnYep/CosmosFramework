using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.Entity
{
    public interface IEntity
    {
        int Id { get; }
        string EnitityAssetName { get; }
        object Handle { get; }
        /// <summary>
        /// 实体回收。
        /// </summary>
        void OnRecycle();
        /// <summary>
        /// 实体显示。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        void OnShow(object userData);
        /// <summary>
        /// 实体隐藏。
        /// </summary>
        /// <param name="isShutdown">是否是关闭实体管理器时触发。</param>
        /// <param name="userData">用户自定义数据。</param>
        void OnHide(bool isShutdown, object userData);
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
        /// <summary>
        /// 实体轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void OnUpdate(float elapseSeconds, float realElapseSeconds);
    }
}
