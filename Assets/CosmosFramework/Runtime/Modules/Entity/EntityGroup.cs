using System;
using System.Collections.Generic;
namespace Cosmos.Entity
{
    internal sealed class EntityGroup : IEntityGroup
    {
        static readonly Pool<EntityGroup> entityGroupPool
            = new Pool<EntityGroup>(() => { return new EntityGroup(); },g=> { g.Release(); });

        public string EntityGroupName { get; private set; }
        public int EntityCount { get { return entityLinkedList.Count; } }
        public object EntityAsset { get; private set; }
        public IEntity EntityRoot { get; private set; }
        public object EntityRootInstance { get; private set; }
        public IObjectPool ObjectPool { get; private set; }

        LinkedList<IEntity> entityLinkedList=new LinkedList<IEntity>();
        IEntityHelper entityHelper;
        Action refreshHandler;
        event Action RefreshHandler
        {
            add { refreshHandler += value; }
            remove { refreshHandler -= value; }
        }
        /// <summary>
        /// 根节点是否是实体对象；
        /// </summary>
        bool entityRootIsInstance = false;
        public void AssignObjectPool(IObjectPool objectPool)
        {
            this.ObjectPool = objectPool;
            ObjectPool.OnObjectSpawn += go => { go.gameObject.SetActive(true); };
            ObjectPool.OnObjectDespawn+= go => { go.gameObject.SetActive(false); };
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
                entityHelper.AttachToParent(entity, root);
        }
        public void RemoveEntity(IEntity entity)
        {
            entityLinkedList.Remove(entity);
            RefreshHandler -= entity.OnRefresh;
            var root = entityRootIsInstance == false ? EntityRoot : EntityRootInstance;
            if (root != null)
                entityHelper.DeatchFromParent(entity);
        }
        public void ClearChildEntities()
        {
            entityLinkedList.Clear();
            refreshHandler = null;
        }
        void Release()
        {
            EntityGroupName = string.Empty;
            entityLinkedList.Clear();
            EntityAsset = null;
            EntityRoot = null;
            EntityRootInstance = null;
            ObjectPool = null;
            entityHelper = null;
            refreshHandler = null;
        }
        internal static EntityGroup Create(string name, object entityAsset, IEntityHelper entityHelper)
        {
            var entityGroup = entityGroupPool.Spawn();
            entityGroup .EntityGroupName = name;
            entityGroup.EntityAsset = entityAsset;
            entityGroup.entityHelper = entityHelper;
            return entityGroup;
        }
        internal static void Release(EntityGroup entityGroup)
        {
            entityGroupPool.Despawn(entityGroup);
        }
    }
}
