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

        Action<ObjectPoolRegisterSuccessEventArgs> objectPoolRegisterSuccess;
        Action<ObjectPoolRegisterFailureEventArgs> objectPoolRegisterFailure;

        /// <inheritdoc/>
        public event Action<ObjectPoolRegisterSuccessEventArgs> ObjectPoolRegisterSuccess
        {
            add { objectPoolRegisterSuccess += value; }
            remove { objectPoolRegisterSuccess -= value; }
        }
        /// <inheritdoc/>
        public event Action<ObjectPoolRegisterFailureEventArgs> ObjectPoolRegisterFailure
        {
            add { objectPoolRegisterFailure += value; }
            remove { objectPoolRegisterFailure -= value; }
        }
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
                          OnObjectPoolRegisterSuccess(assetInfo.PoolName, pool);
                      }
                      else
                      {
                          OnObjectPoolRegisterFailure(assetInfo.PoolName,assetInfo.AssetName);
                      }
                  });
            }
            else
            {
                var hasPool = poolDict.TryGetValue(assetInfo.PoolName, out var pool);
                callback?.Invoke(pool);
                return null;
            }
        }
        /// <inheritdoc/>
        public IObjectPool RegisterObjectPool(string poolName, GameObject spawnAsset)
        {
            if (!HasObjectPool(poolName))
            {
                var pool = ObjectPool.Create(spawnAsset, poolName, default);
                poolDict.TryAdd(poolName, pool);
                ElapseRefreshHandler += pool.OnElapseRefresh;
                OnObjectPoolRegisterSuccess(poolName, pool);
                return pool;
            }
            else
            {
                poolDict.TryGetValue(poolName, out var pool);
                return pool;
            }
        }
        /// <inheritdoc/>
        public void DeregisterObjectPool(string poolName)
        {
            if (poolDict.Remove(poolName, out var pool))
            {
                ElapseRefreshHandler -= pool.OnElapseRefresh;
                if (!string.IsNullOrEmpty(pool.ObjectPoolAssetInfo.PoolName))
                    objectPoolAssetHelper.UnloadObjectAsset(pool.ObjectPoolAssetInfo);
                ObjectPool.Release(pool);
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
        void OnObjectPoolRegisterSuccess(string poolName, IObjectPool objectPool)
        {
            var eventArgs = ObjectPoolRegisterSuccessEventArgs.Create(poolName, objectPool);
            objectPoolRegisterSuccess?.Invoke(eventArgs);
            ObjectPoolRegisterSuccessEventArgs.Release(eventArgs);
        }
        void OnObjectPoolRegisterFailure( string poolName,string assetName)
        {
            var eventArgs = ObjectPoolRegisterFailureEventArgs.Create(poolName, $"{ assetName} not exist.");
            objectPoolRegisterFailure?.Invoke(eventArgs);
            ObjectPoolRegisterFailureEventArgs.Release(eventArgs);
        }
        #endregion
    }
}