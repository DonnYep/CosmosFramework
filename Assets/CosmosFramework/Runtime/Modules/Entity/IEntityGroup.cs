using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Entity;

namespace Cosmos
{
    public interface IEntityGroup : IRefreshable
    {
        string EntityGroupName { get; }
        int EntityCount { get; }
        object EntityAsset { get; }
        IEntity EntityRoot { get; }
        IObjectPool ObjectPool { get; }
        /// <summary>
        /// 为实体组赋予根节点；
        /// </summary>
        /// <param name="root">根节点</param>
        void AssignEntityRoot(IEntity root);
        bool HasEntity(string entityName);
        bool HasEntity(int entityId);
        IEntity GetEntity(int entityId);
        IEntity GetEntity(string entityName);
        IEntity[] GetEntities(string entityName);
        IEntity[] GetAllChildEntities();
        void AddEntity(IEntity entity);
        void RemoveEntity(IEntity entity);
        void ClearChildEntities();
    }
}
