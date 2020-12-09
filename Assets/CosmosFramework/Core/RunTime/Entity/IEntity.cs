using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using UnityEngine;
namespace Cosmos.Entity
{
    /// <summary>
    /// 与unity耦合的实体对象，当前版本中使用的是此实体对象
    /// </summary>
    public interface IEntity: IReference, IRefreshable, IOperable,IRecyclable
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
        object EntityAsset { get; }
        /// <summary>
        /// 实体所属的实体组。
        /// </summary>
        IEntityGroup EntityGroup{get;}
        /// <summary>
        /// 父实体对象；
        /// </summary>
        IEntity ParentEntity { get; }
        /// <summary>
        /// 子实体总数
        /// </summary>
        int ChildEntityCount { get; }
        /// <summary>
        /// 配置实体对象；
        /// </summary>
        /// <param name="entityId">实体id</param>
        /// <param name="entityName">实体名称</param>
        /// <param name="entityAsset">实体索引的具体对象</param>
        /// <param name="entityGroup">实体所在的实体组</param>
        void SetEntity(int entityId,string entityName,object entityAsset,IEntityGroup entityGroup);
        /// <summary>
        /// 获取一个子实体
        /// </summary>
        /// <returns>获取的子实体</returns>
        IEntity GetChildEntity();
        /// <summary>
        /// 获取所有子实体
        /// </summary>
        /// <returns>所有子实体的数组</returns>
        IEntity[] GetChildEntities();
        /// <summary>
        /// 挂载到一个实体上；
        /// </summary>
        /// <param name="parent">父实体对象</param>
        void OnAttachTo(IEntity parent);
        /// <summary>
        /// 挂载一个子实体到此实体上；
        /// </summary>
        /// <param name="child">子实体对象</param>
        void OnAttached(IEntity child);
        /// <summary>
        /// 从父实体对象上接触挂载；
        /// </summary>
        /// <param name="parent">父实体对象</param>
        void OnDetachFrom(IEntity parent);
        /// <summary>
        /// 解除子实体的挂载；
        /// </summary>
        /// <param name="child">子实体对象</param>
        void OnDetached(IEntity child);
    }
}
