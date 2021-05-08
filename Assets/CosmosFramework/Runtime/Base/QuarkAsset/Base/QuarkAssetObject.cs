using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.QuarkAsset
{
    [Serializable]
    public struct QuarkAssetObject
    {
        public string AssetName;
        public string AssetExtension;
        public string AssetPath;
        public string AssetType;
        public string AssetGuid;
    }
}