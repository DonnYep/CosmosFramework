using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.QuarkAsset
{
    /// <summary>
    /// QuarkAssetDataset用于在Editor Runtime快速开发时使用；
    /// build之后需配合AB资源使用；
    /// </summary>
    [Serializable]
    public class QuarkAssetDataset : DatasetBase
    {
        public QuarkAssetLoadMode QuarkAssetLoadMode;
        public int QuarkAssetCount;
        public List<QuarkAssetObject> QuarkAssetObjectList;
        public override void Dispose()
        {
            QuarkAssetLoadMode = QuarkAssetLoadMode.None;
            QuarkAssetCount = 0;
            QuarkAssetObjectList?.Clear();
        }
    }
}