//====================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos
{
    public interface IObjectPoolManager: IModuleManager
    {
        void RegisterSpawnPool(object objKey, GameObject spawnItem, Action<GameObject> onSpawn, Action<GameObject> onDespawn);
        /// <summary>
        /// 设置与更新生成的对象，测试时候使用
        /// </summary>
        /// <param name="objKey"></param>
        /// <param name="spawnItem"></param>
        void SetSpawnItem(object objKey, GameObject spawnItem);
        /// <summary>
        /// 注销对象池
        /// </summary>
        /// <param name="objKey"></param>
        void DeregisterSpawnPool(object objKey);
        /// <summary>
        /// 获取池中对象的个数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int GetPoolCount(object key);
        /// <summary>
        /// 产生对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        GameObject Spawn(object key);
        /// <summary>
        /// 回收单个对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="go"></param>
        void Despawn(object key, GameObject go);
        /// <summary>
        /// 批量回收对象，若有key，则失活，否则销毁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gos"></param>
        void Despawns(object key, GameObject[] gos);
        /// <summary>
        /// 清空指定对象池
        /// </summary>
        /// <param name="key"></param>
        void Clear(object key);
        /// <summary>
        /// 清除所有池对象
        /// </summary>
        void ClearAll();
        /// <summary>
        /// 生成对象，不经过池。用于一次性的对象产生
        /// </summary>
        GameObject SpawnNotUsePool(GameObject go, Transform spawnTransform);
    }
}
