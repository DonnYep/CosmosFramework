using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.QuarkAsset
{
    [Serializable]
    public struct QuarkAssetObject
    {
        public string AssetName { get; set; }
        public string AssetExtension { get; set; }
        public string AssetPath { get; set; }
        public string AssetType { get; set; }
    }
}