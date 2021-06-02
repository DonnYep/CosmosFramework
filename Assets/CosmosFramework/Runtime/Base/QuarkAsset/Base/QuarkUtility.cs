using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Cosmos.Quark 
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
        static Dictionary<string, LinkedList<QuarkAssetDatabaseObject>> assetDatabaseMap;
        /// <summary>
        /// BuiltAssetBundle 模式下资源的映射；
        /// Key : ABName---Value :  Lnk [QuarkAssetABObject]
        /// </summary>
        static Dictionary<string, LinkedList<QuarkAssetBundleObject>> builtAssetBundleMap;

        public static QuarkAssetDataset QuarkAssetData { get { return quarkAssetData; } }
        /// <summary>
        /// 对Manifest进行编码；
        /// </summary>
        /// <param name="manifest">unityWebRequest获取的Manifest文件对象</param>
        public static void SetBuiltAssetBundleModeData(QuarkManifest manifest)
        {
            var lnkDict = new Dictionary<string, LinkedList<QuarkAssetBundleObject>>();
            foreach (var mf in manifest.ManifestDict)
            {
                var assetPaths = mf.Value.Assets;
                var length = assetPaths.Length;
                for (int i = 0; i < length; i++)
                {
                    var qab = GetAssetBundleObject(mf.Value.ABName, assetPaths[i]);
                    if (!lnkDict.TryGetValue(qab.AssetName, out var lnkList))
                    {
                        lnkList = new LinkedList<QuarkAssetBundleObject>();
                        lnkList.AddLast(qab);
                        lnkDict.Add(qab.AssetName, lnkList);
                    }
                    else
                    {
                        lnkList.AddLast(qab);
                    }
                }
            }
            builtAssetBundleMap = lnkDict;
        }
        /// <summary>
        /// 对QuarkAssetDataset进行编码
        /// </summary>
        /// <param name="assetData">QuarkAssetDataset对象</param>
        public static void SetAssetDatabaseModeData(QuarkAssetDataset assetData)
        {
            var lnkDict = new Dictionary<string, LinkedList<QuarkAssetDatabaseObject>>();
            var length = assetData.QuarkAssetObjectList.Count;
            for (int i = 0; i < length; i++)
            {
                var name = assetData.QuarkAssetObjectList[i].AssetName;
                if (!lnkDict.TryGetValue(name, out var lnkList))
                {
                    var lnk = new LinkedList<QuarkAssetDatabaseObject>();
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



        public static T LoadAsset<T>(string assetName, string assetExtension = null)
            where T : UnityEngine.Object
        {
            T asset = null;
            var isPath = IsPath(assetName);
            switch (QuarkAssetLoadMode)
            {
                case QuarkAssetLoadMode.AssetDatabase:
                    {
#if UNITY_EDITOR
                        if (isPath)
                            asset = AssetDatabaseLoadAssetByPath<T>(assetName);
                        else
                            asset = AssetDatabaseLoadAssetByName<T>(assetName, assetExtension);
#endif
                    }
                    break;
                case QuarkAssetLoadMode.BuiltAssetBundle:
                    {
                        if (isPath)
                            asset = BuiltAssetBundleLoadAssetByPath<T>(assetName);
                        else
                            asset = BuiltAssetBundleLoadAssetByName<T>(assetName, assetExtension);
                    }
                    break;
            }
            return asset;
        }
        public static GameObject LoadPrefab(string assetName, string assetExtension = null)
        {
            return LoadAsset<GameObject>(assetName, assetExtension);
        }
        public static GameObject Instantiate(string assetName, string assetExtension = null)
        {
            var go = LoadAsset<GameObject>(assetName, assetExtension);
            return GameObject.Instantiate(go);
        }
        public static void UnLoadAsset()
        {

        }
#if UNITY_EDITOR
        static T AssetDatabaseLoadAssetByName<T>(string assetName, string assetExtension = null)
            where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            QuarkAssetDatabaseObject quarkAssetDatabaseObject = new QuarkAssetDatabaseObject();
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            if (assetDatabaseMap.TryGetValue(assetName, out var lnk))
                quarkAssetDatabaseObject = GetAssetDatabaseObject<T>(lnk, assetExtension);
            if (!string.IsNullOrEmpty(quarkAssetDatabaseObject.AssetGuid))
            {
                var guid2path = UnityEditor.AssetDatabase.GUIDToAssetPath(quarkAssetDatabaseObject.AssetGuid);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(guid2path);
            }
            else
                return null;
        }
        static T AssetDatabaseLoadAssetByPath<T>(string assetPath)
       where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentNullException("Asset name is invalid!");
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
#endif

        static T BuiltAssetBundleLoadAssetByName<T>(string assetPath, string assetExtension = null)
where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentNullException("Asset name is invalid!");
            QuarkAssetBundleObject abObject = QuarkAssetBundleObject.None;
            if (builtAssetBundleMap.TryGetValue(assetPath, out var lnkList))
            {
                if (!string.IsNullOrEmpty(assetExtension))
                {
                    abObject = lnkList.First.Value;
                }
                else
                {
                    foreach (var obj in lnkList)
                    {
                        if (obj.AssetExtension == assetExtension)
                        {
                            abObject = obj;
                            break;
                        }
                    }
                }

                //TODO Get AssetBundle

            }
            return null;
        }
        static T BuiltAssetBundleLoadAssetByPath<T>(string assetPath)
where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentNullException("Asset name is invalid!");

            return null;
        }
        static QuarkAssetDatabaseObject GetAssetDatabaseObject<T>(LinkedList<QuarkAssetDatabaseObject> lnk, string assetExtension = null)
            where T : UnityEngine.Object
        {
            var assetType = typeof(T).ToString();
            QuarkAssetDatabaseObject quarkAssetObject = new QuarkAssetDatabaseObject();
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
                    quarkAssetObject = tempObj.AssetExtension == assetExtension == true ? tempObj : QuarkAssetDatabaseObject.None;
                }
                else
                {
                    quarkAssetObject = tempObj;
                }
            }
            return quarkAssetObject;
        }
        static QuarkAssetBundleObject GetAssetBundleObject(string abName, string assetName)
        {
            var qab = new QuarkAssetBundleObject()
            {
                AssetBundleName = abName,
                AssetPath = assetName,
            };
            var strs = Utility.Text.StringSplit(assetName, new string[] { "/" });
            var nameWithExt = strs[strs.Length - 1];
            var splits = Utility.Text.StringSplit(nameWithExt, new string[] { "." });
            qab.AssetExtension = Utility.Text.Combine(".", splits[splits.Length - 1]);
            qab.AssetName = nameWithExt.Replace(qab.AssetExtension, "");
            return qab;
        }
        static bool IsPath(string context)
        {
            return context.Contains("Assets/");
        }
    }
}