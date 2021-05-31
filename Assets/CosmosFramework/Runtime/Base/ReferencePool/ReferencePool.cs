using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    public static class ReferencePool
    {
        sealed class ReferPool<T>
            where T : class
        {
            public int PoolAccquireCount { get { return poolAccquireCount; } }
            public int AccquireCount { get { return accquireCount; } }
            public int ReleaseCount { get { return releaseCount; } }
            public int Count { get { return pool.Count; } }
            public Type ReferenceType { get { return referenceType; } }
            readonly Pool<T> pool;
            readonly Type referenceType;
            int poolAccquireCount = 0;
            int accquireCount = 0;
            int releaseCount = 0;
            public ReferPool(Type type, Func<T> objectGenerator, Action<T> onRelease)
            {
                referenceType = type;
                pool = new Pool<T>(objectGenerator, onRelease);
            }
            public T Accquire()
            {
                poolAccquireCount++;
                accquireCount++;
                return pool.Spawn();
            }
            public void Release(T obj)
            {
                accquireCount--;
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
        public static T Accquire<T>() where T : class, IReference, new()
        {
            return GetReferencePool(typeof(T)).Accquire() as T;
        }
        public static IReference Accquire(Type type)
        {
            CheckAccquireType(type);
            return GetReferencePool(type).Accquire();
        }
        public static void Release(IReference reference)
        {
            if (reference == null)
                throw new ArgumentNullException("Reference is invalid.");
            var type = reference.GetType();
            GetReferencePool(type).Release(reference);
        }
        public static void Release<T>(T[] references) where T : class, IReference, new()
        {
            if (references == null)
                throw new ArgumentNullException("Reference array is invalid.");
            var type = typeof(T);
            var pool = GetReferencePool(type);
            var length = references.Length;
            for (int i = 0; i < length; i++)
            {
                pool.Release(references[i]);
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
            CheckAccquireType(type);
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
            return new ReferencePoolInfo(type, pool.AccquireCount, pool.ReleaseCount, pool.PoolAccquireCount);
        }
        public static ReferencePoolInfo GetReferencePoolInfo(Type type)
        {
            CheckAccquireType(type);
            var pool = GetReferencePool(type);
            return new ReferencePoolInfo(type, pool.AccquireCount, pool.ReleaseCount, pool.PoolAccquireCount);
        }
        public static ReferencePoolInfo[] GetAllReferencePoolInfos()
        {
            int index = 0;
            ReferencePoolInfo[] pools = new ReferencePoolInfo[Count];
            foreach (var pool in referencePoolDict.Values)
            {
                pools[index++] = new ReferencePoolInfo (pool.ReferenceType, pool.AccquireCount, pool.ReleaseCount, pool.PoolAccquireCount);
            }
            return pools;
        }

        static void CheckAccquireType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Reference type is invalid !");
            if (!type.IsClass || type.IsAbstract)
                throw new ArgumentException("Reference type is not a non-abstract class type !");
            if (typeof(IReference).IsAssignableFrom(type))
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