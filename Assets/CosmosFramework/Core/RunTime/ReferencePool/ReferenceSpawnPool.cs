using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.Reference
{
    internal sealed class ReferenceSpawnPool 
    {
        Queue<IReference> referenceQueue = new Queue<IReference>();
        internal int ReferenceCount { get { return referenceQueue.Count; } }
        internal IReference Spawn(Type type)
        {
            IReference refer;
            if (referenceQueue.Count > 0)
                refer = referenceQueue.Dequeue();
            else
                refer = Utility.Assembly.GetTypeInstance(type) as IReference;
            return refer;
        }
        internal IReference Spawn<T>()
            where T:class, IReference, new()
        {
            IReference refer;
            if (referenceQueue.Count > 0)
                refer = referenceQueue.Dequeue();
            else
                refer = new T() as IReference;
            return refer;
        }
        internal void Despawn(IReference refer)
        {
            if (referenceQueue.Count >= ReferencePoolManager._ReferencePoolCapcity)
                refer = null;
            else
            {
                refer.Clear();
                referenceQueue.Enqueue(refer);
            }
        }
        internal void Clear()
        {
            referenceQueue.Clear();
        }
    }
}