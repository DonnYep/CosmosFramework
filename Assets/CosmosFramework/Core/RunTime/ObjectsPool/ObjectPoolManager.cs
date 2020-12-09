using UnityEngine;
using System.Collections.Generic;
using System;
namespace Cosmos.ObjectPool
{
    [Module]
    internal sealed class ObjectPoolManager : Module//, IObjectPoolManager
    {
        #region Properties
        Dictionary<TypeStringPair, ObjectSpawnPool> poolDict;
        Action<long> elapseRefreshHandler;
        event Action<long> ElapseRefreshHandler
        {
            add { elapseRefreshHandler += value; }
            remove { elapseRefreshHandler -= value; }
        }
        IResourceManager resourceManager;
        #endregion

        #region Methods
        public override void OnInitialization()
        {
            poolDict = new Dictionary<TypeStringPair, ObjectSpawnPool>();
        }
        public override void OnPreparatory()
        {
            resourceManager = GameManager.GetModule<IResourceManager>();
        }
        public override void OnElapseRefresh(long msNow)
        {
            elapseRefreshHandler?.Invoke(msNow);
        }

        public Coroutine RegisterObjectAssetAsync(ObjectAssetInfo objectAssetInfo,Action<IObjectSpawnPool>onRegisterCallback=null)
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
                        var pool = new ObjectSpawnPool(spawnItem);
                        poolDict.TryAdd(objectKey, new ObjectSpawnPool(spawnItem));
                        onRegisterCallback?.Invoke(pool);
                    }
                    else
                        throw new ArgumentException($"object key :{objectKey} is exist.");
                });
        }
        public IObjectSpawnPool RegisterObjectAsset(ObjectAssetInfo objectAssetInfo)
        {
            if (objectAssetInfo == null)
            {
                throw new ArgumentNullException("objectAssetInfo is  invalid.");
            }
            var objectKey = objectAssetInfo.ObjectKey;
            if (!HasObjectPool(objectKey))
            {
                var spawnItem = resourceManager.LoadPrefab(objectAssetInfo);
                var pool = new ObjectSpawnPool(spawnItem);
                poolDict.TryAdd(objectKey, new ObjectSpawnPool(spawnItem));
                return pool;
            }
            else
                throw new ArgumentException($"object key :{objectKey} is exist.");
        }
        public IObjectSpawnPool RegisterObjectAsset(TypeStringPair objectKey, object spawnItem)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            if (!HasObjectPool(objectKey))
            {
                var pool = new ObjectSpawnPool(spawnItem);
                poolDict.TryAdd(objectKey, new ObjectSpawnPool(spawnItem));
                return pool;
            }
            else
                throw new ArgumentException($"object key :{objectKey} is exist.");
        }
        public IObjectSpawnPool RegisterObjectAsset(Type objectType, string name, object spawnItem)
        {
            var objectKey = new TypeStringPair(objectType, name);
            return RegisterObjectAsset(objectKey, spawnItem);
        }
        public IObjectSpawnPool RegisterObjectAsset(Type objectType, object spawnItem)
        {
            var objectKey = new TypeStringPair(objectType);
            return RegisterObjectAsset(objectKey, spawnItem);
        }
        public IObjectSpawnPool RegisterObjectAsset<T>(string name, object spawnItem) where T : class
        {
            return RegisterObjectAsset(typeof(T), name, spawnItem);
        }
        public IObjectSpawnPool RegisterObjectAsset<T>(object spawnItem) where T : class
        {
            return RegisterObjectAsset(typeof(T), spawnItem);
        }
        public IObjectSpawnPool RegisterObjectAsset(string name, object spawnItem)
        {
            var objectKey = new TypeStringPair(typeof(object), name);
            if (!HasObjectPool(objectKey))
            {
                var pool = new ObjectSpawnPool(spawnItem);
                poolDict.TryAdd(objectKey, new ObjectSpawnPool(spawnItem));
                return pool;
            }
            else
                throw new ArgumentException($"object key :{objectKey} is exist.");
        }


        public void DeregisterObjectAsset(TypeStringPair objectKey)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            if (poolDict.Remove(objectKey, out var pool))
            {
                ElapseRefreshHandler -= pool.OnElapseRefresh;
                pool.Clear();
            }
        }
        public void DeregisterObjectAsset(Type objectType, string name)
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
            DeregisterObjectAsset(objectKey);
        }
        public void DeregisterObjectAsset(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            DeregisterObjectAsset(objectKey);
        }
        public void DeregisterObjectAsset<T>(string name) where T : class
        {
            DeregisterObjectAsset(typeof(T), name);
        }
        public void DeregisterObjectAsset<T>() where T : class
        {
            DeregisterObjectAsset(typeof(T));
        }
        public void DeregisterObjectAsset(string name)
        {
            DeregisterObjectAsset(typeof(object), name);
        }

        public object SpawnObject(TypeStringPair objectKey)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            var hasPool = poolDict.TryGetValue(objectKey, out var pool);
            if (hasPool)
            {
                return pool.Spawn();
            }
            else
            {
                throw new ArgumentNullException($"object key :{objectKey} has not been register");
            }
        }
        public object SpawnObject(Type objectType, string name)
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
            return SpawnObject(objectKey);
        }
        public object SpawnObject(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            return SpawnObject(objectKey);
        }
        public object SpawnObject<T>(string name) where T : class
        {
            return SpawnObject(typeof(T), name);
        }
        public object SpawnObject<T>() where T : class
        {
            return SpawnObject(typeof(T));
        }
        public object SpawnObject(string name)
        {
            return SpawnObject(typeof(object), name);
        }

        public object[] SpawnObjects(TypeStringPair objectKey, int count)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            if (count <= 0)
            {
                throw new ArgumentException($"count :{count} is  invalid.");
            }
            var hasPool = poolDict.TryGetValue(objectKey, out var pool);
            if (hasPool)
            {
                return pool.Spawns(count);
            }
            else
            {
                throw new ArgumentNullException($"object key :{objectKey} has not been register");
            }
        }
        public object[] SpawnObjects(Type objectType, string name, int count)
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
            return SpawnObjects(objectKey, count);
        }
        public object[] SpawnObjects(Type objectType, int count)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            return SpawnObjects(objectKey, count);
        }
        public object[] SpawnObjects<T>(string name, int count) where T : class
        {
            return SpawnObjects(typeof(T), name, count);
        }
        public object[] SpawnObjects<T>(int count) where T : class
        {
            return SpawnObjects(typeof(T), count);
        }
        public object[] SpawnObjects(string name, int count)
        {
            return SpawnObjects(typeof(object), name, count);
        }

        public void DespawnObject(TypeStringPair objectKey, object target)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            if (poolDict.TryGetValue(objectKey, out var pool))
            {
                pool?.Despawn(target);
            }
            else
            {
                throw new ArgumentNullException($"object key :{objectKey} has not been register");
            }
        }
        public void DespawnObject(Type objectType, string name, object target)
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
            DespawnObject(objectKey, target);
        }
        public void DespawnObject(Type objectType, object target)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            DespawnObject(objectKey, target);

        }
        public void DespawnObject<T>(string name, object target) where T : class
        {
            DespawnObject(typeof(T), name, target);
        }
        public void DespawnObject<T>(object target) where T : class
        {
            DespawnObject(typeof(T), target);
        }
        public void DespawnObject(string name, object target)
        {
            DespawnObject(typeof(object), name, target);
        }

        public void DespawnObjects(TypeStringPair objectKey, object[] targets)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            if (poolDict.TryGetValue(objectKey, out var pool))
            {
                pool?.Despawns(targets);
            }
            else
            {
                throw new ArgumentNullException($"object key :{objectKey} has not been register");
            }
        }
        public void DespawnObjects(Type objectType, string name, object[] targets)
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
            DespawnObjects(objectKey, targets);
        }
        public void DespawnObjects(Type objectType, object[] targets)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            DespawnObjects(objectKey, targets);

        }
        public void DespawnObjects<T>(string name, object[] targets) where T : class
        {
            DespawnObjects(typeof(T), name, targets);
        }
        public void DespawnObjects<T>(object[] targets) where T : class
        {
            DespawnObjects(typeof(T), targets);
        }
        public void DespawnObjects(string name, object[] targets)
        {
            DespawnObjects(typeof(object), name, targets);
        }

        public int GetPoolCount(TypeStringPair objectKey)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            var hasPool = poolDict.TryGetValue(objectKey, out var pool);
            if (hasPool)
            {
                return pool.ObjectCount;
            }
            else
            {
                throw new ArgumentNullException($"object key :{objectKey} has not been register");
            }
        }
        public int GetPoolCount(Type objectType, string name)
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
            return GetPoolCount(objectKey);
        }
        public int GetPoolCount(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            return GetPoolCount(objectKey);
        }
        public int GetPoolCount<T>(string name) where T : class
        {
            return GetPoolCount(typeof(T), name);
        }
        public int GetPoolCount<T>() where T : class
        {
            return GetPoolCount(typeof(T));
        }
        public int GetPoolCount(string name)
        {
            return GetPoolCount(typeof(object), name);
        }

        public IObjectSpawnPool GetObjectSpawnPool(TypeStringPair objectKey)
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
        public IObjectSpawnPool GetObjectSpawnPool(Type objectType, string name)
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
            return GetObjectSpawnPool(objectKey);
        }
        public IObjectSpawnPool GetObjectSpawnPool(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType is  invalid.");
            }
            var objectKey = new TypeStringPair(objectType);
            return GetObjectSpawnPool(objectKey);
        }
        public IObjectSpawnPool GetObjectSpawnPool<T>(string name) where T : class
        {
            return GetObjectSpawnPool(typeof(T), name);
        }
        public IObjectSpawnPool GetObjectSpawnPool<T>() where T : class
        {
            return GetObjectSpawnPool(typeof(T));
        }
        public IObjectSpawnPool GetObjectSpawnPool(string name)
        {
            return GetObjectSpawnPool(typeof(object), name);
        }

        public bool HasObjectPool(TypeStringPair objectKey)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            return poolDict.ContainsKey(objectKey);
        }
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
        public bool HasObjectPool(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("type is is invalid");
            }

            var objectKey = new TypeStringPair(objectType);
            return poolDict.ContainsKey(objectKey);
        }
        public bool HasObjectPool<T>(string name) where T : class
        {

            return HasObjectPool(typeof(T), name);
        }
        public bool HasObjectPool<T>() where T : class
        {
            return HasObjectPool(typeof(T));
        }
        public bool HasObjectPool(string name)
        {
            return HasObjectPool(typeof(object), name);
        }

        public void Clear(TypeStringPair objectKey)
        {
            if (objectKey == null)
            {
                throw new ArgumentNullException("objectKey is  invalid.");
            }
            if (poolDict.TryGetValue(objectKey, out var pool))
            {
                pool?.Clear();
            }
            else
            {
                throw new ArgumentNullException($"object key :{objectKey} has not been register");
            }
        }
        /// <summary>
        /// 清空指定对象池
        /// </summary>
        /// <param name="key"></param>
        public void Clear(Type objectType, string name)
        {
            var objectKey = new TypeStringPair(objectType, name);
            Clear(objectKey);
        }
        public void Clear(Type objectType)
        {
            var objectKey = new TypeStringPair(objectType);
            Clear(objectKey);
        }
        public void Clear<T>(string name) where T : class
        {
            Clear(typeof(T), name);
        }
        public void Clear<T>() where T : class
        {
            Clear(typeof(T));
        }
        public void Clear(string name)
        {
            Clear(typeof(object), name);
        }

        /// <summary>
        /// 清除所有池对象
        /// </summary>
        public void ClearAll()
        {
            foreach (var pool in poolDict)
            {
                pool.Value.Clear();
            }
            poolDict.Clear();
        }

        #endregion
    }
}