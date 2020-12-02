using UnityEngine;
using System.Collections.Generic;
using System;
namespace Cosmos.ObjectPool
{
    [Module]
    internal sealed class ObjectPoolManager : Module, IObjectPoolManager
    {
        #region Properties
        internal static readonly short _ObjectPoolCapacity = 150;
        Dictionary<object, ObjectSpawnPool> spawnPoolDict = new Dictionary<object, ObjectSpawnPool>();
        GameObject activeObjectMount;
        /// <summary>
        /// 激活对象在场景中的容器，唯一
        /// </summary>
        internal GameObject ActiveObjectMount
        {
            get
            {
                if (activeObjectMount == null)
                {
                    activeObjectMount = new GameObject("ObjectPoolModule->>ActiveObjectMount");
                    activeObjectMount.transform.ResetWorldTransform();
                    activeObjectMount.transform.SetParent(MountPoint.transform);
                }
                return activeObjectMount;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 注册对象池
        /// </summary>
        /// <param name="objKey"></param>
        /// <param name="spawnItem"></param>
        /// <param name="onSpawn"></param>
        /// <param name="onDespawn"></param>
        public void RegisterSpawnPool(object objKey, GameObject spawnItem, Action<GameObject> onSpawn, Action<GameObject> onDespawn)
        {
            if (!spawnPoolDict.ContainsKey(objKey))
            {
                spawnPoolDict.Add(objKey, new ObjectSpawnPool(spawnItem, onSpawn, onDespawn));
            }
        }
        /// <summary>
        /// 设置与更新生成的对象，测试时候使用
        /// </summary>
        /// <param name="objKey"></param>
        /// <param name="spawnItem"></param>
        public void SetSpawnItem(object objKey, GameObject spawnItem)
        {
            if (spawnPoolDict.ContainsKey(objKey))
                spawnPoolDict[objKey].SetSpawnItem(spawnItem);
            else
                throw new ArgumentNullException("ObjectPoolManager\n" + objKey.ToString() + "\n objKey not exist");
        }
        /// <summary>
        /// 注销对象池
        /// </summary>
        /// <param name="objKey"></param>
        public void DeregisterSpawnPool(object objKey)
        {
            if (spawnPoolDict.ContainsKey(objKey))
            {
                spawnPoolDict[objKey].Clear();
                spawnPoolDict[objKey].ClearAction();
                spawnPoolDict.Remove(objKey);
            }
        }
        /// <summary>
        /// 获取池中对象的个数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetPoolCount(object key)
        {
            if (spawnPoolDict.ContainsKey(key))
            {
                return spawnPoolDict[key].ObjectCount;
            }
            else
            {
                Utility.Debug.LogError("ObjectPoolManager-->>" + "Pool no register, null count", key as UnityEngine.Object);
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
            if (spawnPoolDict.ContainsKey(key))
            {
                return spawnPoolDict[key].Spawn();
            }
            else
            {
                Utility.Debug.LogError("ObjectPoolManager-->>" + "Pool not be registered", key as UnityEngine.Object);
                return null;
            }
        }
        /// <summary>
        /// 回收单个对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="go"></param>
        public void Despawn(object key, GameObject go)
        {
            if (spawnPoolDict.ContainsKey(key))
            {
                spawnPoolDict[key].Despawn(go);
            }
            else
            {
                //如果对象没有key，则直接销毁
                Utility.Debug.LogInfo("ObjectPoolManager\n" + "Despawn fail ,pool not exist,Destroying it instead.!", MessageColor.PURPLE, key as UnityEngine.Object);
                GameManager.KillObject(go);
            }
        }
        /// <summary>
        /// 批量回收对象，若有key，则失活，否则销毁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gos"></param>
        public void Despawns(object key, GameObject[] gos)
        {
            if (spawnPoolDict.ContainsKey(key))
            {
                for (int i = 0; i < gos.Length; i++)
                {
                    spawnPoolDict[key].Despawn(gos[i]);
                }
            }
            else
            {
                Utility.Debug.LogInfo("ObjectPoolManager\n" + "Despawn fail ,pool not exist,Destroying it instead.!", MessageColor.PURPLE, key as UnityEngine.Object);
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
            if (spawnPoolDict.ContainsKey(key))
            {
                spawnPoolDict[key].Clear();
            }
            else
            {
                Utility.Debug.LogError("ObjectPoolManager-->>" + "clear fail , pool not exist!", key as UnityEngine.Object);
            }
        }
        /// <summary>
        /// 清除所有池对象
        /// </summary>
        public void ClearAll()
        {
            foreach (var pool in spawnPoolDict)
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
        #endregion
    }
}