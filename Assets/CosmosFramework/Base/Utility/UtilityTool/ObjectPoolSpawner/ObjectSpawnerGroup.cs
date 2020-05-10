using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class ObjectSpawnerGroup : ObjectSpawner
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
                        Facade.Instance.SetObjectSpawnItem(SpawnObjectGroup[i].SpawnTransform, SpawnObjectGroup[i].PoolObject.SpawnObject);
                        var go = Facade.Instance.SpawnObject(SpawnObjectGroup[i].SpawnTransform);
                        AlignObject(SpawnObjectGroup[i].PoolObject.AlignType, go, SpawnObjectGroup[i].SpawnTransform);
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
                    Facade.Instance.RegisterObjcetSpawnPool(SpawnObjectGroup[i].SpawnTransform, SpawnObjectGroup[i].PoolObject.SpawnObject,
                      SpawnHandler, DespawnHandler);
                }
            }
        }
        protected override void DeregisterSpawner()
        {
            for (int i = 0; i < SpawnObjectGroup.Count; i++)
            {
                if (SpawnObjectGroup[i].SpawnTransform != null && SpawnObjectGroup[i].PoolObject != null)
                {
                    Facade.Instance.DeregisterObjectSapwnPool(SpawnObjectGroup[i].SpawnTransform);
                }
            }
            GameManager.KillObject(deactiveObjectMount);
        }
        protected override void SpawnHandler(GameObject go)
        {
            if (go == null)
                return;
            Facade.Instance.StartCoroutine(EnumCollect(SpawnObjectGroup[flag].CollectDelay,
                (tempFlag) => { Facade.Instance.DespawnObject(SpawnObjectGroup[Utility.Converter.Int( tempFlag)].SpawnTransform, go); },flag));
        }
        protected IEnumerator EnumCollect(float delay, CFAction<object> action ,object arg)
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
        [System.Serializable]
        class ObjectGroup
        {
            [SerializeField]
            Transform spawnTransform;
            public Transform SpawnTransform { get { return spawnTransform; } }
            [SerializeField]
            ObjectPoolDataSet poolDataSet;
            public ObjectPoolDataSet PoolObject { get { return poolDataSet; } }
            public float CollectDelay { get { return poolDataSet.CollectDelay; } }
        }
    }
}