using System;
using System.Collections.Generic;
 using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface IReferencePoolManager: IModuleManager
    {
        int GetPoolCount<T>() where T : class, IReference, new();
        T Spawn<T>() where T : class, IReference, new();
        IReference SpawnInterface<T>() where T : class, IReference, new();
        IReference Spawn(Type type);
        void Despawn(IReference refer);
        void Despawns(params IReference[] refers);
        void Despawns<T>(List<T> refers) where T : class, IReference, new();
        void Despawns<T>(T[] refers) where T : class, IReference, new();
        void Clear(Type type);
        void Clear<T>() where T : class, IReference, new();
        void ClearAll();
    }
}
