using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.Quark 
{
    /// <summary>
    /// QuarkAssetDataset用于在Editor Runtime快速开发时使用；
    /// build之后需配合AB资源使用；
    /// </summary>
    [Serializable]
    public class QuarkAssetDataset : DatasetBase
    {
        /// <summary>
        /// 包含的路径；
        /// </summary>
        public List<string> IncludeDirectories;
        public int QuarkAssetCount;
        public List<QuarkAssetDatabaseObject> QuarkAssetObjectList;
        public override void Dispose()
        {
            QuarkAssetCount = 0;
            QuarkAssetObjectList?.Clear();
        }
    }
}