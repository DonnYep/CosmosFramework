using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace Cosmos.QuarkAsset
{
    public enum QuarkAssetLoadMode : byte
    {
        None = 0x0,
        AssetDataBase = 0x1,
        BuiltAssetBundle = 0x2
    }
    public static class QuarkUtility
    {
        static QuarkAssetDataset quarkAssetData;
        static Dictionary<string, LinkedList<QuarkAssetObject>> assetDict;
        public static QuarkAssetDataset QuarkAssetData { get { return quarkAssetData; } }
        public static T LoadAssetByPath<T>(string assetPath)
            where T : UnityEngine.Object
        {
            T asset = null;
            switch (quarkAssetData.QuarkAssetLoadMode)
            {
                case QuarkAssetLoadMode.AssetDataBase:
                    asset = AssetDatabaseLoadAssetByPath<T>(assetPath);
                    break;
                case QuarkAssetLoadMode.BuiltAssetBundle:
                    break;
            }
            return asset;
        }
        public static T LoadAssetByName<T>(string assetName, string assetExtension = null)
            where T : UnityEngine.Object
        {
            T asset = null;
            switch (quarkAssetData.QuarkAssetLoadMode)
            {
                case QuarkAssetLoadMode.AssetDataBase:
                    asset = AssetDatabaseLoadAssetByName<T>(assetName, assetExtension);
                    break;
                case QuarkAssetLoadMode.BuiltAssetBundle:
                    break;
            }
            return asset;
        }
        public static GameObject LoadPrefab(string assetName, string assetExtension = null)
        {
            return LoadAssetByName<GameObject>(assetName, assetExtension);
        }
        public static GameObject Instantiate<T>(string assetName, string assetExtension = null)
        {
            var go = LoadAssetByName<GameObject>(assetName, assetExtension);
            return GameObject.Instantiate(go);
        }
        public static void UnLoadAsset()
        {

        }
        public static void SetData(QuarkAssetDataset assetData, Dictionary<string, LinkedList<QuarkAssetObject>> lnkData)
        {
            assetDict = lnkData;
            quarkAssetData = assetData;
        }
        static T AssetDatabaseLoadAssetByName<T>(string assetName, string assetExtension = null)
            where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            QuarkAssetObject quarkAssetObject = new QuarkAssetObject();
            if (assetDict == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
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
#if UNITY_EDITOR
            var guid2path = UnityEditor.AssetDatabase.GUIDToAssetPath(quarkAssetObject.AssetGuid);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(guid2path);
#else
            return null;
#endif
        }
        static T AssetDatabaseLoadAssetByPath<T>(string assetPath)
       where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentNullException("Asset name is invalid!");
            if (assetDict == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            return null;
#endif
        }
    }
}