using System.Collections.Generic;
using System;
namespace Cosmos.ObjectPool
{
    //================================================
    /*
     * 1、对象池；
     */
    //================================================
    [Module]
    internal sealed class ObjectPoolManager : Module, IObjectPoolManager
    {
        //TODO ObjectPool对async/await语法支持
        #region Properties
        Dictionary<ObjectPoolKey, IObjectPool> poolDict;

        Action<float> elapseRefreshHandler;
        event Action<float> ElapseRefreshHandler
        {
            add { elapseRefreshHandler += value; }
            remove { elapseRefreshHandler -= value; }
        }
        IResourceManager resourceManager;
        #endregion

        #region Methods
        /// <summary>
        /// 注册对象池（异步）;
        /// </summary>
        /// <param name="objectAssetInfo">对象资源信息</param>
        /// <param name="onRegisterCallback">注册成功后的回调，若失败则不回调</param>
        /// <returns>协程对象</returns>
        public async void RegisterObjectPoolAsync(ObjectAssetInfo objectAssetInfo, Action<IObjectPool> onRegisterCallback = null)
        {
            if (objectAssetInfo == null)
                throw new ArgumentNullException("objectAssetInfo is  invalid.");
            await resourceManager.LoadPrefabAsync(objectAssetInfo,
                 (spawnItem) =>
                 {
                     var objectKey = objectAssetInfo.ObjectKey;
                     if (!HasObjectPool(objectKey))
                     {
                         var pool = ObjectPool.Create(spawnItem, objectKey);
                         poolDict.TryAdd(objectKey, pool);
                         ElapseRefreshHandler += pool.OnElapseRefresh;
                         onRegisterCallback?.Invoke(pool);
                     }
                     else
                         throw new ArgumentException($"object key :{objectKey} is exist.");
                 });
        }
        /// <summary>
        /// 注册对象池；
        /// </summary>
        /// <param name="objectAssetInfo">对象资源信息</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool(ObjectAssetInfo objectAssetInfo)
        {
            if (objectAssetInfo == null)
                throw new ArgumentNullException("objectAssetInfo is  invalid.");
            var objectKey = objectAssetInfo.ObjectKey;
            if (!HasObjectPool(objectKey))
            {
                var spawnItem = resourceManager.LoadPrefab(objectAssetInfo);
                var pool = ObjectPool.Create(spawnItem, objectKey);
                poolDict.TryAdd(objectKey, pool);
                ElapseRefreshHandler += pool.OnElapseRefresh;
                return pool;
            }
            else
                throw new ArgumentException($"object key :{objectKey} is exist.");
        }
        /// <summary>
        /// 注册对象池；
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        /// <param name="spawnAsset">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool(ObjectPoolKey objectKey, object spawnAsset)
        {
            if (!HasObjectPool(objectKey))
            {
                var pool = ObjectPool.Create(spawnAsset, objectKey);
                poolDict.TryAdd(objectKey, pool);
                ElapseRefreshHandler += pool.OnElapseRefresh;
                return pool;
            }
            else
                throw new ArgumentException($"object key :{objectKey} is exist.");
        }
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <param name="spawnAsset">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool(string name, object spawnAsset)
        {
            return RegisterObjectPool(new ObjectPoolKey(typeof(object), name), spawnAsset);
        }

        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        public void DeregisterObjectPool(ObjectPoolKey objectKey)
        {
            if (poolDict.Remove(objectKey, out var pool))
            {
                ElapseRefreshHandler -= pool.OnElapseRefresh;
                ObjectPool.Release(pool.CastTo<ObjectPool>());
            }
        }
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="name">对象的名称</param>
        public void DeregisterObjectPool(string name)
        {
            DeregisterObjectPool(new ObjectPoolKey(typeof(object), name));
        }

        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        /// <returns>对象池对象的接口</returns>
        public IObjectPool GetObjectPool(ObjectPoolKey objectKey)
        {
            var hasPool = poolDict.TryGetValue(objectKey, out var pool);
            if (hasPool)
            {
                return pool;
            }
            else
            {
                throw new ArgumentNullException($"object key :{objectKey} has not been register");
            }
        }
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <returns>对象池对象的接口</returns>
        public IObjectPool GetObjectPool(string name)
        {
            return GetObjectPool(new ObjectPoolKey(typeof(object), name));
        }

        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        /// <returns>是否存在</returns>
        public bool HasObjectPool(ObjectPoolKey objectKey)
        {
            return poolDict.ContainsKey(objectKey);
        }
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <returns>是否存在</returns>
        public bool HasObjectPool(string name)
        {
            return HasObjectPool(new ObjectPoolKey(typeof(object), name));
        }

        /// <summary>
        /// 注销所有池对象
        /// </summary>
        public void DeregisterAllObjectPool()
        {
            foreach (var pool in poolDict)
            {
                pool.Value.ClearPool();
            }
            poolDict.Clear();
        }
        protected override void OnPreparatory()
        {
            poolDict = new Dictionary<ObjectPoolKey, IObjectPool>();
            resourceManager = GameManager.GetModule<IResourceManager>();
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