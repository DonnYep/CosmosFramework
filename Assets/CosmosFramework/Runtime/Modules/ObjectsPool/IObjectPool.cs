using System;
using UnityEngine;
namespace Cosmos.ObjectPool
{
    public interface IObjectPool
    {
        event Action<GameObject> OnObjectSpawn;
        event Action<GameObject> OnObjectDespawn;
        int ExpireTime { get; set; }
        int ReleaseInterval { get; set; }
        int Capacity { get; set; }
        string ObjectPoolName { get; }
        /// <summary>
        /// 生成对象，默认执行gameObject.SetActive(true)操作
        /// </summary>
        /// <returns>生成的对象</returns>
        GameObject Spawn();
        /// <summary>
        /// 回收对象，默认执行gameObject.SetActive(false)操作
        /// </summary>
        /// <param name="go">回收的对象</param>
        void Despawn(GameObject go);
        /// <summary>
        /// 清除池中的缓存对象引用
        /// </summary>
        void ClearPool();
        void OnElapseRefresh(float deltatime);
    }
}
