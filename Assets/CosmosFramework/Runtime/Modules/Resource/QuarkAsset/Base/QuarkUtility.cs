using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace Cosmos.QuarkAsset
{
    public enum QuarkAssetLoadMode : byte
    {
        None = 0,
        AssetDataBase = 1,
        BuiltAssetBundle = 2
    }
    public static class QuarkUtility
    {
        static QuarkAssetLoadMode mode;
        static QuarkAssetData quarkAssetData;
        static Dictionary<string, LinkedList<QuarkAssetObject>> assetDict;
        public static QuarkAssetData QuarkAssetData { get { return quarkAssetData; } }
        public static void SetAndSaveQuarkAsset(QuarkAssetData assetData)
        {
#if UNITY_EDITOR
            quarkAssetData = assetData;
            var json = Utility.Json.ToJson(quarkAssetData, true);
            var lnkDict = EncodeSchema(assetData);
            assetDict = lnkDict;
            var linkedJson = Utility.Json.ToJson(lnkDict, true);
            Utility.IO.OverwriteTextFile(ApplicationConst.LibraryPath, QuarkAssetConst.QuarkAssetFileName, json);
            Utility.IO.OverwriteTextFile(ApplicationConst.LibraryPath, QuarkAssetConst.LinkedQuarkAssetFileName, linkedJson);
#endif
        }
        public static QuarkAssetData LoadQuarkAssetData()
        {
            var filePath = Utility.IO.CombineRelativeFilePath(QuarkAssetConst.QuarkAssetFileName, ApplicationConst.LibraryPath);
            var lnkPath = Utility.IO.CombineRelativeFilePath(QuarkAssetConst.LinkedQuarkAssetFileName, ApplicationConst.LibraryPath);
            var json = Utility.IO.ReadTextFileContent(filePath);
            var lnkJson = Utility.IO.ReadTextFileContent(lnkPath);
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(lnkJson))
                return null;
            quarkAssetData = Utility.Json.ToObject<QuarkAssetData>(json);
            assetDict = Utility.Json.ToObject<Dictionary<string, LinkedList<QuarkAssetObject>>>(lnkJson);
            return Utility.Json.ToObject<QuarkAssetData>(json);
        }
        public static void ClearQuarkAsset()
        {
            var filePath = Utility.IO.CombineRelativeFilePath(QuarkAssetConst.QuarkAssetFileName, ApplicationConst.LibraryPath);
            var lnkPath = Utility.IO.CombineRelativeFilePath(QuarkAssetConst.LinkedQuarkAssetFileName, ApplicationConst.LibraryPath);
            Utility.IO.DeleteFile(filePath);
            Utility.IO.DeleteFile(lnkPath);
        }
        public static T LoadAsset<T>(string assetName, string assetExtension = null)
            where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            QuarkAssetObject quarkAssetObject = new QuarkAssetObject();
            if (assetDict == null)
            {
                var lnkPath = Utility.IO.CombineRelativeFilePath(QuarkAssetConst.LinkedQuarkAssetFileName, ApplicationConst.LibraryPath);
                try
                {
                    var lnkJson = Utility.IO.ReadTextFileContent(lnkPath);
                    assetDict = Utility.Json.ToObject<Dictionary<string, LinkedList<QuarkAssetObject>>>(lnkJson);
                }
                catch
                {
                    throw new Exception("未执行QuarkAsset build 操作");
                }
            }
            if (assetDict.TryGetValue(assetName, out var lnk))
            {
                if (!string.IsNullOrEmpty(assetExtension))
                {
                    foreach (var assetObj in lnk)
                    {
                        if (assetObj.AssetExtension.Equals(assetExtension))
                        {
                            quarkAssetObject = assetObj;
                            break;
                        }
                    }
                }
                else
                {
                    quarkAssetObject = lnk.First.Value;
                }
            }
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(quarkAssetObject.AssetPath);
        }
        static Dictionary<string, LinkedList<QuarkAssetObject>> EncodeSchema(QuarkAssetData assetData)
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
    }
}
