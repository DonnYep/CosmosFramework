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
        /// <summary>
        /// 当前池对象中的数量
        /// </summary>
        public int ObjectCount { get { return objectQueue.Count; } }
        /// <summary>
        /// 生成的对象
        /// </summary>
        Object spawnItem;
        Action<Object> onSpawn;
        Action<Object> onDespawn;
        Queue<Object> objectQueue;
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
        public void SetObjectHandler(Action<Object> onSpawn, Action<Object> onDespawn)
        {
            this.onDespawn = onDespawn;
            this.onSpawn = onSpawn;
        }
        public object Spawn()
        {
            Object go;
            if (objectQueue.Count > 0)
            {
                go = objectQueue.Dequeue();
            }
            else
            {
                go = GameObject.Instantiate(spawnItem) as GameObject;//实例化产生
            }
            onSpawn?.Invoke(go);//表示一个可空类型，空内容依旧可以执行
            return go;
        }
         public void Despawn(object targetGo)
        {
            var go = targetGo.Convert<Object>();
            if (ObjectCount >= capacity)
            {
                GameObject.Destroy(go);//超出部分被销毁
            }
            else
            {
                onDespawn?.Invoke(go);
                if (go == null)
                    return;
                objectQueue.Enqueue(go);//只有回收的时候会被加入列表
            }
        }
        public void ClearPool()
        {
            while (objectQueue.Count > 0)
            {
                var go = objectQueue.Dequeue();
                GameObject.Destroy(go);
            }
            objectQueue.Clear();
        }
        internal ObjectPool(object spawnItem)
        {
            this.spawnItem = spawnItem.Convert<Object>();
            objectQueue = new Queue<Object>();
        }
        internal void SetSpawnItem(Object spawnItem)
        {
            if (this.spawnItem != spawnItem)
            {
                this.spawnItem = spawnItem;
                ClearPool();
            }
        }
    }
}
