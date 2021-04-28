using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.CosmosEditor
{
    [Serializable]
    public class AssetInfoData : IEditorData
    {
        public int AssetCount;
        public List<AssetInfo> AssetInfos;
        public void AddAssetInfo(string assetName, string assetExtension, string assetPath)
        {
            var info = new AssetInfo() { AssetExtension = assetExtension, AssetName = assetName, AssetPath = assetPath };
            if (AssetInfos == null)
                AssetInfos = new List<AssetInfo>();
            AssetInfos.Add(info);
            AssetCount = AssetInfos.Count;
        }
        public void Dispose()
        {
            AssetCount = 0;
            AssetInfos?.Clear();
        }
        [Serializable]
        public class AssetInfo
        {
            public string AssetName;
            public string AssetExtension;
            public string AssetPath;
        }
    }
}