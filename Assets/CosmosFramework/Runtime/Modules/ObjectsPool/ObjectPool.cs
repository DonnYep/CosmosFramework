using UnityEngine;
using System.Collections.Generic;
using System;
using Cosmos.Event;
using Object = UnityEngine.Object;
namespace Cosmos.ObjectPool
{
    internal sealed class ObjectPool: IObjectPool
    {
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
        public Type ObjectType { get { return objectKey.Type; } }
        public string Name { get { return objectKey.String; } }
        TypeStringPair objectKey;
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
        public void OnElapseRefresh(long msNow)
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
        internal ObjectPool(GameObject spawnItem, TypeStringPair objectKey)
        {
            this.spawnItem = spawnItem;
            pool = new Pool<GameObject>(capacity,
            () => { return GameObject.Instantiate(this.spawnItem); },
            (go) => { go.gameObject.SetActive(true); },
            (go) => { go.gameObject.SetActive(false); },
            (go) => { GameObject.Destroy(go); });
            this.objectKey = objectKey;
        }
    }
}
