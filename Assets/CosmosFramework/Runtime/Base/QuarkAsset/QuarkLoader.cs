using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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
        Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();
        /// <summary>
        /// AssetDataBase模式下资源的映射字典；
        /// Key : AssetName---Value :  Lnk [QuarkAssetObject]
        /// </summary>
        Dictionary<string, LinkedList<QuarkAssetDatabaseObject>> assetDatabaseMap
            = new Dictionary<string, LinkedList<QuarkAssetDatabaseObject>>();
        /// <summary>
        /// BuiltAssetBundle 模式下资源的映射；
        /// Key : AssetName---Value :  Lnk [QuarkAssetABObject]
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
                var assets = mf.Value.Assets;
                if (assets == null)
                    continue;
                foreach (var a in assets)
                {
                    var abObject = GetAssetBundleObject(mf.Value.ABName, a.Key, a.Value);
                    if (!lnkDict.TryGetValue(abObject.AssetName, out var lnkList))
                    {
                        lnkList = new LinkedList<QuarkAssetBundleObject>();
                        lnkList.AddLast(abObject);
                        lnkDict.Add(abObject.AssetName, lnkList);
                    }
                    else
                    {
                        lnkList.AddLast(abObject);
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
                return QuarkUtility.Unity.StartCoroutine(EnumRequestAssetBundle(localFullPath, ab =>
               {
                   assetBundleDict.Add(assetBundleName, ab);
                   asset = assetBundleDict[assetBundleName].LoadAllAssets<T>();
                   callback?.Invoke(asset);
               }, errorMessage => { callback?.Invoke(null); }));
            }
        }
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false)
    where T : UnityEngine.Object
        {
            CheckBuildInfo();
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, callback, instantiate));
        }

        public T LoadAsset<T>(string assetName, string assetExtension = null)
where T : UnityEngine.Object
        {
            CheckBuildInfo();
            T asset = null;
            var isPath = assetName.Contains("Assets/");
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
        QuarkAssetBundleObject GetAssetBundleObject(string abName, string assetPath, string assetName)
        {
            var abObject = new QuarkAssetBundleObject()
            {
                AssetBundleName = abName,
                AssetPath = assetPath,
            };
            var strs = Utility.Text.StringSplit(assetPath, new string[] { "/" });
            var nameWithExt = strs[strs.Length - 1];
            var splits = Utility.Text.StringSplit(nameWithExt, new string[] { "." });
            abObject.AssetExtension = Utility.Text.Combine(".", splits[splits.Length - 1]);
            abObject.AssetName = assetName;
            return abObject;
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
                    Utility.Debug.LogError(e);
                }
            }
        }
        IEnumerator EnumRequestAssetBundle(string uri, Action<AssetBundle> doneCallback, Action<string> failureCallback)
        {
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    failureCallback?.Invoke(request.error);
                }
                else
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                    doneCallback?.Invoke(bundle);
                }
            }
        }
        IEnumerator EnumLoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false)
    where T : UnityEngine.Object
        {
            T asset = null;
            string assetBundleName = string.Empty;
            if (builtAssetBundleMap.TryGetValue(assetName, out var abObject))
            {
                assetBundleName = abObject.First.Value.AssetBundleName;
            }
            else
            {
                Utility.Debug.LogError($"资源中不存在：{assetName}");
                yield break;
            }
            if (string.IsNullOrEmpty(assetBundleName))
            {
                callback?.Invoke(asset);
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
                callback?.Invoke(asset);
        }
        IEnumerator EnumLoadDependenciesAssetBundleAsync(string assetBundleName)
        {
            if (quarkBuildInfo != null)
            {
                if (!assetBundleDict.ContainsKey(assetBundleName))
                {
                    var abPath = Path.Combine(LocalPath, assetBundleName);
                    yield return EnumRequestAssetBundle(abPath, ab =>
                    {
                        assetBundleDict.TryAdd(assetBundleName, ab);
                    }, null);
                }
                QuarkBuildInfo.AssetData buildInfo = null;
                foreach (var item in quarkBuildInfo.AssetDataMaps)
                {
                    if (item.Value.ABName == assetBundleName)
                    {
                        buildInfo = item.Value;
                        break;
                    }
                }
                if (buildInfo != null)
                {
                    var dependList = buildInfo.DependList;
                    var length = dependList.Count;
                    for (int i = 0; i < length; i++)
                    {
                        var dependentABName = dependList[i];
                        if (!assetBundleDict.ContainsKey(dependentABName))
                        {
                            var abPath = Path.Combine(LocalPath, dependentABName);
                            Utility.Debug.LogInfo($"加载 {assetBundleName} 的依赖AB包：{dependentABName}");
                            yield return EnumRequestAssetBundle(abPath, ab =>
                            {
                                assetBundleDict.TryAdd(dependentABName, ab);
                            }, null);
                        }
                    }
                }
            }
        }
    }
}
