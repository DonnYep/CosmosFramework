using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos.CosmosEditor
{
    [Serializable]
    public class AssetInfoData : IEditorData
    {
        public List<AssetInfo> AssetInfos;
        public void AddAssetInfo(string assetName,string assetExtension,string assetPath)
        {
         var info=   new AssetInfo() { AssetExtension = assetExtension, AssetName = assetName, AssetPath = assetPath };
            if (AssetInfos == null)
                AssetInfos = new List<AssetInfo>();
            AssetInfos.Add(info); ; ;
        }
        public void Dispose()
        {
            AssetInfos.Clear(); ;
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