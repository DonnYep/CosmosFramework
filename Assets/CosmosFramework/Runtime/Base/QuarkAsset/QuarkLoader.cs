using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.Quark
{
    public class QuarkLoader
    {
        /// <summary>
        /// Key:[ABName] ; Value : [ABHash]
        /// </summary>
        Dictionary<string, string> assetBundleHashDict;

        QuarkAssetDataset quarkAssetData;
        /// <summary>
        /// 存储ab包中包含的资源信息；
        /// </summary>
        QuarkManifest quarkManifest;
        /// <summary>
        /// 存储ab包之间的引用关系；
        /// </summary>
        QuarkBuildInfo quarkBuildInfo;
        public string LocalPath { get; private set; }
        /// <summary>
        /// Key : [ABName] ; Value : [AssetBundle]
        /// </summary>
        Dictionary<string, AssetBundle> assetBundleDict;
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
        public QuarkAssetLoadMode QuarkAssetLoadMode { get; set; }
        public QuarkLoader(string localPath)
        {
            LocalPath = localPath;
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
            quarkManifest = manifest;
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
        /// <summary>
        /// 加载指定ab包中的所有资源；
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="assetBundleName">指定的AB包名</param>
        /// <param name="callback">加载结束回调</param>
        public Coroutine LoadAllAssetAsync<T>(string assetBundleName, Action<T[]> callback) where T : UnityEngine.Object
        {
            T[] asset = null;
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                asset = assetBundleDict[assetBundleName].LoadAllAssets<T>();
                if (asset == null)
                {
                    callback?.Invoke(null);
                }
                callback?.Invoke(asset);
                return null;
            }
            else
            {
                var localFullPath = Path.Combine(assetBundleName, LocalPath);
                return QuarkUtility.Unity.DownloadAssetBundleAsync(localFullPath, null, ab =>
                {
                    assetBundleDict.Add(assetBundleName, ab);
                    asset = assetBundleDict[assetBundleName].LoadAllAssets<T>();
                    callback?.Invoke(asset);
                }, errorMessage => { callback?.Invoke(null); });
            }
        }
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false)
    where T : UnityEngine.Object
        {
            CheckBuildInfo();
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, callback, instantiate));
        }
        // todo
        public IEnumerator EnumLoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false)
            where T : UnityEngine.Object
        {
            T asset = null;
            string assetBundleName = string.Empty;
            foreach (var manifest in quarkManifest.ManifestDict)
            {
                var assets = manifest.Value.Assets;
                var length = assets.Length;
                for (int i = 0; i < length; i++)
                {
                    if (assets[i] == assetName)
                    {
                        assetBundleName = manifest.Value.ABName;
                        break;
                    }
                }
            }
            Utility.Debug.LogInfo($"Quark 加载 ab step 0：{assetBundleName}");

            if (string.IsNullOrEmpty(assetBundleName))
            {
                callback.Invoke(asset);
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(assetBundleName);

            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                asset = assetBundleDict[assetBundleName].LoadAsset<T>(assetName);
                if (asset != null)
                {
                    if (instantiate)
                    {
                        asset = GameObject.Instantiate(asset);
                    }
                }
            }
            if (asset != null)
                callback.Invoke(asset);
        }
        public T LoadAsset<T>(string assetName, string assetExtension = null)
where T : UnityEngine.Object
        {
            CheckBuildInfo();
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
        public void UnLoadAsset(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                assetBundleDict[assetBundleName].Unload(unloadAllLoadedObjects);
                assetBundleDict.Remove(assetBundleName);
            }
            if (assetBundleHashDict.ContainsKey(assetBundleName))
            {
                assetBundleHashDict.Remove(assetBundleName);
            }
        }
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            foreach (var assetBundle in assetBundleDict)
            {
                assetBundle.Value.Unload(unloadAllLoadedObjects);
            }
            assetBundleDict.Clear();
            assetBundleHashDict.Clear();
            AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
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
                if (!assetBundleDict.TryGetValue(abObject.AssetBundleName, out var bundle))
                {

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
                var url = Utility.IO.WebPathCombine(LocalPath, qabObject.AssetBundleName);
                yield return QuarkUtility.Unity.DownloadAssetBundleAsync(url, null, ab =>
                {
                    assetBundleDict.TryAdd(qabObject.AssetBundleName, ab);
                    assetBundle = ab;
                }, null);
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
                throw new ArgumentNullException("Asset name is invalid!"); if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
#endif
        T BuiltAssetBundleLoadAssetByName<T>(string assetName, string assetExtension = null)
  where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            QuarkAssetBundleObject abObject = QuarkAssetBundleObject.None;
            if (builtAssetBundleMap.TryGetValue(assetName, out var lnkList))
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
            }
            else
            {
                //QuarkUtility.Unity.DownloadAssetBundleAsync()
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

        IEnumerator EnumLoadDependenciesAssetBundleAsync(string assetBundleName)
        {
            Utility.Debug.LogInfo($"Quark 加载 ab step1：{assetBundleName}");

            if (quarkBuildInfo != null)
            {
                if (quarkBuildInfo.AssetDataMaps.TryGetValue(assetBundleName, out var buildInfo))
                {
                    Utility.Debug.LogInfo($"Quark 加载 ab step2：{assetBundleName}");
                    if (!assetBundleDict.ContainsKey(assetBundleName))
                    {
                        var abPath = Path.Combine(LocalPath, assetBundleName);
                        yield return QuarkUtility.Unity.DownloadAssetBundleAsync(abPath, null, ab =>
                        {
                            assetBundleDict.TryAdd(assetBundleName, ab);
                        }, null);
                    }
                    var dependList = buildInfo.DependList;
                    var length = dependList.Count;
                    for (int i = 0; i < length; i++)
                    {
                        var dependentABName = dependList[i];
                        var abPath = Path.Combine(LocalPath, dependentABName);
                        yield return QuarkUtility.Unity.DownloadAssetBundleAsync(abPath, null, ab =>
                        {
                            assetBundleDict.TryAdd(dependentABName, ab);
                        }, null);
                    }
                }
            }
            yield return null;
        }
        void CheckBuildInfo()
        {
            if (quarkBuildInfo == null)
            {
                try
                {
                    var buildInfoPath = Path.Combine(LocalPath, QuarkConsts.BuildInfoFileName);
                    var buildInfoContext = Utility.IO.ReadTextFileContent(buildInfoPath);
                    quarkBuildInfo = Utility.Json.ToObject<QuarkBuildInfo>(buildInfoContext);
                }
                catch (Exception e)
                {
                    Utility.Debug.LogInfo(e);
                }
            }
        }
    }
}
