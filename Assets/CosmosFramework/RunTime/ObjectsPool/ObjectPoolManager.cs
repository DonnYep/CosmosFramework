using UnityEngine;
using System.Collections.Generic;
//using Cosmos.Event;
namespace Cosmos.ObjectPool
{
    [DisallowMultipleComponent]
    public sealed class ObjectPoolManager: Module<ObjectPoolManager>
    {
       public const  short _ObjectPoolCapacity = 50;
        Dictionary<object, ObjectSpawnPool> spawnPool = new Dictionary<object, ObjectSpawnPool>();
        GameObject activeObjectMount;
        /// <summary>
        /// 激活对象在场景中的容器，唯一
        /// </summary>
        public GameObject ActiveObjectMount
        {
            get
            {
                if (activeObjectMount == null)
                {
                    activeObjectMount = new GameObject("ObjectPoolModule->>ActiveObjectMount");
                    activeObjectMount.transform.RestWorldTransform();
                }
                return activeObjectMount;
            }
        }
        protected override void InitModule()
        {
            RegisterModule(CFModule.ObjectPool);
        }
        /// <summary>
        /// 注册对象池
        /// </summary>
        /// <param name="objKey"></param>
        /// <param name="spawnItem"></param>
        /// <param name="onSpawn"></param>
        /// <param name="onDespawn"></param>
        public void RegisterSpawnPool(object objKey,GameObject spawnItem,CFAction<GameObject> onSpawn,CFAction<GameObject> onDespawn)
        {
            if (!spawnPool.ContainsKey(objKey))
            {
                spawnPool.Add(objKey, new ObjectSpawnPool(spawnItem,onSpawn,onDespawn));
            }
        }
        /// <summary>
        /// 设置与更新生成的对象，测试时候使用
        /// </summary>
        /// <param name="objKey"></param>
        /// <param name="spawnItem"></param>
        public void SetSpawnItem(object objKey,GameObject spawnItem)
        {
            if (spawnPool.ContainsKey(objKey))
                spawnPool[objKey].SetSpawnItem(spawnItem);
            else
                Utility.DebugError(objKey.ToString() + "\n objKey not exist");
        }
        /// <summary>
        /// 注销对象池
        /// </summary>
        /// <param name="objKey"></param>
        public void DeregisterSpawnPool(object objKey)
        {
            if (spawnPool.ContainsKey(objKey))
            {
                spawnPool[objKey].Clear();
                spawnPool[objKey].ClearAction();
                spawnPool.Remove(objKey);
            }
        }
        /// <summary>
        /// 获取池中对象的个数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetPoolCount(object key)
        {
            if (spawnPool.ContainsKey(key))
            {
                return spawnPool[key].ObjectCount;
            }
            else
            {
                Utility.DebugError("Pool no register, null count", key as UnityEngine.Object);
                return -1;//未注册对象池则返回-1
            }
        }
        /// <summary>
        /// 产生对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject Spawn(object key)
        {
            if (spawnPool.ContainsKey(key))
            {
                return spawnPool[key].Spawn();
            }
            else
            {
                Utility.DebugError("Pool no register", key as UnityEngine.Object);
                return null;
            }
        }
        /// <summary>
        /// 回收单个对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="go"></param>
        public void Despawn(object key,GameObject go)
        {
            if (spawnPool.ContainsKey(key))
            {
                spawnPool[key].Despawn(go);
            }
            else
            {
                //如果对象没有key，则直接销毁
                Utility.DebugLog("Despawn fail ,pool not exist,Destroying it instead.!",MessageColor.purple, key as UnityEngine.Object);
                GameManager.KillObject(go);
            }
        }
        /// <summary>
        /// 批量回收对象，若有key，则失活，否则销毁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gos"></param>
        public void Despawns(object key,GameObject[] gos)
        {
            if (spawnPool.ContainsKey(key))
            {
                for (int i = 0; i < gos.Length; i++)
                {
                    spawnPool[key].Despawn(gos[i]);
                }
            }
            else
            {
                Utility.DebugLog("Despawn fail ,pool not exist,Destroying it instead.!",MessageColor.purple, key as UnityEngine.Object);
                for (int i = 0; i < gos.Length; i++)
                {
                    GameManager.KillObject(gos[i]);
                }
            }
        }
        /// <summary>
        /// 清空指定对象池
        /// </summary>
        /// <param name="key"></param>
        public void Clear(object key)
        {
            if (spawnPool.ContainsKey(key))
            {
                spawnPool[key].Clear();
            }
            else
            {
                Utility.DebugError("clear fail , pool not exist!", key as UnityEngine.Object);
            }
        }
        /// <summary>
        /// 清除所有池对象
        /// </summary>
        public void ClearAll()
        {
            foreach (var pool in spawnPool)
            {
                pool.Value.Clear();
            }
        }
        /// <summary>
        /// 生成对象，不经过池。用于一次性的对象产生
        /// </summary>
        public GameObject SpawnNotUsePool(GameObject go, Transform spawnTransform)
        {
            return GameObject.Instantiate(go, spawnTransform);
        }
    }
}