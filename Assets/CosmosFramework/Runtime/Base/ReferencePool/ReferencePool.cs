using System.Collections.Generic;
using System;
namespace Cosmos
{
    public static class ReferencePool
    {
        sealed class ReferPool<T>
            where T : class
        {
            public int PoolAcquireCount { get { return poolAcquireCount; } }
            public int AcquireCount { get { return acquireCount; } }
            public int ReleaseCount { get { return releaseCount; } }
            public int Count { get { return pool.Count; } }
            public Type ReferenceType { get { return referenceType; } }
            readonly Pool<T> pool;
            readonly Type referenceType;
            int poolAcquireCount = 0;
            int acquireCount = 0;
            int releaseCount = 0;
            public ReferPool(Type type, Func<T> objectGenerator, Action<T> onRelease)
            {
                referenceType = type;
                pool = new Pool<T>(objectGenerator, onRelease);
            }
            public T Acquire()
            {
                poolAcquireCount++;
                acquireCount++;
                return pool.Spawn();
            }
            public void Release(T obj)
            {
                acquireCount--;
                releaseCount++;
                pool.Despawn(obj);
            }
            public void Clear()
            {
                pool.Clear();
            }
        }
        public static int Count { get { return referencePoolDict.Count; } }
        static readonly Dictionary<Type, ReferPool<IReference>> referencePoolDict
            = new Dictionary<Type, ReferPool<IReference>>();
        /// <summary>
        /// 清除所有池；
        /// </summary>
        public static void ClearAll()
        {
            foreach (var pool in referencePoolDict.Values)
            {
                pool.Clear();
            }
            referencePoolDict.Clear();
        }
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferencePool(typeof(T)).Acquire() as T;
        }
        public static IReference Acquire(Type type)
        {
            CheckAcquireType(type);
            return GetReferencePool(type).Acquire();
        }
        public static void Release(IReference reference)
        {
            if (reference == null)
                return;
            var type = reference.GetType();
            GetReferencePool(type).Release(reference);
        }
        public static void Release<T>(IEnumerable<T> references) where T : class, IReference, new()
        {
            if (references == null)
                return;
            var type = typeof(T);
            var pool = GetReferencePool(type);
            foreach (var reference in references)
            {
                pool.Release(reference);
            }
        }
        public static void Release(params IReference[] references)
        {
            if (references == null)
                throw new ArgumentNullException("Reference array is invalid.");
            var length = references.Length;
            for (int i = 0; i < length; i++)
            {
                var type = references.GetType();
                var pool = GetReferencePool(type);
                pool.Release(references[i]);
            }
        }
        /// <summary>
        /// 移除引用池；
        /// </summary>
        /// <param name="type">目标类型</param>
        public static void RemovePool(Type type)
        {
            CheckAcquireType(type);
            referencePoolDict.Remove(type, out var pool);
            pool?.Clear();
        }
        /// <summary>
        /// 移除引用池；
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public static void RemovePool<T>() where T : class, IReference, new()
        {
            referencePoolDict.Remove(typeof(T), out var pool);
            pool?.Clear();
        }

        public static ReferencePoolInfo GetReferencePoolInfo<T>()
             where T : class, IReference, new()
        {
            var type = typeof(T);
            var pool = GetReferencePool(type);
            return new ReferencePoolInfo(type, pool.AcquireCount, pool.ReleaseCount, pool.PoolAcquireCount);
        }
        public static ReferencePoolInfo GetReferencePoolInfo(Type type)
        {
            CheckAcquireType(type);
            var pool = GetReferencePool(type);
            return new ReferencePoolInfo(type, pool.AcquireCount, pool.ReleaseCount, pool.PoolAcquireCount);
        }
        public static ReferencePoolInfo[] GetAllReferencePoolInfos()
        {
            int index = 0;
            ReferencePoolInfo[] pools = new ReferencePoolInfo[Count];
            foreach (var pool in referencePoolDict.Values)
            {
                pools[index++] = new ReferencePoolInfo(pool.ReferenceType, pool.AcquireCount, pool.ReleaseCount, pool.PoolAcquireCount);
            }
            return pools;
        }

        static void CheckAcquireType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Reference type is invalid !");
            if (!type.IsClass || type.IsAbstract)
                throw new ArgumentException("Reference type is not a non-abstract class type !");
            if (!typeof(IReference).IsAssignableFrom(type))
                throw new ArgumentException("Reference type is not inherit from IReference!");
        }
        static ReferPool<IReference> GetReferencePool(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Reference type is invalid !");
            if (!referencePoolDict.TryGetValue(type, out var pool))
            {
                pool = new ReferPool<IReference>(type, () => { return Activator.CreateInstance(type) as IReference; }, (t) => { t.Release(); });
                referencePoolDict.Add(type, pool);
            }
            return pool;
        }
    }
}