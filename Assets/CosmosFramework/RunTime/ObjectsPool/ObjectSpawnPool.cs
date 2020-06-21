using System;
using System.Collections.Generic;
namespace Cosmos.ObjectPool
{
    public sealed class ObjectSpawnPool
    {
        ObjectPoolVariable poolData;
         Queue<IObject> objectList = new Queue<IObject>();
        public ObjectSpawnPool(ObjectPoolVariable poolData)
        {
            this.poolData = poolData;
        }
        public void SetSpawnItem(IObject spawnItem)
        {
            if (poolData == null)
                poolData = new ObjectPoolVariable();
            if (poolData. SpawnItem != spawnItem)
            {
                this.poolData.SpawnItem=spawnItem;
                Clear();
            }
        }
        public void ClearAction()
        {
            poolData.Clear();
        }
        /// <summary>
        /// 当前池对象中的数量
        /// </summary>
        public int ObjectCount { get { return objectList.Count; } }
        public IObject  Spawn()
        {
            IObject go=default;
            if (objectList.Count > 0)
            {
                go =objectList.Dequeue();
            }
            else
            {
                go =poolData.CreateHandler?.Invoke();
            }
            if (go == null)
                throw new ArgumentNullException("ObjectSpawnPool : can't create IObjectItem !");
           poolData.OnSpawnHandler?.Invoke(go);//表示一个可空类型，空内容依旧可以执行
            go.OnSpawn();
            return go;
        }
        public void Despawn( IObject go)
        {
            if (ObjectCount >= ObjectPoolManager.OBJECT_POOL_CAPACITY)
            {
                go.OnTermination();
            }
            else
            {
              poolData. OnDespawnHandler?.Invoke(go);
                if (go == null)
                    return;
                go.OnDespawn();
                objectList.Enqueue(go);//只有回收的时候会被加入列表
            }
        }
        public void Despawns(IObject[] gos)
        {
            int length = gos.Length;
            if (ObjectCount >= ObjectPoolManager.OBJECT_POOL_CAPACITY)
            {
                for (int i = 0; i < length; i++)
                {
                    gos[i].OnTermination();
                }
            }
            else
            {
                int overflowCount = ObjectCount + length - ObjectPoolManager.OBJECT_POOL_CAPACITY;
                int residualCount = ObjectPoolManager.OBJECT_POOL_CAPACITY - ObjectCount;
                if (overflowCount>0)
                {
                    for (int i = 0; i < residualCount; i++)
                    {
                        poolData.OnDespawnHandler?.Invoke(gos[i]);
                        if (gos[i] == null)
                            return;
                        gos[i].OnDespawn();
                        objectList.Enqueue(gos[i]);//只有回收的时候会被加入列表
                    }
                    for (int i = 0; i < length; i++)
                    {
                        gos[i].OnTermination();
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++)
                    {
                        poolData.OnDespawnHandler?.Invoke(gos[i]);
                        if (gos[i] == null)
                            return;
                        gos[i].OnDespawn();
                        objectList.Enqueue(gos[i]);//只有回收的时候会被加入列表
                    }
                }
            }
        }
        public void Clear()
        {
            while (objectList.Count > 0)
            {
                var go = objectList.Dequeue();
                go.OnTermination();
            }
        }
    }
}
