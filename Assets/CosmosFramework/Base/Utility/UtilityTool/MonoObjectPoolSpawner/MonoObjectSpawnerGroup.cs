using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class MonoObjectSpawnerGroup : MonoObjectSpawner
    {
        [Header("能用，慎用")]
        [SerializeField] List<ObjectGroup> spawnObjectGroup = new List<ObjectGroup>();
        HashSet<MonoObjectBase> uncollectibleHashSet = new HashSet<MonoObjectBase>();
        public override HashSet<MonoObjectBase> UncollectibleHashSet { get { return uncollectibleHashSet; } protected set { uncollectibleHashSet = value; } }
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
                            //Facade.SetObjectSpawnItem(SpawnObjectGroup[i].SpawnTransform, SpawnObjectGroup[i].PoolObject.SpawnObject);
                            //var go = Facade.SpawnObject(SpawnObjectGroup[i].SpawnTransform);
                            //AlignObject(SpawnObjectGroup[i].PoolObject.AlignType, go, SpawnObjectGroup[i].SpawnTransform);
                    }
                }
            }
        }
        protected override void Awake()
        {
            Utility.DebugLog(" 初始化 MonoObjectSpawnerGroup；当前此工具不可用，需要修改多对象生成的数据结构",MessageColor.RED);
        }
        protected override void OnDestroy()
        {
            Utility.DebugLog(" 销毁 MonoObjectSpawnerGroup；当前此工具不可用，需要修改多对象生成的数据结构", MessageColor.RED);
        }
        protected override void RegisterSpawner()
        {
            for (int i = 0; i < SpawnObjectGroup.Count; i++)
            {
                if(SpawnObjectGroup[i].SpawnTransform!=null&&SpawnObjectGroup[i].PoolObject!=null)
                {
                    //Facade.RegisterObjcetSpawnPool(SpawnObjectGroup[i].SpawnTransform, SpawnObjectGroup[i].PoolObject.SpawnObject,
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
                    //Facade.DeregisterObjectSpawnPool(SpawnObjectGroup[i].SpawnTransform);
                }
            }
            GameManager.KillObject(deactiveObjectMount);
        }
        protected override void SpawnHandler(IObject go)
        {
            if (go == null)
                return;
            //Facade.StartCoroutine(EnumCollect(SpawnObjectGroup[flag].CollectDelay,
            //    (tempFlag) => { Facade.DespawnObject(SpawnObjectGroup[Utility.Converter.Int( tempFlag)].SpawnTransform, go); },flag));
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
                        //var go = GameObject.Instantiate(SpawnObjectGroup[i].PoolObject.SpawnObject);
                        //AlignObject(SpawnObjectGroup[i].PoolObject.AlignType, go, SpawnObjectGroup[i].SpawnTransform);
                        //uncollectibleHashSet.Add(go);
                    }
                }
            }
        }

        protected override MonoObjectBase Create()
        {
            throw new System.NotImplementedException();
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