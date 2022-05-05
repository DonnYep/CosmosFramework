using UnityEngine;
using System;
namespace Cosmos.ObjectPool
{
    internal sealed class ObjectPool : IObjectPool
    {
        static readonly Pool<ObjectPool> poolInstPool
            = new Pool<ObjectPool>
            (
                () => { return new ObjectPool(); },
                p => { p.Release(); }
            );

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
        public string ObjectPoolName { get; private set; }
        public ObjectPoolAssetInfo ObjectPoolAssetInfo { get; private set; }
        /// <summary>
        /// 当前池对象中的数量
        /// </summary>
        public int ObjectCount { get { return pool.Count; } }
        /// <summary>
        /// 生成的对象
        /// </summary>
        /// 
        GameObject spawnAsset;

        Pool<GameObject> pool;

        Action<GameObject> onObjectSpawn;
        Action<GameObject> onObjectDespawn;
        public event Action<GameObject> OnObjectSpawn
        {
            add { onObjectSpawn += value; }
            remove { onObjectSpawn -= value; }
        }
        public event Action<GameObject> OnObjectDespawn
        {
            add { onObjectDespawn += value; }
            remove { onObjectDespawn -= value; }
        }

        int expireTime;
        int capacity;
        int releaseInterval = 5;
        private ObjectPool() { }
        public void OnElapseRefresh(float deltatime)
        {
            if (expireTime <= 0)
                return;
        }
        public GameObject Spawn()
        {
            var go = pool.Spawn();
            while (go == null)
            {
                pool.Spawn();
            }
            onObjectSpawn?.Invoke(go);
            return go;
        }
        public void Despawn(GameObject go)
        {
            if (go == null)
                return;
            onObjectDespawn?.Invoke(go);
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
            spawnAsset = null;
            onObjectSpawn = null;
            onObjectDespawn = null;
            capacity = 0;
            expireTime = 0;
            ObjectPoolName = string.Empty;
            ObjectPoolAssetInfo = null;
        }
        void Init(GameObject spawnAsset, string objectPoolName, ObjectPoolAssetInfo assetInfo)
        {
            this.spawnAsset = spawnAsset;
            pool = new Pool<GameObject>(capacity,
            () => { return GameObject.Instantiate(this.spawnAsset); },
            (go) => { go.gameObject.SetActive(true); },
            (go) => { go.gameObject.SetActive(false); },
            (go) => { GameObject.Destroy(go); });
            this.ObjectPoolName = objectPoolName;
            this.ObjectPoolAssetInfo = assetInfo;
        }
        internal static ObjectPool Create(GameObject spawnAsset, string objectPoolName, ObjectPoolAssetInfo assetInfo)
        {
            var p = poolInstPool.Spawn();
            p.Init(spawnAsset, objectPoolName, assetInfo);
            return p;
        }
        internal static void Release(ObjectPool objectPool)
        {
            poolInstPool.Despawn(objectPool);
        }
    }
}
