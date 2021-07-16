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
        /// <summary>
        /// 本地缓存的地址；
        /// </summary>
        public string DownloadPath { get; set; }
        /// <summary>
        /// 远端存储的地址；
        /// </summary>
        public string URL { get; set; }

        QuarkAssetDataset quarkAssetData;
        public QuarkAssetDataset QuarkAssetData { get { return quarkAssetData; } }



        /// <summary>
        /// Latest===Expired
        /// </summary>
        Action<string[], string[]> onComparedDifferences;
        /// <summary>
        /// Latest===Expired
        /// </summary>
        public event Action<string[], string[]> OnComparedDifferences
        {
            add { OnComparedDifferences += value; }
            remove { onComparedDifferences -= value; }
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

        //名字相同，但是HASH不同，则认为资源有作修改，需要加入到下载队列中；
        List<string> downloadable = new List<string>();
        //本地有但是远程没有，则标记为可删除的文件，并加入到可删除队列；
        List<string> deletable = new List<string>();

        /// <summary>
        /// 对Manifest进行编码；
        /// </summary>
        /// <param name="manifest">unityWebRequest获取的Manifest文件对象</param>
        public  void SetBuiltAssetBundleModeData(QuarkManifest manifest)
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
        /// <summary>
        /// 检测 local与Remote之间manifest的差异；
        /// 若存在差异，回调中传入ture；
        /// 若不存在差异，回调中传入false；
        /// </summary>
        /// <param name="updatableCallback">是否可更新回调</param>
        public Coroutine CheckLatestManifestAsync()
        {
            return QuarkUtility.Unity.StartCoroutine(EnumCheckLatestManifest());
        }
        /// <summary>
        /// 异步下载AB资源；
        /// </summary>
        /// <param name="overallProgress">下载的整体进度</param>
        /// <param name="progress">单个资源下载的进度</param>
        /// <returns>协程对象</returns>
        public Coroutine DownloadAssetBundlesAsync(Action<float> overallProgress, Action<float> progress)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumDownloadAssetBundle(overallProgress, progress));
        }
        IEnumerator EnumCheckLatestManifest()
        {
            QuarkManifest localManifest = null;
            QuarkManifest remoteManifest = null;
            if (!string.IsNullOrEmpty(DownloadPath))
            {
                var localManifestUrl = Utility.IO.WebPathCombine(DownloadPath, QuarkConsts.ManifestName);
                yield return QuarkUtility.Unity.DownloadTextAsync(localManifestUrl, null, manifestText =>
                 {
                     try
                     {
                         localManifest = Utility.Json.ToObject<QuarkManifest>(manifestText);
                     }
                     catch { }
                 });
            }
            else
                throw new ArgumentNullException("LocalAssetBundleUrl is invalid ! ");
            if (!string.IsNullOrEmpty(URL))
            {
                var remoteManifestUrl = Utility.IO.WebPathCombine(URL, QuarkConsts.ManifestName);
                yield return QuarkUtility.Unity.DownloadTextAsync(remoteManifestUrl, null, manifestText =>
                 {
                     try
                     {
                         remoteManifest = Utility.Json.ToObject<QuarkManifest>(manifestText);
                     }
                     catch { }
                 });
            }
            else
                throw new ArgumentNullException("RemoteManifestUrl is invalid ! ");

            if (localManifest != null)
            {
                //若本地的Manifest不为空，远端的Manifest不为空，则对比二者之间的差异；
                //远端有本地没有，则缓存至downloadable；
                //远端没有本地有，则缓存至deleteable；
                if (remoteManifest != null)
                {
                    foreach (var remoteMF in remoteManifest.ManifestDict)
                    {
                        if (localManifest.ManifestDict.TryGetValue(remoteMF.Key, out var localMF))
                        {
                            if (localMF.Hash != remoteMF.Value.Hash)
                            {
                                downloadable.Add(remoteMF.Value.ABName);
                            }
                        }
                        else
                        {
                            downloadable.Add(remoteMF.Value.ABName);
                        }
                    }
                    foreach (var localMF in localManifest.ManifestDict)
                    {
                        if (!remoteManifest.ManifestDict.ContainsKey(localMF.Key))
                        {
                            deletable.Add(localMF.Key);
                        }
                    }
                }
            }
            else
            {
                //若本地的Manifest为空，远端的Manifest不为空，则将需要下载的资源url缓存到downloadable;
                if (remoteManifest != null)
                {
                    downloadable.AddRange(remoteManifest.ManifestDict.Keys.ToList());
                }
            }
            downloadable.TrimExcess();
            deletable.TrimExcess();
            onComparedDifferences?.Invoke(downloadable.ToArray(), deletable.ToArray());
        }
        IEnumerator EnumDownloadAssetBundle(Action<float> overallProgress, Action<float> progress)
        {
            var downloadableAssetUrls = new string[downloadable.Count];
            var downloadLength = downloadableAssetUrls.Length;
            for (int i = 0; i < downloadLength; i++)
            {
                downloadableAssetUrls[i] = Utility.IO.WebPathCombine(URL, downloadable[i]);
            }
            //删除本地多余的资源；
            var deleteLength = deletable.Count;
            for (int i = 0; i < deleteLength; i++)
            {
                var deleteFilePath = Utility.IO.WebPathCombine(DownloadPath, deletable[i]);
                Utility.IO.DeleteFile(deleteFilePath);
            }
            yield return QuarkUtility.Unity.DownloadAssetBundlesBytesAsync(downloadableAssetUrls, overallProgress, progress, bundleBytes =>
            {
                var bundleLength = bundleBytes.Count;
                for (int i = 0; i < bundleLength; i++)
                {
                    var cachePath = Utility.IO.WebPathCombine(DownloadPath, downloadable[i]);
                    Utility.IO.WriteFile(bundleBytes[i], cachePath);
                }
            });
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
