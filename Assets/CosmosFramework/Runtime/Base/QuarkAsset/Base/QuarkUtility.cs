using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Cosmos.QuarkAsset
{
    public enum QuarkAssetLoadMode : byte
    {
        None = 0x0,
        AssetDatabase = 0x1,
        BuiltAssetBundle = 0x2
    }
    public static class QuarkUtility
    {
        public static QuarkAssetLoadMode QuarkAssetLoadMode { get; set; }
        static QuarkAssetDataset quarkAssetData;
        /// <summary>
        /// AssetDataBase模式下资源的映射字典；
        /// Key : AssetName---Value :  Lnk [QuarkAssetObject]
        /// </summary>
        static Dictionary<string, LinkedList<QuarkAssetObject>> assetDatabaseMap;
        /// <summary>
        /// BuiltAssetBundle 模式下资源的映射；
        /// Key : ABName---Value :  Lnk [QuarkAssetABObject]
        /// </summary>
        static Dictionary<string, LinkedList<QuarkAssetABObject>> builtAssetBundleMap;

        public static QuarkAssetDataset QuarkAssetData { get { return quarkAssetData; } }
        /// <summary>
        /// 对Manifest进行编码；
        /// </summary>
        /// <param name="manifest">unityWebRequest获取的Manifest文件对象</param>
        public static void SetBuiltAssetBundleModeData(QuarkAssetManifest manifest)
        {
            var lnkDict = new Dictionary<string, LinkedList<QuarkAssetABObject>>();
            foreach (var mf in manifest.ManifestDict)
            {
                var assetPaths = mf.Value.Assets;
                var length = assetPaths.Length;
                for (int i = 0; i < length; i++)
                {
                    if (!lnkDict.TryGetValue(assetPaths[i], out var lnkList))
                    {
                        lnkList= new LinkedList<QuarkAssetABObject>();
                        var qab = GetQuarkAssetObject(mf.Value.ABName, assetPaths[i]);
                        lnkList.AddLast(qab);
                        lnkDict.Add(assetPaths[i], lnkList);
                    }
                    else
                    {
                        var qab = GetQuarkAssetObject(mf.Value.ABName, assetPaths[i]);
                        lnkList.AddLast(qab);
                    }
                }
            }
            builtAssetBundleMap = lnkDict;
        }
        public static void SetAssetDatabaseModeData(QuarkAssetDataset assetData)
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
            quarkAssetData = assetData;
            assetDatabaseMap = lnkDict;
        }
        public static T LoadAssetByPath<T>(string assetPath)
            where T : UnityEngine.Object
        {
            T asset = null;
            switch (QuarkAssetLoadMode)
            {
                case QuarkAssetLoadMode.AssetDatabase:
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
            switch (QuarkAssetLoadMode)
            {
                case QuarkAssetLoadMode.AssetDatabase:
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
        public static GameObject Instantiate(string assetName, string assetExtension = null)
        {
            var go = LoadAssetByName<GameObject>(assetName, assetExtension);
            return GameObject.Instantiate(go);
        }
        public static void UnLoadAsset()
        {

        }
        public static void SetData(QuarkAssetDataset assetData, Dictionary<string, LinkedList<QuarkAssetObject>> lnkData)
        {
            assetDatabaseMap = lnkData;
            quarkAssetData = assetData;
        }
        static T AssetDatabaseLoadAssetByName<T>(string assetName, string assetExtension = null)
            where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            QuarkAssetObject quarkAssetObject = new QuarkAssetObject();
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            if (assetDatabaseMap.TryGetValue(assetName, out var lnk))
                quarkAssetObject = GetQuarkAssetObject<T>(lnk, assetExtension);
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(quarkAssetObject.AssetGuid))
            {
                var guid2path = UnityEditor.AssetDatabase.GUIDToAssetPath(quarkAssetObject.AssetGuid);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(guid2path);
            }
            else
                return null;
#else
            return null;
#endif
        }
        static T AssetDatabaseLoadAssetByPath<T>(string assetPath)
       where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentNullException("Asset name is invalid!");
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            return null;
#endif
        }
        static QuarkAssetObject GetQuarkAssetObject<T>(LinkedList<QuarkAssetObject> lnk, string assetExtension = null)
            where T : UnityEngine.Object
        {
            var assetType = typeof(T).ToString();
            QuarkAssetObject quarkAssetObject = new QuarkAssetObject();
            var tempObj = lnk.First.Value;
            if (tempObj.AssetType != assetType)
            {
                foreach (var assetObj in lnk)
                {
                    if (assetObj.AssetType == assetType)
                    {
                        if (!string.IsNullOrEmpty(assetExtension))
                        {
                            if (assetObj.AssetExtension == assetExtension)
                            {
                                quarkAssetObject = assetObj;
                                break;
                            }
                        }
                        else
                        {
                            quarkAssetObject = assetObj;
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(assetExtension))
                {
                    quarkAssetObject = tempObj.AssetExtension == assetExtension == true ? tempObj : QuarkAssetObject.None;
                }
                else
                {
                    quarkAssetObject = tempObj;
                }
            }
            return quarkAssetObject;
        }


        static QuarkAssetABObject GetQuarkAssetObject(string abName,string assetName)
        {
            var qab = new QuarkAssetABObject()
            {
                AssetBundleName = abName,
                AssetPath = assetName,
            };
            var strs = Utility.Text.StringSplit(assetName, new string[] { "/" });
            var nameWithExt = strs[strs.Length - 1];
            qab.AssetName = nameWithExt;
            var splits = Utility.Text.StringSplit(nameWithExt, new string[] { "." });
            qab.AssetExtension = Utility.Text.Combine(".", splits[splits.Length - 1]);
            return qab;
        }
    }
}