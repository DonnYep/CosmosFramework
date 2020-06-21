using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.Reference
{
    internal sealed class ReferencePoolManager : Module<ReferencePoolManager>
    {
        /// <summary>
        /// 单个引用池上线
        /// </summary>
        public static readonly short _ReferencePoolCapcity= 100;
        Dictionary<Type, ReferenceSpawnPool> referenceDict = new Dictionary<Type, ReferenceSpawnPool>();
        internal int GetPoolCount<T>() 
            where T : class, IReference, new()
        {
            try
            {
                    return referenceDict[typeof(T)].ReferenceCount;
            }
            catch (Exception)
            {
                throw new ArgumentNullException("Type :" + typeof(T).FullName + " not register in reference pool");
            }
        }
       internal T Spawn<T>() 
            where T: class, IReference ,new()
        {
            Type type = typeof(T);
            if (!referenceDict.ContainsKey(type))
            {
                referenceDict.Add(type, new ReferenceSpawnPool());
            }
            return referenceDict[type].Spawn<T>() as T;
        }
       internal IReference SpawnInterface<T>()
            where T : class, IReference, new()
        {
            Type type = typeof(T);
            if (!referenceDict.ContainsKey(type))
            {
                referenceDict.Add(type, new ReferenceSpawnPool());
            }
            return referenceDict[type].Spawn<T>();
        }
       internal IReference Spawn(Type type)
        {
            if (!referenceDict.ContainsKey(type))
            {
                referenceDict.Add(type, new ReferenceSpawnPool());
            }
            return referenceDict[type].Spawn(type);
        }
       internal void Despawn(IReference refer)
        {
            Type type = refer.GetType();
            if (!referenceDict.ContainsKey(type))
                referenceDict.Add(type, new ReferenceSpawnPool());
            referenceDict[type].Despawn(refer);
        }
        internal void Despawns(params IReference[] refers)
        {
            for (int i = 0; i < refers.Length; i++)
            {
                Type type = refers[i].GetType();
                if (!referenceDict.ContainsKey(type))
                    referenceDict.Add(type, new ReferenceSpawnPool());
                referenceDict[type].Despawn(refers[i]);
            }
        }
       internal void Despawns<T>(List<T> refers)
            where T:class ,IReference,new()
        {
            Type type = typeof(T);
            if (!referenceDict.ContainsKey(type))
            {
                referenceDict.Add(type, new ReferenceSpawnPool());
            }
            if (refers.Count <= 0)
                return;
            for (int i = 0; i < refers.Count; i++)
            {
                referenceDict[type].Despawn(refers[i]);
            }
            refers.Clear();
        }
       internal void Despawns<T>(T[] refers)
            where T :class,IReference,new()
        {
            Type type = typeof(T);
            if (!referenceDict.ContainsKey(type))
            {
                referenceDict.Add(type, new ReferenceSpawnPool());
            }
            if (refers.Length <= 0)
                return;
            for (int i = 0; i < refers.Length; i++)
            {
                referenceDict[type].Despawn(refers[i]);
            }
        }
       internal void Clear(Type type)
        {
            if (referenceDict.ContainsKey(type))
            {
                referenceDict[type].Clear();
            }
        }
       internal void Clear<T>()
            where T : class, IReference, new()
        {
            Type type = typeof(T);
            Clear(type);
        }
       internal void ClearAll()
        {
            foreach (var referPool in referenceDict)
            {
                referPool.Value.Clear();
            }
        }
    }
}