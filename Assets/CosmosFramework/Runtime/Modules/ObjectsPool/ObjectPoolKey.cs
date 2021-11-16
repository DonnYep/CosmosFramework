using System;
namespace Cosmos.ObjectPool
{
    public struct ObjectPoolKey : IEquatable<ObjectPoolKey>
    {
        readonly Type poolType;
        readonly string poolName;
        public Type PoolType { get { return poolType; } }
        public string PoolName { get { return poolName; } }
        public ObjectPoolKey(Type poolType, string poolName)
        {
            if (poolType == null)
                throw new ArgumentNullException("poolType is invalid !");
            this.poolType = poolType;
            this.poolName = poolName;
        }
        public ObjectPoolKey(Type poolType):this(poolType,string.Empty){}
        public bool Equals(ObjectPoolKey other)
        {
            return other.poolName == poolName && other.poolType == poolType;
        }
        public override bool Equals(object obj)
        {
            return obj is ObjectPoolKey && Equals((ObjectPoolKey)obj);
        }
        public static bool operator ==(ObjectPoolKey a, ObjectPoolKey b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(ObjectPoolKey a, ObjectPoolKey b)
        {
            return !(a == b);
        }
        readonly public static ObjectPoolKey None = new ObjectPoolKey(typeof(object));
        public override int GetHashCode()
        {
            return poolType.GetHashCode() ^ poolName.GetHashCode();
        }
        public override string ToString()
        {
            if (poolType == null)
                throw new ArgumentNullException("poolType is invalid !");
            var typeName = poolType.FullName;
            return string.IsNullOrEmpty(poolName) ? typeName : $"{typeName}.{poolName}";
        }

    }
}
