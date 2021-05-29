using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.QuarkAsset
{
    [Serializable]
    public class QuarkAssetABBuildInfo
    {

        [Serializable]
        public class AssetData
        {
            public int Id { get; set; }
            public string ABName { get; set; }
            /// <summary>
            /// AB打包出来的Hash；
            /// </summary>
            public string ABHash { get; set; }
            public int ReferenceCount { get; set; } = 0;
            public List<string> DependList { get; set; }
        }
        public QuarkAssetABBuildInfo()
        {
            AssetDataMaps = new Dictionary<string, AssetData>();
        }
        public string BuildTime { get; set; }
        public Dictionary<string, AssetData> AssetDataMaps { get; set; } 
        public void Dispose()
        {
            AssetDataMaps.Clear();
            BuildTime = null;
        }
    }
}
