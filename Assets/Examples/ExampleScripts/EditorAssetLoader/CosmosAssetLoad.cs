using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Cosmos.Test
{
    public class CosmosAssetLoad
    {
        //static AssetInfoData assetInfo;
        //public static void LoadAssetByPath<T>(string assetPath) where T : Object
        //{
        //    AssetDatabase.LoadAssetAtPath<T>(assetPath);
        //}
        //public static T LoadAssetByName<T>(string assetName) where T : Object
        //{
        //    if (assetInfo == null)
        //    {
        //        assetInfo = CosmosEditorUtility.ReadEditorData<AssetInfoData>("AssetInfoData.json");
        //    }
        //    var length = assetInfo.AssetInfos.Count;
        //    for (int i = 0; i < length; i++)
        //    {
        //        if (assetInfo.AssetInfos[i].AssetName.Contains(assetName))
        //        {
        //            var dirtyPath =assetInfo.AssetInfos[i].AssetPath;
        //            return AssetDatabase.LoadAssetAtPath<T>(dirtyPath);
        //        }
        //    }
        //    return null;
        //}
    }
}