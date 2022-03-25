using System.Collections.Generic;
using System;
using UnityEngine;
namespace Quark.Asset
{
    /// <summary>
    /// QuarkAssetDataset用于在Editor Runtime快速开发时使用；
    /// build之后需配合AB资源使用；
    /// </summary>
    [Serializable]
    public sealed class QuarkAssetDataset : ScriptableObject, IDisposable
    {
        /// <summary>
        /// 包含的路径；
        /// </summary>
        public List<QuarkDirHashPair> DirHashPairs;
        public int QuarkAssetCount;
        public List<QuarkAssetDatabaseObject> QuarkAssetObjectList;
        public void Init()
        {
            DirHashPairs = new List<QuarkDirHashPair>();
            QuarkAssetObjectList = new List<QuarkAssetDatabaseObject>();
        }
        public void Dispose()
        {
            QuarkAssetCount = 0;
            QuarkAssetObjectList?.Clear();
            DirHashPairs.Clear();
        }
    }
}