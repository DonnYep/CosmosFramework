using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class MonoObjectSpawnerMultiple : MonoObjectSpawner
    {
        [ContextMenuItem("Reset","ResetSpanwer")]
        [SerializeField] List<Transform> spawnTransforms = new List<Transform>();
        public List<Transform> SpawnTransforms { get { return spawnTransforms; } }
        [SerializeField]
        ObjectPoolDataSet poolDataSet;
        public ObjectPoolDataSet PoolDataSet { get { return poolDataSet; } }
        public override float CollectDelay { get { return poolDataSet.CollectDelay; } }
        HashSet<MonoObjectBase> uncollectibleHashSet = new HashSet<MonoObjectBase>();
        public override HashSet<MonoObjectBase> UncollectibleHashSet { get { return uncollectibleHashSet; }protected set { uncollectibleHashSet = value; } }
        public override void Spawn()
        {
            for (int i = 0; i < SpawnTransforms.Count; i++)
            {
                if (!poolDataSet.ObjectAddsResult)
                    return;
                for (int j = 0; j < poolDataSet.SpawnCount; j++)
                {
                    //Facade.SetObjectSpawnItem(objectKey, PoolDataSet.SpawnObject);
                    poolVariable.SpawnItem = PoolDataSet.SpawnObject;
                    var go = Facade.SpawnObject(objectKey);
                    AlignObject(PoolDataSet.AlignType, go, SpawnTransforms[i]);
                }
            }
        }
        protected override void Awake()
        {
            Utility.DebugLog(" 初始化 MonoObjectSpawnerMultiple；当前此工具不可用，需要修改多对象生成的数据结构", MessageColor.RED);
        }
        protected override void OnDestroy()
        {
            Utility.DebugLog(" 销毁 MonoObjectSpawnerMultiple；当前此工具不可用，需要修改多对象生成的数据结构", MessageColor.RED);
        }
        protected override void RegisterSpawner()
        {
            if (poolDataSet != null)
            {
                objectKey = new ObjectKey<MonoObjectItem>(this.GetType().FullName);
                poolVariable = new ObjectPoolVariable(PoolDataSet.SpawnObject, SpawnHandler, DespawnHandler, Create);
                Facade.RegisterObjcetSpawnPool(objectKey, poolVariable);
            }
        }
        //TODO不可回收的对象
        public override void SpawnUncollectible()
        {
            for (int i = 0; i < SpawnTransforms.Count; i++)
            {
                if (!poolDataSet.ObjectAddsResult)
                    return;
                for (int j = 0; j < poolDataSet.SpawnCount; j++)
                {
                    var go = GameObject.Instantiate(PoolDataSet.SpawnObject);
                    AlignObject(PoolDataSet.AlignType, go, SpawnTransforms[i]);
                    uncollectibleHashSet.Add(go);
                }
            }
        }

        protected override MonoObjectBase Create()
        {
            throw new System.NotImplementedException();
        }
    }
}