using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using UnityEngine;

namespace Cosmos.Quark
{
    //================================================
    //1、QuarkAsset是一款unity资源管理的解决方案。摒弃了Resources原生的
    // 加载模式，主要针对AssetBundle在Editor模式与Runtime模式加载方式的
    //统一。
    //
    // 2、Editor模式下，加载方式主要依靠unity生成的gid进行资源寻址。通过
    // gid可以忽略由于更改文件夹地址导致的加载失败问题。
    //
    // 3、加载资源可直接通过资源名进行加载，无需通过相对地址或者完整路径
    //名。若文件资源相同，则可通过后缀名、相对于unity的assetType、以及完整
    //路径规避。
    //4、Quark设计方向是插件化，即插即用，因此内置了很多常用工具函数；
    //================================================
    public class QuarkManager : Singleton<QuarkManager>
    {
        QuarkManifest quarkManifest;
        /// <summary>
        /// AssetDataBase模式下资源的映射字典；
        /// Key : AssetName---Value :  Lnk [QuarkAssetObject]
        /// </summary>
        Dictionary<string, LinkedList<QuarkAssetDatabaseObject>> assetDatabaseMap;
        /// <summary>
        /// BuiltAssetBundle 模式下资源的映射；
        /// Key : ABName---Value :  Lnk [QuarkAssetABObject]
        /// </summary>` 
        Dictionary<string, LinkedList<QuarkAssetBundleObject>> builtAssetBundleMap;
        /// <summary>
        /// Key:[ABName] ; Value : [ABHash]
        /// </summary>
        Dictionary<string, string> assetBundleHashDict;

        /// <summary>
        /// Key : [ABName] ; Value : [AssetBundle]
        /// </summary>
        Dictionary<string, AssetBundle> assetBundleDict;
        public QuarkAssetLoadMode QuarkAssetLoadMode { get; set; }

        QuarkDownloader quarkDownloader;
        QuarkComparator quarkComparator;

        /// <summary>
        /// 本地缓存的地址；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// 远端存储的地址；
        /// </summary>
        public string URL { get; private set; }

        QuarkAssetDataset quarkAssetData;
        public QuarkAssetDataset QuarkAssetData { get { return quarkAssetData; } }

        /// <summary>
        /// Latest===Expired
        /// </summary>
        public event Action<string[], string[]> OnComparedDifferences
        {
            add { quarkComparator.OnComparedDifferences += value; }
            remove { quarkComparator.OnComparedDifferences -= value; }
        }
        /// <summary>
        /// 比较失败，传入ErrorMessage；
        /// </summary>
        public event Action<string> OnComparedFailure
        {
            add { quarkComparator.OnComparedFailure += value; }
            remove { quarkComparator.OnComparedFailure -= value; }
        }

        /// <summary>
        /// URL---DownloadPath
        /// </summary>
        public event Action<string, string> DownloadStart
        {
            add { quarkDownloader.OnDownloadStart += value; }
            remove { quarkDownloader.OnDownloadStart -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---Data
        /// </summary>
        public event Action<string, string, byte[]> DownloadSuccess
        {
            add { quarkDownloader.OnDownloadSuccess += value; }
            remove { quarkDownloader.OnDownloadSuccess -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---ErrorMessage
        /// </summary>
        public event Action<string, string, string> DownloadFailure
        {
            add { quarkDownloader.OnDownloadFailure += value; }
            remove { quarkDownloader.OnDownloadFailure -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---OverallProgress(0~100%)---IndividualProgress(0~100%)
        /// </summary>
        public event Action<string, string, float, float> DownloadOverall
        {
            add { quarkDownloader.OnDownloadOverall += value; }
            remove { quarkDownloader.OnDownloadOverall -= value; }
        }
        /// <summary>
        /// SuccessURIs---FailureURIs---TimeSpan
        /// </summary>
        public event Action<string[], string[], TimeSpan> DownloadFinish
        {
            add { quarkDownloader.OnDownloadFinish += value; }
            remove { quarkDownloader.OnDownloadFinish -= value; }
        }

        public void Init(string uri, string localPath)
        {
            quarkComparator = new QuarkComparator(uri, localPath);
        }

        /// <summary>
        /// 对Manifest进行编码；
        /// </summary>
        /// <param name="manifest">unityWebRequest获取的Manifest文件对象</param>
        public void SetBuiltAssetBundleModeData(QuarkManifest manifest)
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
        public void SetAssetDatabaseModeData(QuarkAssetDataset assetData)
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

        public void UnLoadAsset()
        {
        }

        #region BuiltAssetBundle
        #endregion
        public T[] LoadAllAsset<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            T[] asset = null;
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                asset = assetBundleDict[assetBundleName].LoadAllAssets<T>();
                if (asset == null)
                {
                    throw new ArgumentNullException($"AB包 {assetBundleName} 中不存在资源 {assetName} ！");
                }
            }
            return asset;
        }
        public T LoadAsset<T>(string assetName, string assetExtension = null)
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

        T LoadAssetFromABByName<T>(string assetPath, string assetExtension = null)
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
                // var assetBundle = LoadAssetBundle(abObject);
            }
            return null;
        }
        IEnumerator EnumLoadAssetBundleAsync(QuarkAssetBundleObject qabObject)
        {
            if (!assetBundleDict.TryGetValue(qabObject.AssetBundleName, out var assetBundle))
            {
                var url = Utility.IO.WebPathCombine(DownloadPath, qabObject.AssetBundleName);
                yield return QuarkUtility.Unity.DownloadAssetBundleAsync(url, null, ab =>
                {
                    assetBundleDict.TryAdd(qabObject.AssetBundleName, ab);
                    assetBundle = ab;
                });
            }
        }
#if UNITY_EDITOR
        T AssetDatabaseLoadAssetByName<T>(string assetName, string assetExtension = null)
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
        T AssetDatabaseLoadAssetByPath<T>(string assetPath)
       where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentNullException("Asset name is invalid!");
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
#endif
        T BuiltAssetBundleLoadAssetByName<T>(string assetPath, string assetExtension = null)
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
        T BuiltAssetBundleLoadAssetByPath<T>(string assetPath)
 where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentNullException("Asset name is invalid!");

            return null;
        }
        QuarkAssetDatabaseObject GetAssetDatabaseObject<T>(LinkedList<QuarkAssetDatabaseObject> lnk, string assetExtension = null)
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
        QuarkAssetBundleObject GetAssetBundleObject(string abName, string assetName)
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
        bool IsPath(string context)
        {
            return context.Contains("Assets/");
        }
    }
}
