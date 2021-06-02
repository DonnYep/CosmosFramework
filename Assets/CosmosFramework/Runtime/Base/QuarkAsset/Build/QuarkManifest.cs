using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Quark 
{
    [Serializable]
    public class QuarkManifest
    {
        [Serializable]
        public class ManifestItem
        {
            public string ABName{ get; set; }
            /// <summary>
            /// Manifest AssetFileHash
            /// AB打包出来之后生成的Hash码；
            /// </summary>
            public string Hash { get; set; }
            /// <summary>
            /// Assets根目录下的相对路径数组；
            /// </summary>
            public string[] Assets { get; set; }
        }
        /// <summary>
        /// Key:ABName;Value:Manifest;
        /// </summary>
        public Dictionary<string, ManifestItem> ManifestDict { get; set; }

        public QuarkManifest()
        {
            ManifestDict = new Dictionary<string, ManifestItem>();
        }
    }
}
