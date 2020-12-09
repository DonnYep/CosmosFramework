using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.ObjectPool;
using Cosmos.Mono;

namespace Cosmos{
    public class MonoObjectSpawnerGroup : MonoObjectSpawner
    {
        [Header("能用，慎用")]
        [SerializeField] List<ObjectGroup> spawnObjectGroup = new List<ObjectGroup>();
        HashSet<GameObject> uncollectibleHashSet = new HashSet<GameObject>();
        public override HashSet<GameObject> UncollectibleHashSet { get { return uncollectibleHashSet; } protected set { uncollectibleHashSet = value; } }
        /// <summary>
        /// 只读类型
        /// </summary>
        List<ObjectGroup> SpawnObjectGroup { get { return spawnObjectGroup; } }
        //标志位，存储当前group中的
        int flag = 0;
        IObjectPoolManager objectPoolManager;
        IMonoManager monoManager;
        public override void Spawn()
        {
            for (int i = 0; i < SpawnObjectGroup.Count; i++)
            {
                if (SpawnObjectGroup[i].SpawnTransform != null && SpawnObjectGroup[i].PoolObject != null)
                {
                    if (!SpawnObjectGroup[i].PoolObject.ObjectAddsResult)
                        return;
                    flag = i;
                    for (int j = 0; j < SpawnObjectGroup[i].PoolObject.SpawnCount; j++)
                    {
                        //objectPoolManager.SetSpawnItem(SpawnObjectGroup[i].SpawnTransform, SpawnObjectGroup[i].PoolObject.SpawnObject);
                        //var go = objectPoolManager.Spawn(SpawnObjectGroup[i].SpawnTransform);
                        //AlignObject(SpawnObjectGroup[i].PoolObject.AlignType, go, SpawnObjectGroup[i].SpawnTransform);
                    }
                }
            }
        }
        protected override void RegisterSpawner()
        {
            for (int i = 0; i < SpawnObjectGroup.Count; i++)
            {
                if(SpawnObjectGroup[i].SpawnTransform!=null&&SpawnObjectGroup[i].PoolObject!=null)
                {
                    //objectPoolManager.RegisterSpawnPool(SpawnObjectGroup[i].SpawnTransform, SpawnObjectGroup[i].PoolObject.SpawnObject,
                    //  SpawnHandler, DespawnHandler);
                }
            }
        }
        protected override void DeregisterSpawner()
        {
            for (int i = 0; i < SpawnObjectGroup.Count; i++)
            {
                if (SpawnObjectGroup[i].SpawnTransform != null && SpawnObjectGroup[i].PoolObject != null)
                {
                    //objectPoolManager.DeregisterSpawnPool(SpawnObjectGroup[i].SpawnTransform);
                }
            }
            GameObject.Destroy(deactiveObjectMount);
        }
        protected override void SpawnHandler(GameObject go)
        {
            if (go == null)
                return;
            //monoManager.StartCoroutine(EnumCollect(SpawnObjectGroup[flag].CollectDelay,
            //    (tempFlag) => { objectPoolManager.Despawn(SpawnObjectGroup[Utility.Converter.Int( tempFlag)].SpawnTransform, go); },flag));
        }
        protected IEnumerator EnumCollect(float delay, Action<object> action ,object arg)
        {
            int tempFlag = Utility.Converter.Int(arg);
            yield return new WaitForSeconds(delay);
            action?.Invoke(tempFlag);
        }
        public override void SpawnUncollectible()
        {
            for (int i = 0; i < SpawnObjectGroup.Count; i++)
            {
                if (SpawnObjectGroup[i].SpawnTransform != null && SpawnObjectGroup[i].PoolObject != null)
                {
                    if (!SpawnObjectGroup[i].PoolObject.ObjectAddsResult)
                        return;
                    flag = i;
                    for (int j = 0; j < SpawnObjectGroup[i].PoolObject.SpawnCount; j++)
                    {
                        var go = GameObject.Instantiate(SpawnObjectGroup[i].PoolObject.SpawnObject);
                        AlignObject(SpawnObjectGroup[i].PoolObject.AlignType, go, SpawnObjectGroup[i].SpawnTransform);
                        uncollectibleHashSet.Add(go);
                    }
                }
            }
        }
        protected override void Start()
        {
            base.Start();
            objectPoolManager = GameManager.GetModule<IObjectPoolManager>();
            monoManager = GameManager.GetModule<IMonoManager>();
        }
        [System.Serializable]
        class ObjectGroup
        {
            [SerializeField]
            Transform spawnTransform;
            public Transform SpawnTransform { get { return spawnTransform; } }
            [SerializeField]
            ObjectPoolDataset poolDataSet;
            public ObjectPoolDataset PoolObject { get { return poolDataSet; } }
            public float CollectDelay { get { return poolDataSet.CollectDelay; } }
        }
    }
}