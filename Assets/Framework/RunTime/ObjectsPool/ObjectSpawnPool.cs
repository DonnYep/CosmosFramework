using UnityEngine;
using System.Collections.Generic;
using Cosmos.Event;
namespace Cosmos.ObjectPool
{
    public sealed class ObjectSpawnPool
    {
        /// <summary>
        /// 生成的对象
        /// </summary>
        GameObject spawnItem;
        CFAction<GameObject> onSpawn;
        CFAction<GameObject> onDespawn;
        List<GameObject> objectList = new List<GameObject>();
        public ObjectSpawnPool(GameObject spawnItem, CFAction<GameObject> onSpawn, CFAction<GameObject>onDespawn)
        {
            this.spawnItem = spawnItem;
            this.onSpawn = onSpawn;
            this.onDespawn = onDespawn;
        }
        public void SetSpawnItem(GameObject spawnItem)
        {
            if (this.spawnItem != spawnItem)
            {
                this.spawnItem = spawnItem;
                Clear();
            }
        }
        public void ClearAction()
        {
            this.onDespawn = null;
            this.onSpawn = null;
        }
        /// <summary>
        /// 当前池对象中的数量
        /// </summary>
        public int ObjectCount { get { return objectList.Count; } }
        public GameObject Spawn()
        {
            GameObject go;
            if (objectList.Count > 0)
            {
                go = FindUseable();
                if(go!=null)
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
        /// <summary>
        /// 查找到不活跃的对象返回
        /// </summary>
        /// <returns></returns>
        GameObject FindUseable()
        {
            return objectList.Find(g => !g.activeSelf);
        }
        public void Despawn(GameObject go)
        {
            if (ObjectCount >= ObjectPoolManager._ObjectPoolCapacity)
            {
                GameManager.KillObject(go);//超出部分被销毁
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
        public void Clear()
        {
            while (objectList.Count > 0)
            {
                GameObject go = objectList[0];
                objectList.RemoveAt(0);
                GameManager.KillObject(go);
            }
        }
    }
}
