using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Entity
{
    public class EntityGroup :IRefreshable
    {
        public string EntityGroupName { get; private set; }
        public int EntityCount { get { return entityLinkedList.Count; } }
        LinkedList<IEntity> entityLinkedList;
        int entityId;
        public IEntity EntityAssetRoot { get; private set; }
        Action refreshHandler;
        event Action RefreshHandler
        {
            add { refreshHandler += value; }
            remove { refreshHandler -= value; }
        }
        /// <summary>
        /// 为实体组赋予根节点；
        /// </summary>
        /// <param name="root">根节点</param>
        public void AssignAssetRoot(IEntity root)
        {
            EntityAssetRoot = root;
        }
        public EntityGroup(string name)
        {
            EntityGroupName = name;
            entityLinkedList = new LinkedList<IEntity>();
        }
        public void OnRefresh()
        {
            refreshHandler?.Invoke();
        }
        public bool HasEntity(string entityName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity asset name is invalid.");
            }
            foreach (IEntity entity in entityLinkedList)
            {
                if (entity.EntityName == entityName)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasEntity(int entityId)
        {
            foreach (IEntity entity in entityLinkedList)
            {
                if (entity.EntityId == entityId)
                {
                    return true;
                }
            }
            return false;
        }
        public IEntity GetEntity(int entityId)
        {
            foreach (IEntity entity in entityLinkedList)
            {
                if (entity.EntityId == entityId)
                {
                    return entity;
                }
            }
            return null;
        }
        public IEntity GetEntity(string entityName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity asset name is invalid.");
            }
            foreach (IEntity entity in entityLinkedList)
            {
                if (entity.EntityName == entityName)
                {
                    return entity;
                }
            }
            return null;
        }
        public IEntity[] GetEntities(string entityName)
        {
            List<IEntity> entityList = new List<IEntity>();
            if (string.IsNullOrEmpty(entityName))
            {
                throw new ArgumentNullException("Entity asset name is invalid.");
            }
            foreach (IEntity entity in entityLinkedList)
            {
                if (entity.EntityName == entityName)
                {
                    entityList.Add(entity);
                }
            }
            return entityList.ToArray();
        }
        public IEntity[] GetAllEntities()
        {
            List<IEntity> entityList = new List<IEntity>();
            foreach (IEntity entity in entityLinkedList)
            {
                entityList.Add(entity);
            }
            return entityList.ToArray();
        }
        public void AddEntity(IEntity entity)
        {
            entityLinkedList.AddLast(entity);
            RefreshHandler += entity.OnRefresh;
        }
        public void RemoveEntity(IEntity entity)
        {
            entityLinkedList.Remove(entity);
            RefreshHandler -= entity.OnRefresh;
        }
        public void ActiveEntities()
        {
            foreach (IEntity entity in entityLinkedList)
            {
                entity.OnActive();
            }
        }
        public void DeactiveEntities()
        {
            foreach (IEntity entity in entityLinkedList)
            {
                entity.OnDeactive();
            }
        }
    }
}
