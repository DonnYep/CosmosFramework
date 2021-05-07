using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.QuarkAsset;
using UnityEngine;

namespace Cosmos.CosmosEditor
{
    public static partial class QuarkAssetEditorUtility
    {
        static Dictionary<string, LinkedList<QuarkAssetObject>> EncodeSchema(QuarkAssetDataset assetData)
        {
            var lnkDict = new Dictionary<string, LinkedList<QuarkAssetObject>>();
            var length = assetData.QuarkAssetObjectList.Count;
            for (int i = 0; i < length; i++)
            {
                var name = assetData.QuarkAssetObjectList[i].AssetName;
                if (!lnkDict.TryGetValue(name, out var lnkList))
                {
                    var lnk = new LinkedList<QuarkAssetObject>();
                    lnk.AddLast(assetData.QuarkAssetObjectList[i]);
                    lnkDict.Add(name, lnk);
                }
                else
                {
                    lnkList.AddLast(assetData.QuarkAssetObjectList[i]);
                }
            }
            return lnkDict;
        }
        [RuntimeInitializeOnLoadMethod]
        static void InitQuarkAssetData()
        {
            var quarkAssetData = QuarkAssetEditorUtility.Dataset.QuarkAssetDatasetInstance;
            var assetDict = EncodeSchema(quarkAssetData);
            QuarkUtility.SetData(quarkAssetData, assetDict);
        }
    }
}
