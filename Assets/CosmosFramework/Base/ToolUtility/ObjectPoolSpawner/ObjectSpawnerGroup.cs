using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class ObjectSpawnerGroup : ObjectSpawner
    {
        [Header("能用，慎用")]
        [SerializeField] List<ObjectGroup> spawnObjectGroup = new List<ObjectGroup>();
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
                        if (SpawnObjectGroup[i].SpawnTransform != null)
                            go.transform.position = SpawnObjectGroup[i].SpawnTransform.position;
                        go.transform.SetParent(ActiveObjectMount);
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
                (tempFlag) => { Facade.Instance.DespawnObject(SpawnObjectGroup[Utility.Int( tempFlag)].SpawnTransform, go); },flag));
        }
        protected IEnumerator EnumCollect(float delay, CFAction<object> action ,object arg)
        {
            int tempFlag = Utility.Int(arg);
            yield return new WaitForSeconds(delay);
            action?.Invoke(tempFlag);
        }
        [System.Serializable]
        class ObjectGroup
        {
            [SerializeField]
            Transform spawnTransform;
            public Transform SpawnTransform { get { return spawnTransform; } }
            [SerializeField]
            ObjectPoolEventObject poolObject;
            public ObjectPoolEventObject PoolObject { get { return poolObject; } }
            [SerializeField] protected float collectDelay = 3;
            public float CollectDelay { get { if (collectDelay <= 0.1) collectDelay = 0.1f; return collectDelay; } }
        }
    }
}