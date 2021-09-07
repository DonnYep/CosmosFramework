using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos
{
    public interface IObjectPool 
    {
        int ExpireTime { get; set; }
        int ReleaseInterval { get; set; }
        int Capacity { get; set; }
        Type ObjectType { get; }
        string Name { get; }
        /// <summary>
        /// 设置回调；
        /// 此回调在对象被生成、回收时触发；
        /// </summary>
        /// <param name="onSpawn">生成时的回调</param>
        /// <param name="onDespawn">回收时的回调</param>
        void SetCallback(Action<GameObject> onSpawn, Action<GameObject> onDespawn);
        GameObject Spawn();
        void Despawn(object go);
        void ClearPool();
        void OnElapseRefresh(float deltatime);
    }
}
