using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.QuarkAsset
{
    [Serializable]
    public class QuarkAssetManifest
    {
        [Serializable]
        public class ManifestItem
        {
            /// <summary>
            /// Manifest AssetFileHash
            /// </summary>
            public string Hash { get; set; }
            /// <summary>
            /// Assets根目录下的相对路径数组；
            /// </summary>
            public string[] Assets { get; set; }
        }
        public Dictionary<string, ManifestItem> ManifestDict { get; set; }

        public QuarkAssetManifest()
        {
            ManifestDict = new Dictionary<string, ManifestItem>();
        }
    }
}
