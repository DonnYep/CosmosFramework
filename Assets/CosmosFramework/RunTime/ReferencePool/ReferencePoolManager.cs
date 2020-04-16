using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.Reference
{
    public sealed class ReferencePoolManager : Module<ReferencePoolManager>
    {
        /// <summary>
        /// 单个引用池上线
        /// </summary>
        public static readonly short _ReferencePoolCapcity= 100;
        Dictionary<Type, ReferenceSpawnPool> referencePool = new Dictionary<Type, ReferenceSpawnPool>();
        public int GetPoolCount<T>() 
            where T : class, IReference, new()
        {
            if (referencePool.ContainsKey(typeof(T)))
                return referencePool[typeof(T)].ReferenceCount;
            else
            {
                Utility.DebugError("Type :" + typeof(T).FullName + " not register in reference pool");
                return -1;
            }
        }
        public T Spawn<T>() 
            where T:class ,IReference ,new()
        {
            return Spawn(typeof(T)) as T;
        }
        public IReference Spawn(Type type)
        {
            if (!referencePool.ContainsKey(type))
            {
                referencePool.Add(type, new ReferenceSpawnPool());
            }
            return referencePool[type].Spawn(type);
        }
        public void Despawn(IReference refer)
        {
            Type type = refer.GetType();
            if (!referencePool.ContainsKey(type))
                referencePool.Add(type, new ReferenceSpawnPool());
            referencePool[type].Despawn(refer);
        }
        public void Despawns<T>(List<T> refers)
            where T:class ,IReference,new()
        {
            Type type = typeof(T);
            if (!referencePool.ContainsKey(type))
            {
                referencePool.Add(type, new ReferenceSpawnPool());
            }
            for (int i = 0; i < refers.Count; i++)
            {
                referencePool[type].Despawn(refers[i]);
            }
            refers.Clear();
        }
        public void Despawns<T>(T[] refers)
            where T :class,IReference,new()
        {
            Type type = typeof(T);
            if (!referencePool.ContainsKey(type))
            {
                referencePool.Add(type, new ReferenceSpawnPool());
            }
            for (int i = 0; i < refers.Length; i++)
            {
                referencePool[type].Despawn(refers[i]);
            }
        }
        public void Clear(Type type)
        {
            if (referencePool.ContainsKey(type))
            {
                referencePool[type].Clear();
            }
        }
        public void Clear()
        {
            foreach (var referPool in referencePool)
            {
                referPool.Value.Clear();
            }
        }
    }
}