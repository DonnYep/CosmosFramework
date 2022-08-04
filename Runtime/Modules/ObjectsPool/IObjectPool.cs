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
        GameObject Spawn();
        void Despawn(GameObject go);
        void ClearPool();
        void OnElapseRefresh(float deltatime);
    }
}
