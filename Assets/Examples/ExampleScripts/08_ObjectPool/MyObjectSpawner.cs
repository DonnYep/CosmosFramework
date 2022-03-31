using System.Collections.Generic;
using UnityEngine;
using Cosmos.ObjectPool;
namespace Cosmos.Test
{
    /// <summary>
    /// 使用框架中的对象池，有相当多参数需要自定义维护，自定义可控性更高；
    /// 相对易维护的方式是使用实体Entity模块；
    /// </summary>
    public class MyObjectSpawner: MonoBehaviour
    {
        [SerializeField] Transform spawnRoot;
        [SerializeField] Transform despawnRoot;

        readonly string resCube = "ResPrefab_Cube";
        readonly string resSphere = "ResPrefab_Sphere";
        readonly string resCapsule = "ResPrefab_Capsule";
        IObjectPoolManager objectPoolManager;

        IObjectPool cubePool, spherePool, capsulePool;

        TickTimer tickTimer = new TickTimer(true);

        List<GameObject> cubeCache = new List<GameObject>();
        List<GameObject> sphereCache = new List<GameObject>();
        List<GameObject> capsuleCache = new List<GameObject>();

        async void Start()
        {
            CosmosEntry.ResourceManager.AddOrUpdateBuildInLoadHelper(Resource.ResourceLoadMode.Resource, new QuarkLoader());
            objectPoolManager = CosmosEntry.ObjectPoolManager;

            capsulePool = objectPoolManager.RegisterObjectPool(new ObjectAssetInfo(resCapsule, resCapsule));
            spherePool = objectPoolManager.RegisterObjectPool(new ObjectAssetInfo(resSphere, resSphere));
            cubePool = objectPoolManager.RegisterObjectPool(new ObjectAssetInfo(resCube, resCube));

            capsulePool.OnObjectSpawn += OnObjectSpawn;
            cubePool.OnObjectSpawn += OnObjectSpawn;
            spherePool.OnObjectSpawn += OnObjectSpawn;

            capsulePool.OnObjectDespawn += OnObjectDespawn;
            cubePool.OnObjectDespawn += OnObjectDespawn;
            spherePool.OnObjectDespawn += OnObjectDespawn;

            //注意，这里是毫秒单位；
            tickTimer.AddTask(100, OnSpawnTime, null, int.MaxValue);
            //此写法的实际意义为，生成的单位存活时间为15秒；
            await new WaitForSeconds(15f);
            tickTimer.AddTask(100, OnDespawnTime, null, int.MaxValue);
        }
        void Update()
        {
            tickTimer.TickRefresh();
        }
        /// <summary>
        /// 倒计时触发生成事件；
        /// </summary>
        void OnSpawnTime(int idx)
        {
            var cubeGo = cubePool.Spawn();
            cubeCache.Add(cubeGo);

            var sphereGo = spherePool.Spawn();
            sphereCache.Add(sphereGo);

            var capsuleGo = capsulePool.Spawn();
            capsuleCache.Add(capsuleGo);
        }
        /// <summary>
        /// 倒计时出发回收事件；
        /// </summary>
        void OnDespawnTime(int idx)
        {
            {
                var go = cubeCache.RemoveFirst();
                cubePool.Despawn(go);
            }
            {
                var go = sphereCache.RemoveFirst();
                spherePool.Despawn(go);
            }
            {
                var go = capsuleCache.RemoveFirst();
                capsulePool.Despawn(go);
            }
        }
        /// <summary>
        /// 对象被对象池生成事件；
        /// </summary>
        void OnObjectSpawn(GameObject obj)
        {
            obj.transform.SetParent(spawnRoot);
            obj.transform.ResetLocalTransform();
            obj.SetActive(true);
        }
        /// <summary>
        /// 对象被对象池回收事件；
        /// </summary>
        void  OnObjectDespawn(GameObject obj)
        {
            obj.transform.SetParent(despawnRoot);
            obj.transform.ResetLocalTransform();
            obj.SetActive(false);
        }

    }
}