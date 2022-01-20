# if COSMOS_ADDRESSABLE
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
namespace Cosmos
{
    public class AddressableInfo : AssetInfoBase
    {
        public AddressableInfo(string assetName) : base(assetName){}
        public AddressableInfo(IResourceLocation locations) : base(locations.PrimaryKey){}
    }
}
#endif
