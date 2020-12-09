using UnityEngine;
using System.Collections.Generic;
using System;
using Cosmos.Event;
namespace Cosmos.ObjectPool
{
    internal sealed class ObjectSpawnPool: IObjectSpawnPool
    {
        /// <summary>
        /// 生成的对象
        /// </summary>
        GameObject spawnItem;
        Action<GameObject> onSpawn;
        Action<GameObject> onDespawn;
        List<GameObject> objectList = new List<GameObject>();
        /// <summary>
        /// 对象生成后的过期时间；
        /// </summary>
        int expireTime;
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
        int releaseInterval = 5;
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
        int capacity;
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
        public void OnElapseRefresh(long msNow)
        {
            if (expireTime <= 0)
                return;
        }
        public void SetObjectHandler(Action<UnityEngine.GameObject> onSpawn, Action<UnityEngine.GameObject> onDespawn)
        {
            this.onDespawn = onDespawn;
            this.onSpawn = onSpawn;
        }
        internal ObjectSpawnPool(object spawnItem) 
        {
            this.spawnItem = spawnItem.Convert<GameObject>();
        }
        internal void SetSpawnItem(GameObject spawnItem)
        {
            if (this.spawnItem != spawnItem)
            {
                this.spawnItem = spawnItem;
                Clear();
            }
        }
        internal void ClearAction()
        {
            this.onDespawn = null;
            this.onSpawn = null;
        }
        /// <summary>
        /// 当前池对象中的数量
        /// </summary>
        internal int ObjectCount { get { return objectList.Count; } }
        internal GameObject Spawn()
        {
            GameObject go;
            if (objectList.Count > 0)
            {
                go = FindUseable();
                if (go != null)
                    objectList.Remove(go);//从数组中移除
            }
            else
            {
                go = GameObject.Instantiate(spawnItem) as GameObject;//实例化产生
            }
            go.SetActive(true);
            onSpawn?.Invoke(go);//表示一个可空类型，空内容依旧可以执行
            return go;
        }
        internal object[] Spawns(int count)
        {
            List<object> objList = new List<object>();
            for (int i = 0; i < count; i++)
            {
                objectList.Add( Spawn());
            }
            return objectList.ToArray();
        }
        /// <summary>
        /// 查找到不活跃的对象返回
        /// </summary>
        /// <returns></returns>
        GameObject FindUseable()
        {
            return objectList.Find(g => !g.activeSelf);
        }
        internal void Despawn(object targetGo)
        {
            var go = targetGo.Convert<GameObject>();
            if (ObjectCount >= capacity)
            {
                GameObject.Destroy(go);//超出部分被销毁
            }
            else
            {
                onDespawn?.Invoke(go);
                if (go == null)
                    return;
                go.SetActive(false);
                objectList.Add(go);//只有回收的时候会被加入列表
            }
        }
        internal void Despawns(object[] targetGos)
        {
            var length = targetGos.Length;
            for (int i = 0; i < length; i++)
            {
                Despawn(targetGos[i]);
            }
        }
        internal void Clear()
        {
            while (objectList.Count > 0)
            {
                GameObject go = objectList[0];
                GameObject.Destroy(go);
            }
            objectList.Clear();
        }

    }
}
