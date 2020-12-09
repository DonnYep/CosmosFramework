using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface IObjectSpawnPool : IElapesRefreshable
    {
        int ExpireTime { get; set; }
        int ReleaseInterval { get; set; }
        int Capacity { get; set; }
        void SetObjectHandler(Action<UnityEngine.GameObject> onSpawn, Action<UnityEngine.GameObject> onDespawn);
    }
}
