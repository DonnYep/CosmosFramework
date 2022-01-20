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
