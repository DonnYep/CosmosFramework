using Cosmos.ObjectPool;
using System.Threading.Tasks;

namespace Cosmos.Extensions
{
    public static class ObjectPoolManagerExts
    {
        public static async Task<IObjectPool> RegisterObjectPoolAsync(this IObjectPoolManager @this, ObjectPoolAssetInfo assetInfo)
        {
            IObjectPool pool = null;
            await @this.RegisterObjectPoolAsync(assetInfo, (p) => { pool = p; });
            return pool;
        }
    }
}
