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
        /// <summary>
        /// 对象池的数量；
        /// </summary>
        public int PoolCount { get { return poolDict.Count; } }
        Action<float> elapseRefreshHandler;
        event Action<float> ElapseRefreshHandler
        {
            add { elapseRefreshHandler += value; }
            remove { elapseRefreshHandler -= value; }
        }
        IObjectPoolHelper objectPoolHelper;
        #endregion

        #region Methods
        /// <summary>
        /// 设置对象池帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        public void SetHelper(IObjectPoolHelper helper)
        {
            if (helper == null)
                throw new ArgumentNullException("helper is invalid");
            objectPoolHelper = helper;
        }
        /// <summary>
        /// 异步注册对象池；
        /// </summary>
        /// <param name="assetInfo">对象池资源信息</param>
        /// <param name="callback">注册回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RegisterObjectPoolAsync(ObjectPoolAssetInfo assetInfo, Action<IObjectPool> callback)
        {
            if (assetInfo == null)
                throw new ArgumentNullException("objectAssetInfo is  invalid.");
            if (!HasObjectPool(assetInfo.PoolName))
            {
                return objectPoolHelper.LoadObjectAssetAsync(assetInfo, (info, go) =>
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
                          throw new ArgumentException($"{ info.AssetPath} not exist.");
                      }
                  });
            }
            else
                throw new ArgumentException($"object pool :{assetInfo.PoolName} is exist.");
        }
        /// <summary>
        /// 异步注册对象池；
        /// 须使用await获取结果；
        /// </summary>
        /// <param name="assetInfo">对象池资源信息</param>
        /// <returns>Task异步任务</returns>
        public async Task<IObjectPool> RegisterObjectPoolAsync(ObjectPoolAssetInfo assetInfo)
        {
            IObjectPool pool = null;
            await RegisterObjectPoolAsync(assetInfo, (p) => { pool = p; });
            return pool;
        }
        /// <summary>
        /// 注册自定义资源对象池；
        /// </summary>
        /// <param name="poolName">对象池名<</param>
        /// <param name="spawnAsset">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool(string poolName, GameObject spawnAsset)
        {
            if (!HasObjectPool(poolName))
            {
                var pool = ObjectPool.Create(spawnAsset, poolName, null);
                poolDict.TryAdd(poolName, pool);
                ElapseRefreshHandler += pool.OnElapseRefresh;
                return pool;
            }
            else
                throw new ArgumentException($"object pool :{poolName} is exist.");
        }
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="poolName">对象池名<</param>
        public void DeregisterObjectPool(string poolName)
        {
            if (poolDict.Remove(poolName, out var pool))
            {
                ElapseRefreshHandler -= pool.OnElapseRefresh;
                if (pool.ObjectPoolAssetInfo != null)
                    objectPoolHelper.UnloadObjectAsset(pool.ObjectPoolAssetInfo);
                ObjectPool.Release(pool.CastTo<ObjectPool>());
            }
        }
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="pool">对象池</param>
        public void DeregisterObjectPool(IObjectPool pool)
        {
            if (poolDict.Remove(pool.ObjectPoolName, out var srcPool))
            {
                ElapseRefreshHandler -= pool.OnElapseRefresh;
                if (srcPool.ObjectPoolAssetInfo != null)
                    objectPoolHelper.UnloadObjectAsset(srcPool.ObjectPoolAssetInfo);
                ObjectPool.Release(pool.CastTo<ObjectPool>());
            }
        }
        /// <summary>
        /// 获取对象池；
        /// </summary>
        /// <param name="poolName">对象池名</param>
        /// <param name="pool">对象池</param>
        /// <returns>获取结果</returns>
        public bool GetObjectPool(string poolName, out IObjectPool pool)
        {
            pool = null;
            var rst = poolDict.TryGetValue(poolName, out var srcPool);
            pool = srcPool;
            return rst;
        }
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="poolName">对象池名</param>
        /// <returns>是否存在</returns>
        public bool HasObjectPool(string poolName)
        {
            return poolDict.ContainsKey(poolName);
        }
        /// <summary>
        /// 注销所有池对象
        /// </summary>
        public void DeregisterAllObjectPool()
        {
            foreach (var pool in poolDict)
            {
                pool.Value.ClearPool();
                var assetInfo = pool.Value.ObjectPoolAssetInfo;
                if (assetInfo != null)
                    objectPoolHelper.UnloadObjectAsset(assetInfo);
            }
            poolDict.Clear();
        }
        protected override void OnInitialization()
        {
            SetHelper(new DefaultObjectPoolHelper());
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