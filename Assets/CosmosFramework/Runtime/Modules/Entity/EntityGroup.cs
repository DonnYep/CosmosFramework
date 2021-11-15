using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Entity
{
    internal sealed class EntityGroup : IEntityGroup
    {
        public string EntityGroupName { get; private set; }
        public int EntityCount { get { return entityLinkedList.Count; } }
        LinkedList<IEntity> entityLinkedList;
        public object EntityAsset { get; private set; }
        public IEntity EntityRoot { get; private set; }
        public object EntityRootInstance { get; private set; }
        IEntityHelper entityHelper;
        Action refreshHandler;
        event Action RefreshHandler
        {
            add { refreshHandler += value; }
            remove { refreshHandler -= value; }
        }
        public IObjectPool ObjectPool { get; private set; }
        /// <summary>
        /// 根节点是否是实体对象；
        /// </summary>
        bool entityRootIsInstance = false;
        public void AssignObjectPool(IObjectPool objectPool)
        {
            this.ObjectPool = objectPool;
        }
        /// <summary>
        /// 为实体组赋予根节点；
        /// </summary>
        /// <param name="root">根节点</param>
        public void AssignEntityRoot(IEntity root)
        {
            EntityRoot = root;
            entityRootIsInstance = false;
        }
        public void AssignEntityRoot(object root)
        {
            EntityRootInstance = root;
            entityRootIsInstance = true;
        }
        public EntityGroup(string name, object entityAsset, IEntityHelper entityHelper)
        {
            EntityGroupName = name;
            entityLinkedList = new LinkedList<IEntity>();
            this.EntityAsset = entityAsset;
            this.entityHelper = entityHelper;
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
        public IEntity[] GetAllChildEntities()
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
            var root = entityRootIsInstance == false ? EntityRoot : EntityRootInstance;
            if (root != null)
                entityHelper.Deatch(entity, root);
        }
        public void RemoveEntity(IEntity entity)
        {
            entityLinkedList.Remove(entity);
            RefreshHandler -= entity.OnRefresh;
            var root = entityRootIsInstance == false ? EntityRoot : EntityRootInstance;
            if (root != null)
                entityHelper.Deatch(entity, root);
        }
        public void ClearChildEntities()
        {
            entityLinkedList.Clear();
            refreshHandler = null;
        }
    }
}
