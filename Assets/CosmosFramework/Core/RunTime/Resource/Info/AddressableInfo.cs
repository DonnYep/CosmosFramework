using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
