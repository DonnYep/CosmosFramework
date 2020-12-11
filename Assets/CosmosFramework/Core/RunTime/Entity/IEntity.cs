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
    public interface IEntity: IRefreshable
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
        object EntityInstance { get; }
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
        /// 设置实体数据；
        /// </summary>
        /// <param name="entityId">实体id</param>
        /// <param name="entityName">实体名称</param>
        /// <param name="entityAsset"> 实体实例对象</param>
        /// <param name="entityGroup">实体所属的实体组</param>
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
    }
}
