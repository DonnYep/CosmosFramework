using UnityEngine;
using System.Collections.Generic;
using System;
namespace Cosmos.ObjectPool
{
    [Module]
    internal sealed class ObjectPoolManager : Module , IObjectPoolManager
    {
        #region Properties
        Dictionary<TypeStringPair, IObjectPool> poolDict;
        Action<float> elapseRefreshHandler;
        event Action<float> ElapseRefreshHandler
        {
            add { elapseRefreshHandler += value; }
            remove { elapseRefreshHandler -= value; }
        }
        IResourceManager resourceManager;
        #endregion

        #region Methods
        public override void OnInitialization()
        {
            poolDict = new Dictionary<TypeStringPair, IObjectPool>();
        }
        public override void OnPreparatory()
        {
            resourceManager = GameManager.GetModule<IResourceManager>();
        }
        [ElapseRefresh]
        void ElapseRefresh(float deltatime)
        {
            elapseRefreshHandler?.Invoke(deltatime);
        }

        /// <summary>
        /// 注册对象池（异步）;
        /// </summary>
        /// <param name="objectAssetInfo">对象资源信息</param>
        /// <param name="onRegisterCallback">注册成功后的回调，若失败则不回调</param>
        /// <returns>协程对象</returns>
        public Coroutine RegisterObjectPoolAsync(ObjectAssetInfo objectAssetInfo, Action<IObjectPool> onRegisterCallback = null)
        {
            if (objectAssetInfo == null)
            {
                throw new ArgumentNullException("objectAssetInfo is  invalid.");
            }
            return resourceManager.LoadPrefabAsync(objectAssetInfo,
                (spawnItem) =>
                {
                    var objectKey = objectAssetInfo.ObjectKey;
                    if (!HasObjectPool(objectKey))
                    {
                        var pool = new ObjectPool(spawnItem, objectKey);
                        poolDict.TryAdd(objectKey, pool);
                        ElapseRefreshHandler += pool.OnElapseRefresh;
                        onRegisterCallback?.Invoke(pool);
                    }
                    else
                        throw new ArgumentException($"object key :{objectKey} is exist.");
                });
        }
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="objectAssetInfo">对象资源信息</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool(ObjectAssetInfo objectAssetInfo)
        {
            if (objectAssetInfo == null)
            {
                throw new ArgumentNullException("objectAssetInfo is  invalid.");
            }
            var objectKey = objectAssetInfo.ObjectKey;
            if (!HasObjectPool(objectKey))
            {
                var spawnItem = resourceManager.LoadPrefab(objectAssetInfo);
                var pool = new ObjectPool(spawnItem, objectKey);
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
        /// <param name="objectKey">对象池key</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool(TypeStringPair objectKey, GameObject spawnItem)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            if (!HasObjectPool(objectKey))
            {
                var pool = new ObjectPool(spawnItem, objectKey);
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
        /// <param name="objectType">对象的类型</param>
        /// <param name="name">对象的名称</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool(Type objectType, string name, GameObject spawnItem)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType, name);
            return RegisterObjectPool(objectKey, spawnItem);
        }
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool(Type objectType, GameObject spawnItem)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            return RegisterObjectPool(objectKey, spawnItem);
        }
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="name">对象的名称</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool<T>(string name, GameObject spawnItem) where T : class
        {
            return RegisterObjectPool(typeof(T), name, spawnItem);
        }
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool<T>(GameObject spawnItem) where T : class
        {
            return RegisterObjectPool(typeof(T), spawnItem);
        }
        /// <summary>
        /// 注册对象池（同步）;
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <param name="spawnItem">需要生成的对象</param>
        /// <returns>注册生成后的池对象接口</returns>
        public IObjectPool RegisterObjectPool(string name, GameObject spawnItem)
        {
            return RegisterObjectPool(typeof(GameObject), name, spawnItem);
        }

        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        public void DeregisterObjectPool(TypeStringPair objectKey)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            if (poolDict.Remove(objectKey, out var pool))
            {
                ElapseRefreshHandler -= pool.OnElapseRefresh;
                pool.ClearPool();
            }
        }
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <param name="name">对象的名称</param>
        public void DeregisterObjectPool(Type objectType, string name)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType, name);
            DeregisterObjectPool(objectKey);
        }
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        public void DeregisterObjectPool(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            DeregisterObjectPool(objectKey);
        }
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="name">对象的名称</param>
        public void DeregisterObjectPool<T>(string name) where T : class
        {
            DeregisterObjectPool(typeof(T), name);
        }
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        public void DeregisterObjectPool<T>() where T : class
        {
            DeregisterObjectPool(typeof(T));
        }
        /// <summary>
        /// 注销对象池;
        /// </summary>
        /// <param name="name">对象的名称</param>
        public void DeregisterObjectPool(string name)
        {
            DeregisterObjectPool(typeof(GameObject), name);
        }

        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        /// <returns>对象池对象的接口</returns>
        public IObjectPool GetObjectPool(TypeStringPair objectKey)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
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
        /// <param name="objectType">对象的类型</param>
        /// <param name="name">对象的名称</param>
        /// <returns>对象池对象的接口</returns>
        public IObjectPool GetObjectPool(Type objectType, string name)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType, name);
            return GetObjectPool(objectKey);
        }
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <returns>对象池对象的接口</returns>
        public IObjectPool GetObjectPool(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            return GetObjectPool(objectKey);
        }
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="name">对象的名称</param>
        /// <returns>对象池对象的接口</returns>
        public IObjectPool GetObjectPool<T>(string name) where T : class
        {
            return GetObjectPool(typeof(T), name);
        }
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <returns>对象池对象的接口</returns>
        public IObjectPool GetObjectPool<T>() where T : class
        {
            return GetObjectPool(typeof(T));
        }
        /// <summary>
        /// 获得对象池;
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <returns>对象池对象的接口</returns>
        public IObjectPool GetObjectPool(string name)
        {
            return GetObjectPool(typeof(object), name);
        }

        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="objectKey">对象池key</param>
        /// <returns>是否存在</returns>
        public bool HasObjectPool(TypeStringPair objectKey)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            return poolDict.ContainsKey(objectKey);
        }
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <param name="name">对象的名称</param>
        /// <returns>是否存在</returns>
        public bool HasObjectPool(Type objectType, string name)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("type is is invalid");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType, name);
            return poolDict.ContainsKey(objectKey);
        }
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="objectType">对象的类型</param>
        /// <returns>是否存在</returns>
        public bool HasObjectPool(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("type is is invalid");
            }

            var objectKey = new TypeStringPair(objectType);
            return poolDict.ContainsKey(objectKey);
        }
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="name">对象的名称</param>
        /// <returns>是否存在</returns>
        public bool HasObjectPool<T>(string name) where T : class
        {
            return HasObjectPool(typeof(T), name);
        }
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <returns>是否存在</returns>
        public bool HasObjectPool<T>() where T : class
        {
            return HasObjectPool(typeof(T));
        }
        /// <summary>
        /// 是否存在对象池；
        /// </summary>
        /// <param name="name">对象的名称</param>
        /// <returns>是否存在</returns>
        public bool HasObjectPool(string name)
        {
            return HasObjectPool(typeof(GameObject), name);
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

        #endregion
    }
}