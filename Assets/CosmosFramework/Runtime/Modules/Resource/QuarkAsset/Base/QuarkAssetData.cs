using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Cosmos.QuarkAsset
{
    [Serializable]
    public class QuarkAssetData
    {
        public QuarkAssetLoadMode QuarkAssetLoadMode { get; set; }
        public int QuarkAssetCount { get; set; }
        public List<QuarkAssetObject> QuarkAssetObjectList { get; set; }
    }
}
