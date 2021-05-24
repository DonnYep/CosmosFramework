using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.QuarkAsset
{
    public class QuarkAssetABBuildInfo:IDisposable
    {
        public class AssetData
        {
            public int Id;
            public string ABName;
            public int ReferenceCount = 0;
            public string Hash;
            public List<string> DependList;
        }
        public string BuildTime;
        public Dictionary<string, AssetData> AssetDataMaps = new Dictionary<string, AssetData>();
        public void Dispose()
        {
            AssetDataMaps.Clear();
            BuildTime = null;
        }
    }
}
