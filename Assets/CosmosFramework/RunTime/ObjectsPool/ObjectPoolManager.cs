using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.ObjectPool
{
    internal sealed partial class ObjectPoolManager : Module<ObjectPoolManager>
    {
        #region Properties
        Dictionary<TypeNamePair, ObjectSpawnPool> objectPoolDict;
        List<ObjectSpawnPool> objectSpawnPoolCache;
        public const int OBJECT_POOL_CAPACITY=100;
        #endregion
        public override void OnInitialization()
        {
            objectPoolDict = new Dictionary<TypeNamePair, ObjectSpawnPool>();
            objectSpawnPoolCache = new List<ObjectSpawnPool>();
        }
        #region Internal Methods
        internal bool DeregisterSpawnPool<T>(ObjectKey<T> objectKey)
            where T : IObject
        {
            return DeregisterSpawnPool(objectKey.GetValue());
        }
        internal void RegisterSpawnPool<T>(ObjectKey<T> objectKey, ObjectPoolVariable poolData)
            where T : IObject
        {
            RegisterSpawnPool(objectKey.GetValue(), poolData);
        }
        internal bool HasObjectPool<T>(ObjectKey<T> objectKey)
     where T : IObject
        {
            return objectPoolDict.ContainsKey(objectKey.GetValue());
        }
        internal T Spawn<T>(ObjectKey<T> objectKey)
            where T : class,IObject 
        {
            T obj = default;
            var key = objectKey.GetValue();
            if (objectPoolDict.ContainsKey(key))
            {
                obj = objectPoolDict[key].Spawn() as T;
            }
            else
                throw new ArgumentNullException("ObjectPoolManager.Spawn : " + key.ToString() + "key not exist");
            return obj;
        }
        internal void Despawn<T>(ObjectKey<T> objectKey, IObject obj)
            where T : IObject
        {
            var key = objectKey.GetValue();
            if (objectPoolDict.ContainsKey(key))
            {
                objectPoolDict[key].Despawn(obj);
            }
            else
                throw new ArgumentNullException("ObjectPoolManager.Spawn : " + key.ToString() + "key not exist");
        }
        internal void Despawns<T>(ObjectKey<T> objectKey, IObject[] objs)
    where T : IObject
        {
            var key = objectKey.GetValue();
            if (objectPoolDict.ContainsKey(key))
            {
                objectPoolDict[key].Despawns(objs);
            }
            else
                throw new ArgumentNullException("ObjectPoolManager.Spawn : " + key.ToString() + "key not exist");
        }
        internal void ClearAllPool()
        {
            objectPoolDict.Clear();
            objectSpawnPoolCache.Clear();
        }
        internal void ClearPool<T>(ObjectKey<T> objectKey)
            where T : IObject
        {
            var key = objectKey.GetValue();
            if (objectPoolDict.ContainsKey(key))
            {
                objectPoolDict[key].Clear();
            }
            else
                throw new ArgumentNullException("ObjectPoolManager.Spawn : " + key.ToString() + "key not exist");
        }
        internal int GetPoolObjectCount<T>(ObjectKey<T> objectKey)
            where T : IObject
        {
            var key = objectKey.GetValue();
            if (objectPoolDict.ContainsKey(key))
            {
                return objectPoolDict[key].ObjectCount;
            }
            else
                throw new ArgumentNullException("ObjectPoolManager.Spawn : " + key.ToString() + "key not exist");
        }
        internal int GetObjectPoolCount()
        {
            return objectPoolDict.Count;
        }
        #endregion
        #region Private Methoda
        bool HasObjectPool(TypeNamePair typeNamePair)
        {
            return objectPoolDict.ContainsKey(typeNamePair);
        }
        bool DeregisterSpawnPool(TypeNamePair typeNamePair)
        {
            ObjectSpawnPool objectPool = default;
            if (objectPoolDict.TryGetValue(typeNamePair, out objectPool))
            {
                objectPool.Clear();
                return objectPoolDict.Remove(typeNamePair);
            }
            return false;
        }
        void RegisterSpawnPool(TypeNamePair typeNamePair, ObjectPoolVariable poolData)
        {
            if (!objectPoolDict.ContainsKey(typeNamePair))
                objectPoolDict.Add(typeNamePair, new ObjectSpawnPool(poolData));
            else
                throw new ArgumentException("ObjectPoolManager.RegisterSpawnPool : " + typeNamePair.ToString() + "key already exist");
        }
        #endregion
    }
}
