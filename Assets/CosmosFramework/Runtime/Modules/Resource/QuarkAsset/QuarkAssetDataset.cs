using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.QuarkAsset
{
    public class QuarkAssetDataset : ScriptableObject
    {
        public static QuarkAssetDataset Instance{ get; private set; }
        public QuarkAssetLoadMode QuarkAssetLoadMode;
        public int QuarkAssetCount;
        public List<QuarkAssetObject> QuarkAssetObjectList;
        [RuntimeInitializeOnLoadMethod]
        static void InitSingleton()
        {
        }
    }
}