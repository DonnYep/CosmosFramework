using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
namespace Cosmos
{
    public interface IObjectPool : IElapesRefreshable
    {
        int ExpireTime { get; set; }
        int ReleaseInterval { get; set; }
        int Capacity { get; set; }
        Type ObjectType { get; }
        string Name { get;  }
        void SetObjectHandler(Action<Object> onSpawn, Action<Object> onDespawn);
        object Spawn();
        void Despawn(object target);
        void ClearPool();
    }
}
