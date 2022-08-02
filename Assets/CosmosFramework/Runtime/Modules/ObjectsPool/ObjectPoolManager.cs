using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;

namespace Cosmos.ObjectPool
{
    //================================================
    /*
     * 1、对象池模块，采用注册式生成池对象。
     * 
     * 2、对象池名不可重复。
     */
    //================================================
    [Module]
    internal sealed class ObjectPoolManager : Module, IObjectPoolManager
    {
        #region Properties
        Dictionary<string, ObjectPool> poolDict;
        /// <inheritdoc/>
        public int PoolCount { get { return poolDict.Count; } }
        Action<float> elapseRefreshHandler;
        event Action<float> ElapseRefreshHandler
        {
            add { elapseRefreshHandler += value; }
            remove { elapseRefreshHandler -= value; }
        }
        ObjectPoolAssetHelper objectPoolAssetHelper;
        #endregion

        #region Methods
        /// <inheritdoc/>
        public Coroutine RegisterObjectPoolAsync(ObjectPoolAssetInfo assetInfo, Action<IObjectPool> callback)
        {
            if (!HasObjectPool(assetInfo.PoolName))
            {
                return objectPoolAssetHelper.LoadObjectAssetAsync(assetInfo, (info, go) =>
                  {
                      var poolName = info.PoolName;
                      if (go != null)
                      {
                          var pool = ObjectPool.Create(go, poolName, assetInfo);
                          poolDict.TryAdd(poolName, pool);
                          ElapseRefreshHandler += pool.OnElapseRefresh;
                          callback?.Invoke(pool);
                      }
                      else
                      {
                          throw new ArgumentException($"{ info.AssetName} not exist.");
                      }
                  });
            }
            else
                throw new ArgumentException($"object pool :{assetInfo.PoolName} is exist.");
        }
        /// <inheritdoc/>
        public async Task<IObjectPool> RegisterObjectPoolAsync(ObjectPoolAssetInfo assetInfo)
        {
            IObjectPool pool = null;
            await RegisterObjectPoolAsync(assetInfo, (p) => { pool = p; });
            return pool;
        }
        /// <inheritdoc/>
        public IObjectPool RegisterObjectPool(string poolName, GameObject spawnAsset)
        {
            if (!HasObjectPool(poolName))
            {
                var pool = ObjectPool.Create(spawnAsset, poolName, default);
                poolDict.TryAdd(poolName, pool);
                ElapseRefreshHandler += pool.OnElapseRefresh;
                return pool;
            }
            else
                throw new ArgumentException($"object pool :{poolName} is exist.");
        }
        /// <inheritdoc/>
        public void DeregisterObjectPool(string poolName)
        {
            if (poolDict.Remove(poolName, out var pool))
            {
                ElapseRefreshHandler -= pool.OnElapseRefresh;
                if (!string.IsNullOrEmpty(pool.ObjectPoolAssetInfo.PoolName))
                    objectPoolAssetHelper.UnloadObjectAsset(pool.ObjectPoolAssetInfo);
                ObjectPool.Release(pool.CastTo<ObjectPool>());
            }
        }
        /// <inheritdoc/>
        public void DeregisterObjectPool(IObjectPool pool)
        {
            if (poolDict.Remove(pool.ObjectPoolName, out var srcPool))
            {
                ElapseRefreshHandler -= pool.OnElapseRefresh;
                if (!string.IsNullOrEmpty(srcPool.ObjectPoolAssetInfo.PoolName))
                    objectPoolAssetHelper.UnloadObjectAsset(srcPool.ObjectPoolAssetInfo);
                ObjectPool.Release(pool.CastTo<ObjectPool>());
            }
        }
        /// <inheritdoc/>
        public bool GetObjectPool(string poolName, out IObjectPool pool)
        {
            pool = null;
            var rst = poolDict.TryGetValue(poolName, out var srcPool);
            pool = srcPool;
            return rst;
        }
        /// <inheritdoc/>
        public bool HasObjectPool(string poolName)
        {
            return poolDict.ContainsKey(poolName);
        }
        /// <inheritdoc/>
        public void DeregisterAllObjectPool()
        {
            foreach (var pool in poolDict)
            {
                pool.Value.ClearPool();
                var assetInfo = pool.Value.ObjectPoolAssetInfo;
                if (!string.IsNullOrEmpty(assetInfo.PoolName))
                    objectPoolAssetHelper.UnloadObjectAsset(assetInfo);
            }
            poolDict.Clear();
        }
        protected override void OnInitialization()
        {
            objectPoolAssetHelper = new ObjectPoolAssetHelper();
            poolDict = new Dictionary<string, ObjectPool>();
        }
        [ElapseRefresh]
        void ElapseRefresh(float deltatime)
        {
            if (IsPause)
                return;
            elapseRefreshHandler?.Invoke(deltatime);
        }
        #endregion
    }
}