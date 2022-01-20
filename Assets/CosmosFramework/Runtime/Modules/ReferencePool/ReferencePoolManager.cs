using System.Collections.Generic;
using System;
namespace Cosmos.Reference
{
    [Module]
    internal sealed class ReferencePoolManager : Module, IReferencePoolManager
    {
        #region Properties
        /// <summary>
        /// 单个引用池上限
        /// </summary>
        internal static readonly short _ReferencePoolCapcity= 5000;
        Dictionary<Type, ReferenceSpawnPool> referenceDict;
        #endregion

        #region Methods
        public override void OnInitialization()
        {
            referenceDict = new Dictionary<Type, ReferenceSpawnPool>();
        }
        public int GetPoolCount<T>() 
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
        public T Spawn<T>() 
            where T: class, IReference ,new()
        {
            Type type = typeof(T);
            if (!referenceDict.ContainsKey(type))
            {
                referenceDict.Add(type, new ReferenceSpawnPool());
            }
            return referenceDict[type].Spawn<T>() as T;
        }
        public IReference SpawnInterface<T>()
            where T : class, IReference, new()
        {
            Type type = typeof(T);
            if (!referenceDict.ContainsKey(type))
            {
                referenceDict.Add(type, new ReferenceSpawnPool());
            }
            return referenceDict[type].Spawn<T>();
        }
        public IReference Spawn(Type type)
        {
            if (!referenceDict.ContainsKey(type))
            {
                referenceDict.Add(type, new ReferenceSpawnPool());
            }
            return referenceDict[type].Spawn(type);
        }
        public void Despawn(IReference refer)
        {
            Type type = refer.GetType();
            if (!referenceDict.ContainsKey(type))
                referenceDict.Add(type, new ReferenceSpawnPool());
            referenceDict[type].Despawn(refer);
        }
        public void Despawns(params IReference[] refers)
        {
            for (int i = 0; i < refers.Length; i++)
            {
                Type type = refers[i].GetType();
                if (!referenceDict.ContainsKey(type))
                    referenceDict.Add(type, new ReferenceSpawnPool());
                referenceDict[type].Despawn(refers[i]);
            }
        }
        public void Despawns<T>(List<T> refers)
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
        public void Despawns<T>(T[] refers)
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
        public void Clear(Type type)
        {
            if (referenceDict.ContainsKey(type))
            {
                referenceDict[type].Clear();
            }
        }
        public void Clear<T>()
            where T : class, IReference, new()
        {
            Type type = typeof(T);
            Clear(type);
        }
        public void ClearAll()
        {
            foreach (var referPool in referenceDict)
            {
                referPool.Value.Clear();
            }
        }
        #endregion
    }
}