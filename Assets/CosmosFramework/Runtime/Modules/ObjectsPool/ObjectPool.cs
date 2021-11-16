using UnityEngine;
using System.Collections.Generic;
using System;
using Cosmos.Event;
using Object = UnityEngine.Object;
namespace Cosmos.ObjectPool
{
    internal sealed class ObjectPool: IObjectPool
    {
        static readonly Pool<ObjectPool> poolInctPool 
            = new Pool<ObjectPool>(() => { return new ObjectPool(); }, p => { p.Release(); });

        public int ExpireTime
        {
            get { return expireTime; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("ExpireTime is invalid.");
                }
                if (expireTime == value)
                    return;
                expireTime = value;
            }
        }
        public int ReleaseInterval
        {
            get { return releaseInterval; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("ReleaseInterval is invalid.");
                }
                if (value == releaseInterval)
                    return;
                releaseInterval = value;
            }
        }
        public int Capacity
        {
            get { return capacity; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("capacity is invalid.");
                }
                if (value == capacity)
                    return;
                capacity = value;
            }
        }
        public Type ObjectType { get { return objectKey.PoolType; } }
        public string Name { get { return objectKey.PoolName; } }
        ObjectPoolKey objectKey;
        /// <summary>
        /// 当前池对象中的数量
        /// </summary>
        public int ObjectCount { get { return pool.Count; } }
        /// <summary>
        /// 生成的对象
        /// </summary>
        GameObject spawnItem;
        Pool<GameObject> pool;
        Action<GameObject> onSpawn;
        Action<GameObject>  onDespawn;
        /// <summary>
        /// 对象生成后的过期时间；
        /// </summary>
        int expireTime;
        int capacity;
        int releaseInterval = 5;
        private ObjectPool() { }
        public void OnElapseRefresh(float deltatime)
        {
            if (expireTime <= 0)
                return;
        }
        public void SetCallback(Action<GameObject> onSpawn, Action<GameObject> onDespawn)
        {
            this.onDespawn = onDespawn;
            this.onSpawn = onSpawn;
        }
        public GameObject Spawn()
        {
            var go= pool.Spawn();
            onSpawn?.Invoke(go);
            return go;
        }
        public void Despawn(object obj)
        {
            var go = obj.CastTo<GameObject>();
            onDespawn?.Invoke(go);
            pool.Despawn(go);
        }
        public void ClearPool()
        {
            pool.Clear();
        }
        void Release()
        {
            releaseInterval = 5;
            pool.Clear();
            spawnItem = null;
            onSpawn = null;
            onDespawn = null;
            capacity = 0;
            expireTime = 0;
            objectKey = ObjectPoolKey.None;
        }
        void Init(GameObject spawnItem, ObjectPoolKey objectKey)
        {
            this.spawnItem = spawnItem;
            pool = new Pool<GameObject>(capacity,
            () => { return GameObject.Instantiate(this.spawnItem); },
            (go) => { go.gameObject.SetActive(true); },
            (go) => { go.gameObject.SetActive(false); },
            (go) => { GameObject.Destroy(go); });
            this.objectKey = objectKey;
        }
        internal static ObjectPool Create(GameObject spawnItem, ObjectPoolKey objectKey)
        {
            var p= poolInctPool.Spawn();
            p.Init(spawnItem, objectKey);
            return p;
        }
        internal static void Release(ObjectPool objectPool)
        {
            poolInctPool.Despawn(objectPool);
        }
    }
}
