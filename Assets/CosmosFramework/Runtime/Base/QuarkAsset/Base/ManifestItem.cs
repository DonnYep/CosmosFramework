using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.QuarkAsset
{
    public class ManifestItem
    {
        public string CRC { get; set; }
        /// <summary>
        /// Manifest AssetFileHash
        /// </summary>
        public string Hash { get; set; }
        public string[] Assets { get; set; }
    }
}
