using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
# if COSMOS_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Cosmos
{
    public interface IResourceHelper
    {
        void LoadAssetsAsync<T>(AssetInfo info, Action<T> callback) where T : UnityEngine.Object;
    }
}
#endif
