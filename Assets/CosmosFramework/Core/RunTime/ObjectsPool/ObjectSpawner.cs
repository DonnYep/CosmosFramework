using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.ObjectPool;
namespace Cosmos
{
    public abstract class ObjectSpawner: IObjectSpawner
    {
        public string SpawnObjectPoolKey { get; set; }
        public GameObject SpawnItem { get; set; }
        public GameObject ActivatedObjectMounting { get; set; }
        public GameObject DeactivatedObjectMounting { get; set; }
        IObjectPoolManager objectPoolManager;
        public void SetSpawnData(string key, GameObject spawnItem)
        {
            SpawnObjectPoolKey = key;
            SpawnItem = spawnItem;
        }
        public void SetObjectMounting(GameObject activated, GameObject deactivated)
        {
            ActivatedObjectMounting = activated;
            DeactivatedObjectMounting = deactivated;
        }
        /// <summary>
        /// 注册对象池
        /// 非空虚函数
        /// </summary>
        protected virtual void RegisterObjectSpawnPool()
        {
            objectPoolManager.RegisterSpawnPool(SpawnObjectPoolKey, SpawnItem, OnSpawn, OnDespawn);
        }
        /// <summary>
        /// 注销对象池
        /// 非空虚函数
        /// </summary>
        protected virtual void DeregisterObjectSpawnPool()
        {
            objectPoolManager.DeregisterSpawnPool(SpawnObjectPoolKey);
        }
        /// <summary>
        /// 生成时触发的方法
        /// 空虚函数
        /// </summary>
        public virtual void OnSpawn(GameObject go) { }
        /// <summary>
        /// 回收时触发的方法
        /// 空虚函数
        /// </summary>
        public virtual void OnDespawn(GameObject go) { }
        /// <summary>
        /// 非空虚函数
        /// </summary>
        public virtual void OnInitialization()
        {
            RegisterObjectSpawnPool();
        }
        /// <summary>
        /// 非空虚函数
        /// </summary>
        public virtual void OnTermination()
        {
            DeregisterObjectSpawnPool();
            SpawnItem = null;
            SpawnObjectPoolKey = null;
        }
    }
}